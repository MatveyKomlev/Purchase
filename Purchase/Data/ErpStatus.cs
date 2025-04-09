using System;
using System.ComponentModel;

namespace Purchase.Data
{
    public enum ErpStatus : long
    {
        [Description("В процессе")]
        InProgress = 0,

        [Description("Отклонен")]
        Rejected = 1,

        [Description("Одобрен")]
        Approved = 2
    }

    public static class ErpStatusExtensions
    {
        // Метод для получения локализованного имени статуса
        public static string GetName(this ErpStatus status)
        {
            var field = typeof(ErpStatus).GetField(status.ToString());
            if (field != null)
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                    return attribute.Description;
            }

            return status.ToString(); // Если описание не найдено, возвращаем строковое имя
        }
    }
}
