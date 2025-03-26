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
        public string ?Category { get; set; }

        [Required]
        [MaxLength(30)]
        /// <summary>
        /// Автор
        /// </summary>
        public string ?Author { get; set; }

        [Required]
        [MaxLength(20)]
        /// <summary>
        /// Отдел
        /// </summary>
        public string ?Department { get; set; }

        [Required]
        [MaxLength(10)]
        /// <summary>
        /// Статус заказа
        /// </summary>
        public string ?Status { get; set; }
    }


}


public enum StatusText : long
{
    [Description("В процессе")]
    InProgress,

    [Description("Отклонен")]
    Canceled,

    [Description("Одобрен")]
    Completed 


}

// Enum для отделов
public enum Department : long
{
    [Description("Отдел A")]
    Department_A,
    [Description("Отдел B")]
    Department_B,
    [Description("Отдел C")]
    Department_C
}

// Enum для категорий товара
public enum Category : long
{
    [Description("Резисторы")]
    Category_1,
    [Description("Конденсаторы")]
    Category_2,
    [Description("Транзисторы")]
    Category_3,
    [Description("Микросхемы")]
    Category_4,
    [Description("Катушки индуктивности")]
    Category_5
}

// Расширение для статуса
public static class StatementsExtensions
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}

