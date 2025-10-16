using System;
using System.ComponentModel;

namespace Purchase.Data
{
    public enum Priorities : long
    {
        [Description("Низкий")]
        Low = 0,

        [Description("Средний")]
        Medium = 1,

        [Description("Высокий")]
        High = 2,

        [Description("Критический")]
        Critical = 3
    }

    public static class PriorityExtensions
    {
        public static string GetName(this Priorities priority)
        {
            var field = typeof(Priorities).GetField(priority.ToString());
            if (field != null)
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                    return attribute.Description;
            }

            return priority.ToString();
        }
    }
}