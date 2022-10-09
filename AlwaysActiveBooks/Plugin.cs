using BepInEx;
using BepInEx.Configuration;
using System;
using System.Linq;
using UnityEngine;

namespace AlwaysActiveBooks
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Dinkum.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin instance;

        private ConfigEntry<KeyCode> _configKey;
        private ConfigEntry<BookType> _bookType;

        internal void Awake()
        {
            instance = this;

            _configKey = Config.Bind("General", "Enable Key", KeyCode.F10, "This key will enable/disable the display of the critter information.");
            _bookType = Config.Bind("General", "Book Type", BookType.Manual, "Which books will be always active.");

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        internal BookType GetBookType()
        {
            return _bookType.Value;
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
                var newValue = (int)_bookType.Value + 1;
                if (!Enum.IsDefined(typeof(BookType), newValue))
                    newValue = 0;

                _bookType.Value = (BookType)Enum.Parse(typeof(BookType), newValue.ToString());
                Config.Save();

                switch (_bookType.Value)
                {
                    case BookType.Manual:
                        NotificationManager.manage.createChatNotification($"Bug and Fish books are now set to manual.", false);
                        break;
                    case BookType.Both:
                        NotificationManager.manage.createChatNotification($"Bug and Fish books are now set to always active.", false);
                        break;
                    default:
                        NotificationManager.manage.createChatNotification($"{_bookType.Value} book is now set to always active.", false);
                        break;
                }


                var openBugBook = _bookType.Value is BookType.Bug or BookType.Both && Inventory.inv.invSlots.Any(s => s.itemNo == 679);
                var openFishBook = _bookType.Value is BookType.Fish or BookType.Both && Inventory.inv.invSlots.Any(s => s.itemNo == 680);

                AnimalManager.manage.bugBookOpen = openBugBook;
                AnimalManager.manage.fishBookOpen = openFishBook;
            }
            else
            {
                var openBugBook = _bookType.Value is BookType.Bug or BookType.Both && Inventory.inv.invSlots.Any(s => s.itemNo == 679);
                var openFishBook = _bookType.Value is BookType.Fish or BookType.Both && Inventory.inv.invSlots.Any(s => s.itemNo == 680);

                AnimalManager.manage.bugBookOpen |= openBugBook;
                AnimalManager.manage.fishBookOpen |= openFishBook;
            }

            try
            {
                AnimalManager.manage.lookAtBugBook.Invoke();
                AnimalManager.manage.lookAtFishBook.Invoke();
            }
            catch {}
        }
    }
}
