using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Purchase.Data
{
    public class Proposal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "№")]
        public string ID { get; set; }

        [Required]
        [MaxLength(10)]
        public string Number { get; set; }

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
        public string Department { get; set; }

        [Required]
        [MaxLength(60)]
        public string Author { get; set; }
    }
}
