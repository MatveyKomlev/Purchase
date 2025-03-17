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
        /// <summary>
        /// Номер заявки
        /// </summary>
        public int Number { get; set; }

        [Required]
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime DateCreation { get; set; }

        [Required]
        [MaxLength(20)]
        /// <summary>
        /// Категория покупки
        /// </summary>
        public string Category { get; set; }

        [Required]
        [MaxLength(30)]
        /// <summary>
        /// Автор
        /// </summary>
        public string Author { get; set; }

        [Required]
        [MaxLength(20)]
        /// <summary>
        /// Отдел
        /// </summary>
        public string Department { get; set; }

        [Required]
        [MaxLength(10)]
        /// <summary>
        /// Статус заказа
        /// </summary>
        public string Status { get; set; }
    }
}
