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

        [Required]
        [MaxLength(10)]
        public int ApplicationNum { get; set; }

        [Required]
        [MaxLength(60)]
        public DateTime DateCreation { get; set; }

        [Required]
        [MaxLength(60)]
        public string FullNumber { get; set; }

        [Required]
        [MaxLength(60)]
        public string Status { get; set; }

        [Required]
        [MaxLength(60)]
        public string TextStatus { get; set; }

        [Required]
        [MaxLength(60)]
        public string Division { get; set; }

        [Required]
        [MaxLength(60)]
        public string Author { get; set; }
    }
}
