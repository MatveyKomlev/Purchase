using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Purchase.Data
{
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

        // Связь с Proposal
        public int ProposalId { get; set; }
        public Proposal Proposal { get; set; }
    }
}
