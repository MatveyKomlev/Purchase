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
        [Required]
        public int Number { get; set; }
        [Required]
        public DateTime DateCreation { get; set; }
        [Required]
        [MaxLength(30)]
        public string? Author { get; set; }
        [Required]
        [MaxLength(20)]
        public string? Department { get; set; }
        [Required]
        [MaxLength(10)]
        public string? Status { get; set; }
        public List<ProposalMaterial>? Materials { get; set; }
        public List<ProposalCatalog>? Categories { get; set; }
    }


}


