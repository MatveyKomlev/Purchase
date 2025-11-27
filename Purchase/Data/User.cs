using System.ComponentModel.DataAnnotations;

namespace Purchase.Data
{
    public class User
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Имя пользователя обязательно")]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required] 
        public string PasswordSalt { get; set; } = string.Empty;

        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

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