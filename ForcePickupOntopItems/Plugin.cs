using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace ForcePickupOntopItems
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Dinkum.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin instance;

        private readonly Harmony _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        private ConfigEntry<KeyCode> _configKey;

        internal bool Enabled = false;

        internal void Awake()
        {
            instance = this;

            _configKey = Config.Bind("General", "Enable Key", KeyCode.F10, "This key will enable/disable the force pickup of ontop items.");
            _harmony.PatchAll();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} loaded!");
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
            if (Input.GetKeyDown(_configKey.Value))
            {
                Enabled = !Enabled;
                NotificationManager.manage.createChatNotification("Force pickup of ontop items is now " + (Enabled ? "enabled" : "disabled") + ".", false);
            }
        }
    }
}
