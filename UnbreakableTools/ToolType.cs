using System.ComponentModel;

namespace UnbreakableTools
{
    internal enum ToolType
    {
        [Description("none")]
        None,
        [Description("hand tools only")]
        HandTools,
        [Description("power tools only")]
        PowerTools,
        [Description("hand and power tools")]
        Both,
    }
}
