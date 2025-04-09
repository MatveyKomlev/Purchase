using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Purchase.Data
{
    public class ProposalCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "#")]
        public int ID { get; set; }
        public string? Status { get; set; }
        public string? Title { get; set; }
        public string? Material { get; set; }
        public int ProposalId { get; set; } // Связь с Proposal
        public Proposal? Proposal { get; set; }
    }
}
