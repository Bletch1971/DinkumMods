using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.ComponentModel;
using System.Linq;

namespace UnbreakableTools
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Dinkum.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin instance;

        private readonly Harmony _harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        private ConfigEntry<ToolType> _toolType;

        private bool _notificationSent = false;

        internal void Awake()
        {
            instance = this;

            _toolType = Config.Bind("General", "Tool Type", ToolType.None, "Which type of tools should be unbreakable.");

            if (_toolType.Value != ToolType.None)
                _harmony.PatchAll(typeof(UnbreakableToolsPatches));

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        internal ToolType GetToolType()
        {
            return _toolType.Value;
        }

        internal static void LogDebug(string message)
        {
            instance.Logger.LogDebug(message);
        }

        internal static void LogInfo(string message)
        {
            instance.Logger.LogInfo(message);
        }

        internal void Update()
        {
            if (NetworkMapSharer.share.localChar == null || _notificationSent)
                return;

            NotificationManager.manage.createChatNotification($"Unbreakable Tools set to {GetToolTypeDescription(_toolType.Value)}.", false);
            _notificationSent = true;
        }

        internal string GetToolTypeDescription(ToolType toolType)
        {
            var memberData = typeof(ToolType).GetMember(toolType.ToString());

            return (memberData[0].GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute).Description ?? toolType.ToString();
        }
    }
}
