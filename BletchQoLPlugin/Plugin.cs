using BepInEx;
using BepInEx.Configuration;
using System.Linq;
using UnityEngine;

namespace BletchQoLPlugin
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Dinkum.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private ConfigEntry<KeyCode> ConfigKey;
        private bool Enabled;

        internal void Awake()
        {
            ConfigKey = Config.Bind("General", "Toggle Key", KeyCode.F10, "This key will toggle the bug and fish display on or off.");

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} loaded!");
        }

        internal void Update()
        {
            if (Input.GetKeyDown(ConfigKey.Value))
            {
                Enabled = !Enabled;
                NotificationManager.manage.createChatNotification("Fish and Bug display is now " + (Enabled ? "enabled" : "disabled") + ".", false);

                if (Enabled)
                {
                    var invSlots = Inventory.inv.invSlots;
                    AnimalManager.manage.bugBookOpen = invSlots?.Any(i => i.itemNo == ItemIds.BugBook) ?? false;
                    AnimalManager.manage.fishBookOpen = invSlots?.Any(i => i.itemNo == ItemIds.FishBook) ?? false;
                }
                else
                {
                    AnimalManager.manage.bugBookOpen = false;
                    AnimalManager.manage.fishBookOpen = false;
                }
            }
            else
            {
                // if the book is closed, but we are active, then show
                AnimalManager.manage.bugBookOpen |= Enabled;
                AnimalManager.manage.fishBookOpen |= Enabled;
            }

            try
            {
                AnimalManager.manage.lookAtBugBook.Invoke();
            }
            catch { }

            try
            {
                AnimalManager.manage.lookAtFishBook.Invoke();
            }
            catch { }
        }
    }
}
