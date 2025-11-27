using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string? Number { get; set; }

        [Required]
        public DateTime DateCreation { get; set; }

        [MaxLength(100)]
        public string? Author { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        [Required(ErrorMessage = "Статус обязателен")]
        public ErpStatus Status { get; set; }

        public DateTime? Deadline { get; set; }

        [MaxLength(1000)]
        public string? Explanation { get; set; }

        [Required]
        public Priorities Priority { get; set; } = Priorities.Medium;

        [NotMapped]
        public int PositionsCount => Materials?.Count ?? 0;

        public virtual List<ProposalMaterial> Materials { get; set; } = new();
    }
}