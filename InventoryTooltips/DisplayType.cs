using System.ComponentModel;

namespace InventoryTooltips
{
    internal enum DisplayType
    {
        [Description("without Commerce Licence Level adjustment")]
        WithoutLicence = 0,
        [Description("with Commerce Licence Level adjustment")]
        WithLicence,
    }
}
