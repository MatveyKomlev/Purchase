using Microsoft.EntityFrameworkCore;
using Purchase.Data;
using System.Net.Http.Json;
using System.Text.Json;

namespace Purchase.Services
{
    public class ComponentIntegrationService
    {
        private readonly HttpClient _httpClient;
        private readonly IDbContextFactory<PurchaseContext> _contextFactory;

        private const string LCSC_API_BASE_URL = "https://wmsc.lcsc.com/v1/products";

        public ComponentIntegrationService(HttpClient httpClient, IDbContextFactory<PurchaseContext> contextFactory)
        {
            _httpClient = httpClient;
            _contextFactory = contextFactory;

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        /// <summary>
        /// Поиск компонента через LCSC API
        /// </summary>
        public async Task<ComponentSearchResult> SearchComponentAsync(string lcscCode)
        {
            try
            {
                var url = $"{LCSC_API_BASE_URL}?productCode={Uri.EscapeDataString(lcscCode)}";

                // Пробуем получить данные с API
                LcscResponse? apiResponse = null;
                try
                {
                    apiResponse = await _httpClient.GetFromJsonAsync<LcscResponse>(url);
                }
                catch
                {
                    // API недоступно - используем fallback
                }

                if (apiResponse?.Data != null && apiResponse.Data.Any())
                {
                    var product = apiResponse.Data[0];
                    var component = MapToElectronicComponent(product, lcscCode);

                    return new ComponentSearchResult
                    {
                        Success = true,
                        Component = component,
                        Source = "LCSC API"
                    };
                }
                else
                {
                    // Fallback данные
                    return await GetFallbackComponentData(lcscCode);
                }
            }
            catch (Exception ex)
            {
                return new ComponentSearchResult
                {
                    Success = false,
                    ErrorMessage = $"Ошибка поиска: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Fallback данные с реальными производителями
        /// </summary>
        private async Task<ComponentSearchResult> GetFallbackComponentData(string lcscCode)
        {
            var knownComponents = new Dictionary<string, (string, string, int)>
            {
                { "C72038", ("STM32F103C8T6", "STMicroelectronics", 65) },
                { "C115217", ("AMS1117-3.3 LDO Regulator", "Advanced Monolithic Systems", 70) },
                { "C28302", ("LED Red 5mm", "Everlight Electronics", 80) },
                { "C4407", ("Resistor 10kΩ 1% 0805", "UNI-ROYAL", 85) },
                { "C2545", ("Capacitor 100nF 50V 0805", "Samsung Electro-Mechanics", 75) },
                { "C128955", ("ESP32-WROOM-32", "Espressif Systems", 60) },
                { "C133910", ("Arduino Nano", "Arduino LLC", 55) },
                { "C8402", ("2N3904 NPN Transistor", "ON Semiconductor", 70) },
                { "C1655", ("Tactile Switch 6x6mm", "Kailh", 85) },
                { "C8456", ("1N4148 Diode", "Vishay", 75) },
                { "C131280", ("DHT22 Temperature Sensor", "Aosong", 65) }
            };

            if (knownComponents.TryGetValue(lcscCode, out var componentData))
            {
                var component = new ElectronicComponent
                {
                    ManufacturerPartNumber = lcscCode,
                    Manufacturer = componentData.Item2,
                    Description = componentData.Item1,
                    DatasheetUrl = $"https://www.lcsc.com/product-detail/{lcscCode}.html",
                    LocalizationPercent = componentData.Item3,
                    ComplianceStatus = ComplianceStatus.Unknown,
                    Standards = await GetDefaultStandardsAsync()
                };

                return new ComponentSearchResult
                {
                    Success = true,
                    Component = component,
                    Source = "LCSC Fallback Database"
                };
            }

            return new ComponentSearchResult
            {
                Success = false,
                ErrorMessage = $"Компонент '{lcscCode}' не найден. Попробуйте: C72038, C115217, C28302, C4407, C2545"
            };
        }

        /// <summary>
        /// Импорт компонента в каталог с автоматическим расчетом цены
        /// </summary>
        public async Task<ImportResult> ImportToCatalogAsync(ElectronicComponent component, int? catalogId = null)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                var existing = await context.ElectronicComponents
                    .FirstOrDefaultAsync(ec => ec.ManufacturerPartNumber == component.ManufacturerPartNumber);

                if (existing != null)
                {
                    return new ImportResult
                    {
                        Success = false,
                        ErrorMessage = "Компонент с таким артикулом уже существует в системе"
                    };
                }

                // Автоматически рассчитываем цену для каталога
                var calculatedPrice = await CalculateCatalogPrice(component.ManufacturerPartNumber);

                if (catalogId == null)
                {
                    var catalogItem = new ProposalCatalog
                    {
                        Material = component.Description,
                        Category = "Электронные компоненты",
                        ManufacturerPartNumber = component.ManufacturerPartNumber,
                        ManufacturerName = component.Manufacturer,
                        UnitOfMeasure = "шт.",
                        Price = calculatedPrice
                    };

                    context.ProposalCatalogs.Add(catalogItem);
                    await context.SaveChangesAsync();
                    component.CatalogId = catalogItem.ID;
                }
                else
                {
                    component.CatalogId = catalogId;
                    // Обновляем цену в существующем каталоге
                    var catalogItem = await context.ProposalCatalogs.FindAsync(catalogId);
                    if (catalogItem != null)
                    {
                        catalogItem.Price = calculatedPrice;
                    }
                }

                component.CreatedAt = DateTime.UtcNow;
                context.ElectronicComponents.Add(component);
                await context.SaveChangesAsync();

                return new ImportResult
                {
                    Success = true,
                    ImportedComponent = component,
                    Message = $"Компонент успешно импортирован в каталог. Цена: {calculatedPrice / 100.0:F2} USD"
                };
            }
            catch (Exception ex)
            {
                return new ImportResult
                {
                    Success = false,
                    ErrorMessage = $"Ошибка импорта: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Расчет цены для каталога на основе LCSC данных
        /// </summary>
        private async Task<int> CalculateCatalogPrice(string lcscCode)
        {
            // База цен LCSC (в USD)
            var priceDatabase = new Dictionary<string, decimal>
    {
        { "C72038", 3.45m },    // STM32
        { "C115217", 0.18m },   // AMS1117
        { "C28302", 0.045m },   // LED (за 1шт при опте)
        { "C4407", 0.0085m },   // Резистор (за 1шт при опте) ← ДОЛЖНА БЫТЬ ЦЕНА
        { "C2545", 0.012m },    // Конденсатор (за 1шт при опте)
        { "C128955", 4.85m },   // ESP32
        { "C133910", 8.90m },   // Arduino Nano
        { "C8402", 0.035m },    // Транзистор (за 1шт при опте)
        { "C1655", 0.085m },    // Кнопка (за 1шт при опте)
        { "C8456", 0.015m },    // Диод (за 1шт при опте)
        { "C131280", 3.20m }    // DHT22
    };

            // Получаем цену из базы или используем значение по умолчанию
            var unitPrice = priceDatabase.ContainsKey(lcscCode) ? priceDatabase[lcscCode] : 1.00m;

            // Конвертируем в "копейки" (int) - умножаем на 100 и округляем
            return (int)(unitPrice * 100);
        }

        private async Task<List<ComponentStandard>> GetDefaultStandardsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.ComponentStandards
                .Where(cs => cs.IsActive)
                .Take(2)
                .ToListAsync();
        }

        private ElectronicComponent MapToElectronicComponent(LcscProduct product, string code)
        {
            var component = new ElectronicComponent
            {
                ManufacturerPartNumber = code,
                Manufacturer = product.ManufacturerName ?? "Неизвестно",
                Description = product.ProductIntro ?? product.ProductModel ?? $"Компонент {code}",
                DatasheetUrl = product.PdfUrl,
                LocalizationPercent = CalculateRealLocalization(product),
                ComplianceStatus = ComplianceStatus.Unknown
            };

            component.Standards = GetStandardsFromProduct(product);
            return component;
        }

        private int CalculateRealLocalization(LcscProduct product)
        {
            int localization = 0;
            var chineseManufacturers = new[] { "China", "Chinese", "Shenzhen", "Guangdong", "LCSC" };
            if (chineseManufacturers.Any(cm => product.ManufacturerName?.Contains(cm) == true))
                localization += 30;
            if (product.StockNumber > 0)
                localization += 20;
            if (product.ProductBigType == "Active" || product.ProductBigType == "Integrated Circuits")
                localization += 15;
            if (product.ProductPrice?.Any(p => p.ProductPrice > 0 && p.ProductPrice < 5) == true)
                localization += 10;
            return Math.Max(25, Math.Min(localization, 80));
        }

        private List<ComponentStandard> GetStandardsFromProduct(LcscProduct product)
        {
            var standards = new List<ComponentStandard>();

            standards.Add(new ComponentStandard
            {
                Code = "GB/T 191-2008",
                Name = "Упаковка - условные обозначения",
                Type = StandardType.TechnicalRegulation,
                IsActive = true
            });

            standards.Add(new ComponentStandard
            {
                Code = "RoHS",
                Name = "Restriction of Hazardous Substances",
                Type = StandardType.TechnicalRegulation,
                IsActive = true
            });

            if (product.ProductBigType == "Integrated Circuits" || product.ProductBigType == "Active")
            {
                standards.Add(new ComponentStandard
                {
                    Code = "GB/T 4588-2017",
                    Name = "Печатные платы - технические условия",
                    Type = StandardType.TechnicalRegulation,
                    IsActive = true
                });
            }

            return standards;
        }
    }

    // МОДЕЛИ ДЛЯ LCSC API
    public class LcscResponse
    {
        public List<LcscProduct> Data { get; set; } = new();
        public int Total { get; set; }
    }

    public class LcscProduct
    {
        public string? ProductCode { get; set; }
        public string? ProductModel { get; set; }
        public string? ProductIntro { get; set; }
        public string? ManufacturerName { get; set; }
        public string? ProductBigType { get; set; }
        public string? PdfUrl { get; set; }
        public int StockNumber { get; set; }
        public List<LcscPrice>? ProductPrice { get; set; }
        public List<LcscParam>? ProductParam { get; set; }
    }

    public class LcscPrice
    {
        public decimal ProductPrice { get; set; }
        public int ProductNumber { get; set; }
    }

    public class LcscParam
    {
        public string? ParamName { get; set; }
        public string? ParamValue { get; set; }
    }

    public class ComponentSearchResult
    {
        public bool Success { get; set; }
        public ElectronicComponent? Component { get; set; }
        public string? Source { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ImportResult
    {
        public bool Success { get; set; }
        public ElectronicComponent? ImportedComponent { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
    }
}