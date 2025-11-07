using System.ComponentModel;

namespace Purchase.Data
{
    public enum MaterialStatus
    {
        [Description("Новая")]
        New = 0,

        [Description("Есть в базе")]
        InCatalog = 1,

        [Description("Закрыта")]
        Closed = 2
    }

    public static class MaterialStatusExtensions
    {
        public static string GetName(this MaterialStatus status)
        {
            var field = typeof(MaterialStatus).GetField(status.ToString());
            if (field != null)
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                    return attribute.Description;
            }
            return status.ToString();
        }
    }
}
