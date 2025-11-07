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
        [MaxLength(200)] 
        public string? Material { get; set; }

        [Required(ErrorMessage = "Категория обязательна")]
        [MaxLength(100)] 
        public string? Category { get; set; }

        [MaxLength(100)]
        public string? ManufacturerPartNumber { get; set; }

        [MaxLength(150)] 
        public string? ManufacturerName { get; set; }

        [MaxLength(20)]
        public string? UnitOfMeasure { get; set; }

        [Required(ErrorMessage = "Цена обязательна")] 
        [Range(0, int.MaxValue, ErrorMessage = "Цена не может быть отрицательной")]
        public int Price { get; set; }

        public virtual List<ProposalMaterial> ProposalMaterials { get; set; } = new();
    }
}