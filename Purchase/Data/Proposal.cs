using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Purchase.Data
{
    public class Proposal
    {
        [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "#")]
    public int ID { get; set; }

    [Required(ErrorMessage = "Номер обязателен")]
    [StringLength(100)]
    public int Number { get; set; }

    [Required]
    public DateTime DateCreation { get; set; }

    [MaxLength(30)]
    public string? Author { get; set; }

    [MaxLength(20)]
    public string? Department { get; set; }

    [MaxLength(10)]
    [Required(ErrorMessage = "Статус обязателен")]
    public ErpStatus Status { get; set; }

    public DateTime? Deadline { get; set; }
    
    [MaxLength(500)]
    public string? Explanation { get; set; } //Пояснение закупки
    
    [Required]
    public Priorities Priority { get; set; } = Priorities.Medium; 

    public int PositionsCount => Materials?.Count ?? 0;

    // Навигационные свойства
    public List<ProposalMaterial> Materials { get; set; } = new();
    }
}


