using Microsoft.EntityFrameworkCore;

namespace Purchase.Data
{
    public static class SeedData
    {
        public static void Initialize(PurchaseContext context)
        {
            // Проверяем, есть ли уже пользователи
            if (context.Users.Any())
                return;

            // Генерируем реальный хэш и соль для пароля "admin123"
            var password = "admin123";
            var salt = GenerateSalt();
            var hash = HashPassword(password, salt);

            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@company.com",
                PasswordHash = hash,
                PasswordSalt = salt,
                FullName = "Администратор системы",
                Role = UserRole.Admin,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            context.Users.Add(adminUser);
            context.SaveChanges();
        }

        private static string GenerateSalt()
        {
            using var hmac = new System.Security.Cryptography.HMACSHA256();
            return Convert.ToBase64String(hmac.Key);
        }

        private static string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using var hmac = new System.Security.Cryptography.HMACSHA256(saltBytes);
            var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = hmac.ComputeHash(passwordBytes);
            return Convert.ToBase64String(hash);
        }
    }
}