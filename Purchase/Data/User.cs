using System.ComponentModel.DataAnnotations;

namespace Purchase.Data
{
    public class User
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Имя пользователя обязательно")]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string PasswordSalt { get; set; } = string.Empty;

        [Required(ErrorMessage = "ФИО обязательно")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Отдел обязателен")]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty; // ← ДОБАВИЛИ ПОЛЕ

        public UserRole Role { get; set; } = UserRole.User;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLogin { get; set; }
    }

    public enum UserRole
    {
        User,
        Admin
    }
}