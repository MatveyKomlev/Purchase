using System.ComponentModel.DataAnnotations;

namespace Purchase.Data
{
    /// <summary>
    /// Стандарт или технический регламент для компонентов
    /// </summary>
    public class ComponentStandard
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Код стандарта обязателен")]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;  // "ГОСТ 2.701-2008"

        [Required(ErrorMessage = "Название стандарта обязательно")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;  // "Схемы виды типы"

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public StandardType Type { get; set; } = StandardType.GOST;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum StandardType
    {
        GOST,           // Российские стандарты
        ISO,            // Международные
        TechnicalRegulation, // Технические регламенты
        EnterpriseStandard   // Внутренние стандарты предприятия
    }
}