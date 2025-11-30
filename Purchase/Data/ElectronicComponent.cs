using System.ComponentModel.DataAnnotations;

namespace Purchase.Data
{
    /// <summary>
    /// Расширенная информация об электронном компоненте
    /// </summary>
    public class ElectronicComponent
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Артикул производителя обязателен")]
        [StringLength(100)]
        public string ManufacturerPartNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Производитель обязателен")]
        [StringLength(150)]
        public string Manufacturer { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описание обязательно")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string DatasheetUrl { get; set; } = string.Empty;

        // Статус соответствия регламентам
        public ComplianceStatus ComplianceStatus { get; set; } = ComplianceStatus.Unknown;

        // Процент локализации (0-100)
        [Range(0, 100)]
        public int LocalizationPercent { get; set; } = 0;

        // Связь со стандартами (многие-ко-многим)
        public virtual List<ComponentStandard> Standards { get; set; } = new();

        // Внешний ключ на каталог
        public int? CatalogId { get; set; }
        public virtual ProposalCatalog? Catalog { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastVerifiedAt { get; set; }
    }

    public enum ComplianceStatus
    {
        [Display(Name = "❓ Не проверено")]
        Unknown,

        [Display(Name = "✅ Соответствует регламенту")]
        Compliant,

        [Display(Name = "⚠️ Требует проверки")]
        NeedsReview,

        [Display(Name = "❌ Не соответствует")]
        NonCompliant
    }
}