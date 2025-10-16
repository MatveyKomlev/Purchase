using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Purchase.Data
{
    public class ProposalCatalog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "#")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Материал обязателен")]
        public string? Material { get; set; }

        [Required(ErrorMessage = "Категория обязательна")]
        public string? Category { get; set; }

        [MaxLength(50)]
        public string? ManufacturerPartNumber { get; set; }

        [MaxLength(100)]
        public string? ManufacturerName { get; set; }

        [MaxLength(20)]
        public string? UnitOfMeasure { get; set; }

        // Навигационное свойство к материалам заявок
        public List<ProposalMaterial> ProposalMaterials { get; set; } = new();

    }
}
