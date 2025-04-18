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
        public string? NameMaterial { get; set; }
        public string? CategoryMaterial { get; set; } // будет пока вводимым полем, после можно будет сделать чтобы определялось само в зависимости от материала

        [MaxLength(10)]
        public string? Code { get; set; }
        public int Quantity { get; set; }
        public string? Comment { get; set; }

        public int ProposalId { get; set; } 
        public Proposal? Proposal { get; set; }
    }
}
