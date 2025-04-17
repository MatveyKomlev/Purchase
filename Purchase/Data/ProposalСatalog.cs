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
        [Required]
        public string? Material { get; set; }
        [Required]
        public string? Category { get; set; }

        
    }
}
//public int ProposalId { get; set; } 
//public Proposal? Proposal { get; set; }