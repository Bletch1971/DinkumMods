using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Mirror;
using System.Collections.Generic;
using System.Linq;
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
        private ConfigEntry<KeyCode> _lockedHotKey;
        private ConfigEntry<bool> _locked;
        private ConfigEntry<KeyCode> _autoPickupHotKey;
        private ConfigEntry<bool> _autoPickup;

        private float _distance = 8f; // The distance that will auto pickup dropped items.
        private float _refreshTime = 1f; // The time that will refresh the dropped items and try to pickup them.
        private float _timer = 0f;

        internal void Awake()
        {
            instance = this;

            _configKey = Config.Bind("General", "Enable Key", KeyCode.F10, "This key will enable/disable the display of the pickup information. Disable with KeyCode.None.");
            _enabled = Config.Bind("General", "Enabled", true, "Pickup information enabled.");
            _lockedHotKey = Config.Bind("General", "Locked HotKey", KeyCode.L, "This key will enable/disable the locking of items so they cannot be picked up. Disable with KeyCode.None.");
            _locked = Config.Bind("General", "Locked", false, "Default status of the pickup action.");
            _autoPickupHotKey = Config.Bind("General", "Auto-Pickup HotKey", KeyCode.P, "This key will enable/disable the automatic pickup of items. Disable with KeyCode.None.");
            _autoPickup = Config.Bind("General", "Auto-Pickup", false, "Default status of auto pickup.");

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

        internal bool GetAutoPickup()
        {
            return _autoPickup.Value;
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

            if (Input.GetKeyDown(_lockedHotKey.Value))
            {
                _locked.Value = !_locked.Value;
                Config.Save();

                NotificationManager.manage.createChatNotification($"Pickup status is now {(_locked.Value ? "locked" : "unlocked")}.", false);
            }

            if (Input.GetKeyDown(_autoPickupHotKey.Value))
            {
                _autoPickup.Value = !_autoPickup.Value;
                Config.Save();

                NotificationManager.manage.createChatNotification($"Auto-Pickup is now {(_locked.Value ? "enabled" : "disabled")}.", false);
            }

            if (_autoPickup.Value)
            {
                _timer += Time.deltaTime;
                if (_timer >= _refreshTime)
                {
                    if (NetworkMapSharer.share.isServer)
                    {
                        using (IEnumerator<DroppedItem> enumerator = Enumerable.Where(Enumerable.ToList(WorldManager.manageWorld.itemsOnGround), (DroppedItem item) => Vector3.Distance(NetworkMapSharer.share.localChar.transform.position, item.transform.position) <= _distance).GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                var droppedItem = enumerator.Current;
                                if (Inventory.inv.addItemToInventory(droppedItem.myItemId, droppedItem.stackAmount, true))
                                {
                                    SoundManager.manage.play2DSound(SoundManager.manage.pickUpItem);
                                    droppedItem.pickUpLocal();
                                    droppedItem.pickUp();
                                    NetworkServer.UnSpawn(droppedItem.gameObject);
                                    NetworkServer.Destroy(droppedItem.gameObject);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var droppedItem in Enumerable.Where(FindObjectsOfType<DroppedItem>(), (DroppedItem item) => Vector3.Distance(NetworkMapSharer.share.localChar.transform.position, item.transform.position) <= _distance))
                        {
                            if (Inventory.inv.addItemToInventory(droppedItem.myItemId, droppedItem.stackAmount, true))
                            {
                                SoundManager.manage.play2DSound(SoundManager.manage.pickUpItem);
                                droppedItem.pickUpLocal();
                                droppedItem.pickUp();
                                NetworkServer.UnSpawn(droppedItem.gameObject);
                                NetworkServer.Destroy(droppedItem.gameObject);
                            }
                        }
                    }

                    _timer = 0f;
                }
            }
        }
    }
}
