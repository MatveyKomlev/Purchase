using Purchase.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProposalMaterial
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "#")]
    public int ID { get; set; }

    [Required]
    public string? NameMaterial { get; set; }

    [Required]
    public string? CategoryMaterial { get; set; }

    [MaxLength(10)]
    [Required]
    public string? Code { get; set; }

    public int Quantity { get; set; }

    [Required]
    public string? Comment { get; set; }

    [Required]
    public string? StatusM { get; set; }

    // Новые поля
    [MaxLength(50)]
    public string? ManufacturerPartNumber { get; set; }

    [MaxLength(100)]
    public string? ManufacturerName { get; set; }

    [MaxLength(20)]
    public string? UnitOfMeasure { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal EstimatedPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice => Quantity * EstimatedPrice;

    // Связь с Proposal
    public int ProposalId { get; set; }
    public Proposal Proposal { get; set; } = null!;

    public int? CatalogId { get; set; }
    public ProposalCatalog? Catalog { get; set; }
}