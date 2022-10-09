using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace InventoryTooltips
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Dinkum.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin instance;

        internal static readonly bool AlreadyPatched = Harmony.GetPatchInfo(AccessTools.Method(typeof(Inventory), "fillHoverDescription", null, null)) != null;

        private readonly Harmony _harmony = new(PluginInfo.PLUGIN_GUID);

        private ConfigEntry<KeyCode> _configKey;
        private ConfigEntry<bool> _enabled;
        private ConfigEntry<KeyCode> _hotKey;
        private ConfigEntry<DisplayType> _displayType;

        internal void Awake()
        {
            instance = this;

            _configKey = Config.Bind("General", "Enable Key", KeyCode.F10, "This key will enable/disable the display of the tooltip information. Disable with KeyCode.None.");
            _enabled = Config.Bind("General", "Enabled", true, "Tooltip information enabled.");
            _hotKey = Config.Bind("General", "HotKey", KeyCode.LeftControl, "The key that will toggle display of stack price. Disable with KeyCode.None.");
            _displayType = Config.Bind("General", "Display Type", DisplayType.Item, "How the price information will be calculated in the tooltip.");

            _harmony.PatchAll(typeof(InventoryTooltipsPatches));

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        internal bool GetEnabled()
        {
            return _enabled.Value;
        }

        internal DisplayType GetDisplayType()
        {
            return _displayType.Value;
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

                if (_enabled.Value)
                    NotificationManager.manage.createChatNotification($"Tooltip information is now enabled and showing prices for the {GetDisplayTypeDescription(_displayType.Value)}.", false);
                else
                    NotificationManager.manage.createChatNotification($"Tooltip information is now disabled.", false);
            }

            if (Input.GetKeyDown(_hotKey.Value) && Inventory.inv.invOpen && _enabled.Value)
            {
                var newValue = (int)_displayType.Value + 1;
                if (!Enum.IsDefined(typeof(DisplayType), newValue))
                    newValue = 0;
                _displayType.Value = (DisplayType)Enum.Parse(typeof(DisplayType), newValue.ToString());
                Config.Save();

                NotificationManager.manage.createChatNotification($"Tooltip information is showing prices for the {GetDisplayTypeDescription(_displayType.Value)}.", false);
            }
        }

        internal string GetDisplayTypeDescription(DisplayType displayType)
        {
            var memberData = typeof(DisplayType).GetMember(displayType.ToString());

            return (memberData[0].GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute).Description ?? displayType.ToString();
        }
    }
}
