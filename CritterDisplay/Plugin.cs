using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CritterDisplay
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Dinkum.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin instance;
        internal static List<AnimalAI> animalAIs = new List<AnimalAI>();

        private readonly Harmony _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        private ConfigEntry<KeyCode> _configKey;
        private ConfigEntry<BookType> _bookType;

        internal void Awake()
        {
            instance = this;

            _configKey = Config.Bind("General", "Enable Key", KeyCode.F10, "This key will enable/disable the display of the critter information.");
            _bookType = Config.Bind("General", "Book Type", BookType.None, "Which book will show the critter information when opened.");

            _harmony.PatchAll(typeof(CritterDisplayPatches));

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

        internal void OnGUI()
        {
            if (_bookType.Value == BookType.None)
                return;

            if (!NetworkMapSharer.share.localChar)
            {
                animalAIs.Clear();
            }

            bool bookOpen = _bookType.Value == BookType.Fish ? AnimalManager.manage.fishBookOpen : AnimalManager.manage.bugBookOpen;
            bool showInformation = !(Inventory.inv.isMenuOpen() || !NetworkMapSharer.share.localChar || !bookOpen || animalAIs.Count <= 0);
            if (showInformation)
            {
                foreach (var animalAI in animalAIs)
                {
                    var component = animalAI.GetComponent<BugTypes>();
                    if (component && component.isUnderwaterCreature)
                    {
                        var vector = Camera.main.WorldToScreenPoint(animalAI.transform.position);
                        if (vector.z > 0f)
                        {
                            string text;
                            if (PediaManager.manage.isInPedia(component.getBugTypeId()))
                            {
                                text = $"${Inventory.inv.allItems[component.getBugTypeId()].value}\n{Inventory.inv.allItems[component.getBugTypeId()].getInvItemName()}";
                            }
                            else
                            {
                                text = "$????\n?????";
                            }
                            var content = new GUIContent(text);

                            var box = GUI.skin.box;
                            box.fontSize = 13;
                            box.fontStyle = FontStyle.Bold;
                            box.alignment = TextAnchor.MiddleCenter;

                            GUI.backgroundColor = Color.grey;
                            GUI.color = Color.white;

                            var boxSize = box.CalcSize(content);
                            var boxPos = new Vector2(vector.x, Screen.height - vector.y);
                            GUI.Box(new Rect(boxPos.x, boxPos.y, boxSize.x, boxSize.y), text, box);
                        }
                    }
                }
            }
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

                if (_bookType.Value == BookType.None)
                    NotificationManager.manage.createChatNotification($"Critter information is now disabled.", false);
                else
                    NotificationManager.manage.createChatNotification($"Critter information is shown when {_bookType.Value} book is open.", false);
            }
        }
    }
}
