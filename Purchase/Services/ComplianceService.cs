using Microsoft.EntityFrameworkCore;
using Purchase.Data;

namespace Purchase.Services
{
    public class ComplianceService
    {
        private readonly IDbContextFactory<PurchaseContext> _contextFactory;

        public ComplianceService(IDbContextFactory<PurchaseContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Проверка китайских компонентов на соответствие регламентам
        /// </summary>
        public async Task<ComplianceCheckResult> CheckComplianceAsync(int componentId)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                var component = await context.ElectronicComponents
                    .Include(ec => ec.Standards)
                    .FirstOrDefaultAsync(ec => ec.ID == componentId);

                if (component == null)
                {
                    return new ComplianceCheckResult
                    {
                        Success = false,
                        ErrorMessage = "Компонент не найден"
                    };
                }

                // ПРИМЕНЯЕМ ЭВРИСТИКИ ДЛЯ КИТАЙСКИХ КОМПОНЕНТОВ
                var checks = new List<ComplianceCheck>();

                // Эвристика 1: Локализация для китайских компонентов
                var localizationCheck = CheckLocalizationForChina(component);
                checks.Add(localizationCheck);

                // Эвристика 2: Наличие китайских стандартов
                var standardsCheck = CheckChineseStandards(component);
                checks.Add(standardsCheck);

                // Эвристика 3: Категория компонента
                var categoryCheck = CheckComponentCategory(component);
                checks.Add(categoryCheck);

                // Эвристика 4: Наличие документации
                var documentationCheck = CheckDocumentation(component);
                checks.Add(documentationCheck);

                // ОБЩИЙ РЕЗУЛЬТАТ для китайских компонентов
                var passedChecks = checks.Count(c => c.Passed);
                var totalChecks = checks.Count;

                // ЛОГИКА ОПРЕДЕЛЕНИЯ СТАТУСА для Китая
                if (passedChecks >= 3) // 3 из 4 проверок
                {
                    component.ComplianceStatus = ComplianceStatus.Compliant;
                }
                else if (passedChecks >= 2) // 2 из 4 проверок
                {
                    component.ComplianceStatus = ComplianceStatus.NeedsReview;
                }
                else
                {
                    component.ComplianceStatus = ComplianceStatus.NonCompliant;
                }

                component.LastVerifiedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();

                return new ComplianceCheckResult
                {
                    Success = true,
                    Component = component,
                    Checks = checks,
                    PassedChecks = passedChecks,
                    TotalChecks = totalChecks,
                    ComplianceStatus = component.ComplianceStatus
                };
            }
            catch (Exception ex)
            {
                return new ComplianceCheckResult
                {
                    Success = false,
                    ErrorMessage = $"Ошибка проверки: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Эвристика: Проверка локализации для китайских компонентов
        /// </summary>
        private ComplianceCheck CheckLocalizationForChina(ElectronicComponent component)
        {
            // Для китайских компонентов высокий процент локализации - хорошо
            var passed = component.LocalizationPercent >= 50;
            return new ComplianceCheck
            {
                Name = "Уровень локализации в Китае",
                Description = "Высокий процент локализации упрощает логистику",
                Passed = passed,
                RequiredValue = "≥ 50%",
                ActualValue = $"{component.LocalizationPercent}%",
                Weight = 2 // Более важная проверка
            };
        }

        /// <summary>
        /// Эвристика: Проверка китайских стандартов
        /// </summary>
        private ComplianceCheck CheckChineseStandards(ElectronicComponent component)
        {
            var hasChineseStandards = component.Standards.Any(s =>
                s.Name.Contains("GB") || s.Name.Contains("中国") || s.Name.Contains("China"));

            var hasInternationalStandards = component.Standards.Any(s =>
                s.Name.Contains("ISO") || s.Name.Contains("IEC") || s.Name.Contains("RoHS"));

            // Проходим проверку если есть либо китайские, либо международные стандарты
            var passed = hasChineseStandards || hasInternationalStandards;

            return new ComplianceCheck
            {
                Name = "Соответствие стандартам",
                Description = "Наличие китайских (GB) или международных стандартов",
                Passed = passed,
                RequiredValue = "GB/ISO/IEC стандарты",
                ActualValue = hasChineseStandards ? "Китайские стандарты" :
                             hasInternationalStandards ? "Международные стандарты" : "Стандарты не указаны"
            };
        }

        /// <summary>
        /// Эвристика: Проверка категории компонента
        /// </summary>
        private ComplianceCheck CheckComponentCategory(ElectronicComponent component)
        {
            // Критичные компоненты требуют более строгой проверки
            var criticalComponents = new[]
            {
                "STM32", "ESP32", "Arduino", "Microcontroller", "Processor",
                "Wireless", "RF", "Bluetooth", "Wi-Fi"
            };

            var isCritical = criticalComponents.Any(keyword =>
                component.Description?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true);

            // Для некритичных компонентов проверка проще
            var passed = !isCritical;

            return new ComplianceCheck
            {
                Name = "Категория компонента",
                Description = isCritical ?
                    "Критичный компонент - требует тщательной проверки" :
                    "Некритичный компонент - стандартные требования",
                Passed = passed,
                RequiredValue = "Некритичный компонент",
                ActualValue = isCritical ? "Критичный" : "Некритичный"
            };
        }

        /// <summary>
        /// Эвристика: Проверка документации
        /// </summary>
        private ComplianceCheck CheckDocumentation(ElectronicComponent component)
        {
            var hasDatasheet = !string.IsNullOrEmpty(component.DatasheetUrl);
            var hasManufacturer = !string.IsNullOrEmpty(component.Manufacturer) &&
                                 component.Manufacturer != "Неизвестно";

            var passed = hasDatasheet && hasManufacturer;

            return new ComplianceCheck
            {
                Name = "Техническая документация",
                Description = "Наличие datasheet и информации о производителе",
                Passed = passed,
                RequiredValue = "Datasheet + производитель",
                ActualValue = hasDatasheet && hasManufacturer ? "Полная документация" :
                             hasManufacturer ? "Только производитель" : "Документация отсутствует"
            };
        }
    }

    public class ComplianceCheckResult
    {
        public bool Success { get; set; }
        public ElectronicComponent? Component { get; set; }
        public List<ComplianceCheck> Checks { get; set; } = new();
        public int PassedChecks { get; set; }
        public int TotalChecks { get; set; }
        public ComplianceStatus ComplianceStatus { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ComplianceCheck
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Passed { get; set; }
        public string RequiredValue { get; set; } = string.Empty;
        public string ActualValue { get; set; } = string.Empty;
        public int Weight { get; set; } = 1;
    }
}