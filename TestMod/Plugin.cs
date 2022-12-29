using BepInEx;
using BepInEx.Configuration;
using System;
using UnityEngine;
using static TownManager;

namespace TestMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin instance;

        private ConfigEntry<KeyCode> _configKey;

        internal void Awake()
        {
            instance = this;

            _configKey = Config.Bind("General", "Process Key", KeyCode.F10, "This key will perform the mod process.");

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
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
                //OutputAllObjects();
                //OutputAllObjectSettings();
                //OutputAllNpc();
                TownBeautyLevel();
            }
        }

        private void OutputAllObjects()
        {
            Logger.LogInfo("WorldManager.manageWorld.allObjects");
            Logger.LogInfo($"Total: {WorldManager.manageWorld.allObjects.Length}");
            Logger.LogInfo("tileObjectId, name, canBePickedUp, canBePlaceOn, canBePlacedOntoFurniture, beautyType, beautyToAdd");

            foreach (var item in WorldManager.manageWorld.allObjects)
            {
                var setting = WorldManager.manageWorld.allObjectSettings[item.tileObjectId];
                Logger.LogInfo($"{item.tileObjectId}, {item.name}, {item.canBePickedUp()}, {item.canBePlaceOn()}, {item.canBePlacedOntoFurniture()}, {setting.beautyType}, {setting.beautyToAdd}");
            }
        }

        private void OutputAllObjectSettings()
        {
            Logger.LogInfo("WorldManager.manageWorld.allObjectSettings");
            Logger.LogInfo($"Total: {WorldManager.manageWorld.allObjectSettings.Length}");
            Logger.LogInfo("tileObjectId, name, canBePickedUp, canBePlacedOn, canBePlacedOnTopOfFurniture, beautyType, beautyToAdd");

            foreach (var setting in WorldManager.manageWorld.allObjectSettings)
            {
                Logger.LogInfo($"{setting.tileObjectId}, {setting.name}, {setting.canBePickedUp}, {setting.canBePlacedOn()}, {setting.canBePlacedOnTopOfFurniture}, {setting.beautyType}, {setting.beautyToAdd}");
            }
        }

        private void OutputAllNpc()
        {
            Logger.LogInfo("NPCManager.manage.NPCDetails");
            Logger.LogInfo($"Total: {NPCManager.manage.NPCDetails.Length}");
            Logger.LogInfo("i, name, NPCName");

            var i = 0;
            foreach (var details in NPCManager.manage.NPCDetails)
            {
                Logger.LogInfo($"{i}, {details.name}, {details.NPCName}");
                i++;
            }

            Logger.LogInfo("NPCManager.manage.npcStatus");
            Logger.LogInfo($"Total: {NPCManager.manage.npcStatus.Count}, Moved in: {NPCManager.manage.getNoOfNPCsMovedIn()}");
            Logger.LogInfo("i, hasMet, hasMovedIn, moneySpentAtStore, relationshipLevel, acceptedRequest, completedRequest, hasBeenTalkedToToday, hasGossipedToday, checkIfHasBeenGreeted, howManyDaysSincePlayerInteract");

            foreach (var status in NPCManager.manage.npcStatus)
            {
                Logger.LogInfo($"{i}, {status.hasMet}, {status.hasMovedIn}, {status.moneySpentAtStore}, {status.relationshipLevel}, {status.acceptedRequest}, {status.completedRequest}, {status.hasBeenTalkedToToday}, {status.hasGossipedToday}, {status.checkIfHasBeenGreeted(i)}, {status.howManyDaysSincePlayerInteract}");
                i++;
            }
        }

        private void TownBeautyLevel()
        {
            TownManager.manage.calculateTownScore();

            Logger.LogInfo($"townBeautyLevel: {TownManager.manage.townBeautyLevel}");
            for (int i = 0; i < TownManager.manage.beautyLevels.Length; i++)
            {
                Logger.LogInfo($"beautyLevel: {i}-{(TownBeautyType)i}-{TownManager.manage.beautyLevels[i]}");
            }
        }
    }
}
