using Purchase.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProposalMaterial
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "#")]
    public int ID { get; set; }

    [Required(ErrorMessage = "Название материала обязательно")]
    [MaxLength(200)]
    public string? NameMaterial { get; set; }

    [Required(ErrorMessage = "Категория обязательна")]
    [MaxLength(100)]
    public string? CategoryMaterial { get; set; }

    [MaxLength(50)]
    public string? Code { get; set; }

    [Required(ErrorMessage = "Количество обязательно")]
    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
    public int Quantity { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }

    public MaterialStatus StatusM { get; set; } = MaterialStatus.New;

    [MaxLength(100)]
    public string? ManufacturerPartNumber { get; set; }

    [MaxLength(150)]
    public string? ManufacturerName { get; set; }

    [MaxLength(20)]
    public string? UnitOfMeasure { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Цена не может быть отрицательной")] 
    public int EstimatedPrice { get; set; }

    [NotMapped]
    public int TotalPrice => Quantity * EstimatedPrice; 

    public void FillFromCatalog(ProposalCatalog catalog)
    {
        if (catalog != null)
        {
            NameMaterial = catalog.Material;
            CategoryMaterial = catalog.Category;
            ManufacturerPartNumber = catalog.ManufacturerPartNumber;
            ManufacturerName = catalog.ManufacturerName;
            UnitOfMeasure = catalog.UnitOfMeasure;
            EstimatedPrice = catalog.Price;

            Code = catalog.ManufacturerPartNumber ?? $"CAT-{catalog.ID}";
        }
    }

    public int ProposalId { get; set; }
    public virtual Proposal Proposal { get; set; } = null!;

    public int? CatalogId { get; set; }
    public virtual ProposalCatalog? Catalog { get; set; }
}