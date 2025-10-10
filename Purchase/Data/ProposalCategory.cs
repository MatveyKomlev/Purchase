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

        [Required(ErrorMessage = "Материал обязателен")]
        public string? Material { get; set; }

        [Required(ErrorMessage = "Категория обязательна")]
        public string? Category { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public decimal Quantity { get; set; }

        public int ProposalId { get; set; }


    }
}
