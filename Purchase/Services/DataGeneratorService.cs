using Microsoft.EntityFrameworkCore;
using Purchase.Data;

namespace Purchase.Services
{
    public class DataGeneratorService
    {
        private readonly IDbContextFactory<PurchaseContext> _contextFactory;

        public DataGeneratorService(IDbContextFactory<PurchaseContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Генерация массовых тестовых данных
        /// </summary>
        public async Task GenerateTestDataAsync(int usersCount = 50, int proposalsCount = 500, int componentsCount = 1000)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            Console.WriteLine("🧪 Начало генерации тестовых данных...");

            // 1. Генерация пользователей
            await GenerateUsersAsync(context, usersCount);

            // 2. Генерация компонентов
            await GenerateElectronicComponentsAsync(context, componentsCount);

            // 3. Генерация заявок
            await GenerateProposalsAsync(context, proposalsCount);

            Console.WriteLine("✅ Генерация тестовых данных завершена!");
        }

        private async Task GenerateUsersAsync(PurchaseContext context, int count)
        {
            var departments = new[] { "IT", "Производство", "Закупки", "R&D", "QA", "Логистика" };
            var users = new List<User>();

            for (int i = 1; i <= count; i++)
            {
                var user = new User
                {
                    Username = $"user{i}",
                    Email = $"user{i}@company.com",
                    PasswordHash = "temp123",
                    PasswordSalt = "temp_salt",
                    FullName = $"Тестовый пользователь {i}",
                    Department = departments[Random.Shared.Next(departments.Length)],
                    Role = i % 10 == 0 ? UserRole.Admin : UserRole.User, // Каждый 10й - админ
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(365))
                };
                users.Add(user);
            }

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
            Console.WriteLine($"👥 Создано {count} пользователей");
        }

        private async Task GenerateElectronicComponentsAsync(PurchaseContext context, int count)
        {
            var components = new List<ElectronicComponent>();
            var manufacturers = new[]
            {
                "STMicroelectronics", "Texas Instruments", "Infineon", "NXP",
                "Microchip", "Analog Devices", "ON Semiconductor", "Vishay",
                "Murata", "TDK", "Samsung Electro-Mechanics", "UNI-ROYAL"
            };

            var componentTypes = new[]
            {
                ("Микроконтроллер", "STM32F103C8T6", 65),
                ("Стабилизатор", "AMS1117-3.3", 70),
                ("Светодиод", "LED Red 5mm", 80),
                ("Резистор", "Resistor 10k 0805", 85),
                ("Конденсатор", "Cap 100nF 50V", 75),
                ("Транзистор", "2N3904", 70),
                ("Датчик", "DHT22", 60),
                ("Диод", "1N4148", 75)
            };

            for (int i = 1; i <= count; i++)
            {
                var componentType = componentTypes[Random.Shared.Next(componentTypes.Length)];
                var component = new ElectronicComponent
                {
                    ManufacturerPartNumber = $"C{100000 + i}",
                    Manufacturer = manufacturers[Random.Shared.Next(manufacturers.Length)],
                    Description = $"{componentType.Item1} {componentType.Item2}",
                    LocalizationPercent = componentType.Item3 + Random.Shared.Next(-10, 10),
                    ComplianceStatus = (ComplianceStatus)Random.Shared.Next(0, 3),
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(180))
                };
                components.Add(component);
            }

            context.ElectronicComponents.AddRange(components);
            await context.SaveChangesAsync();
            Console.WriteLine($"🔧 Создано {count} электронных компонентов");
        }

        private async Task GenerateProposalsAsync(PurchaseContext context, int count)
        {
            var users = await context.Users.ToListAsync();
            var components = await context.ElectronicComponents.Take(200).ToListAsync();

            var proposals = new List<Proposal>();
            var statuses = new[] { ErpStatus.InProgress, ErpStatus.Approved, ErpStatus.Rejected };
            var priorities = new[] { Priorities.Low, Priorities.Medium, Priorities.High, Priorities.Critical };

            for (int i = 1; i <= count; i++)
            {
                var user = users[Random.Shared.Next(users.Count)];
                var proposal = new Proposal
                {
                    Number = $"З-2024-{i:D4}",
                    DateCreation = DateTime.UtcNow.AddDays(-Random.Shared.Next(90)),
                    Author = user.FullName,
                    Department = user.Department,
                    Status = statuses[Random.Shared.Next(statuses.Length)],
                    Priority = priorities[Random.Shared.Next(priorities.Length)],
                    Deadline = Random.Shared.Next(5) == 0 ? DateTime.UtcNow.AddDays(Random.Shared.Next(30)) : null,
                    Explanation = $"Тестовая заявка #{i}",
                    UserId = user.ID,
                    Materials = GenerateProposalMaterials(components, Random.Shared.Next(1, 8))
                };
                proposals.Add(proposal);
            }

            context.Proposals.AddRange(proposals);
            await context.SaveChangesAsync();
            Console.WriteLine($"📋 Создано {count} заявок с материалами");
        }

        private List<ProposalMaterial> GenerateProposalMaterials(List<ElectronicComponent> components, int count)
        {
            var materials = new List<ProposalMaterial>();
            var statuses = new[] { MaterialStatus.New, MaterialStatus.InCatalog, MaterialStatus.Closed };

            for (int i = 0; i < count; i++)
            {
                var component = components[Random.Shared.Next(components.Count)];
                var material = new ProposalMaterial
                {
                    NameMaterial = component.Description,
                    CategoryMaterial = "Электронные компоненты",
                    Quantity = Random.Shared.Next(1, 101),
                    EstimatedPrice = Random.Shared.Next(1, 1000),
                    StatusM = statuses[Random.Shared.Next(statuses.Length)],
                    CatalogId = null
                };
                materials.Add(material);
            }

            return materials;
        }
    }
}