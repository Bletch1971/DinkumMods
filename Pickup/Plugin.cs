using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace Pickup
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Dinkum.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin instance;

        private readonly Harmony _harmony = new(PluginInfo.PLUGIN_GUID);

        private ConfigEntry<KeyCode> _configKey;
        private ConfigEntry<bool> _enabled;
        private ConfigEntry<KeyCode> _hotKey;
        private ConfigEntry<bool> _locked;

        internal void Awake()
        {
            instance = this;

            _configKey = Config.Bind("General", "Enable Key", KeyCode.F10, "This key will enable/disable the display of the pickup information. Disable with KeyCode.None.");
            _enabled = Config.Bind("General", "Enabled", true, "Pickup information enabled.");
            _hotKey = Config.Bind("General", "HotKey", KeyCode.L, "This key will enable/disable the locking of items so they cannot be picked up. Disable with KeyCode.None.");
            _locked = Config.Bind("General", "Locked", false, "Default status of the pickup action.");

            _harmony.PatchAll(typeof(PickupPatches));

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        internal bool GetEnabled()
        {
            return _enabled.Value;
        }

        internal bool GetLocked()
        {
            return _locked.Value;
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
                _enabled.Value = !_enabled.Value;
                Config.Save();

                NotificationManager.manage.createChatNotification($"Pickup information is now {(_enabled.Value ? "enabled" : "disabled")}.", false);
            }

            if (Input.GetKeyDown(_hotKey.Value))
            {
                _locked.Value = !_locked.Value;
                Config.Save();

                NotificationManager.manage.createChatNotification($"Pickup status is now {(_locked.Value ? "locked" : "unlocked")}.", false);
            }
        }
    }
}
