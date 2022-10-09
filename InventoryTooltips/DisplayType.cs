using System.ComponentModel;

namespace InventoryTooltips
{
    internal enum DisplayType
    {
        [Description("Item")]
        Item = 0,
        [Description("Stack")]
        Stack = 1,
        [Description("Item with Commerce Licence Level")]
        ItemWithLicence = 2,
        [Description("Stack with Commerce Licence Level")]
        StackWithLicence = 3,
    }
}
