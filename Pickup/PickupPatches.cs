using HarmonyLib;
using Mirror;
using System;
using UnityEngine;

namespace Pickup
{
    internal class PickupPatches
    {
        private static WeakReference<CharInteract>[] charInteractRefs = new WeakReference<CharInteract>[0];
        private static DateTime nextUpdateTime = DateTime.MinValue;

        [HarmonyPatch(typeof(TileObject), "canBePickedUp")]
        [HarmonyPostfix]
        private static void canBePickedUpPatch(ref TileObject __instance, ref bool __result)
        {
            if (Plugin.instance.GetLocked())
                __result = false;
        }

        [HarmonyPatch(typeof(CharPickUp), "CmdPickUpObject")]
        [HarmonyPostfix]
        private static void CmdPickUpObjectPatch(CharPickUp __instance, uint pickUpObject)
        {
            if (!Plugin.instance.GetEnabled())
                return;

            var networkIdentity = NetworkIdentity.spawned[pickUpObject];
            if (networkIdentity == null)
                return;

            var component = networkIdentity.gameObject.GetComponent<SellByWeight>();
            if (component == null)
                return;

            var weight = Math.Round(component.getMyWeight(), 2, MidpointRounding.AwayFromZero);
            var price = component.getPrice();
            var message = $"{component.itemName}: {weight}kg, <sprite=11>{price:N0}";

            NotificationManager.manage.createChatNotification(message, false);
        }

        [HarmonyPatch(typeof(NotificationManager), "hintWindowOpen")]
        [HarmonyPostfix]
        private static void hintWindowOpenPatch(ref NotificationManager __instance, ref NotificationManager.toolTipType toolTip)
        {
            if (!Plugin.instance.GetEnabled())
                return;

            if (toolTip != NotificationManager.toolTipType.PickUp || DateTime.UtcNow < nextUpdateTime)
                return;

            nextUpdateTime = DateTime.UtcNow.Add(TimeSpan.FromMilliseconds(250.0));

            var localPlayerTargetedItemName = GetLocalPlayerTargetedItemName();
            if (localPlayerTargetedItemName is null)
                return;

            __instance.toolTipHintBox("null", "Pick Up " + UIAnimationManager.manage.getItemColorTag(localPlayerTargetedItemName ?? "<null>"), "null", "null");
        }

        private static string GetLocalPlayerTargetedItemName()
        {
            string text = null;
            for (int i = 0; i < charInteractRefs.Length; i++)
            {
                CharInteract charInteract;
                if (charInteractRefs[i].TryGetTarget(out charInteract))
                {
                    text = GetTargetedItemName(charInteract);
                    if (text != null)
                    {
                        if (i != 0)
                        {
                            WeakReference<CharInteract> weakReference = charInteractRefs[0];
                            charInteractRefs[0] = charInteractRefs[i];
                            charInteractRefs[i] = weakReference;
                            break;
                        }
                        break;
                    }
                }
            }
            if (text == null)
            {
                CharInteract[] array = UnityEngine.Object.FindObjectsOfType<CharInteract>(true) ?? new CharInteract[0];
                charInteractRefs = new WeakReference<CharInteract>[array.Length];
                for (int j = 0; j < array.Length; j++)
                {
                    charInteractRefs[j] = new WeakReference<CharInteract>(array[j]);
                }
                int k = 0;
                while (k < array.Length)
                {
                    text = GetTargetedItemName(array[k]);
                    if (text != null)
                    {
                        if (k != 0)
                        {
                            WeakReference<CharInteract> weakReference2 = charInteractRefs[0];
                            charInteractRefs[0] = charInteractRefs[k];
                            charInteractRefs[k] = weakReference2;
                            break;
                        }
                        break;
                    }
                    else
                    {
                        k++;
                    }
                }
            }
            return text;
        }

        private static string GetTargetedItemName(CharInteract charInteract)
        {
            return (charInteract != null && charInteract.isLocalPlayer && charInteract.objectAttacking && charInteract.objectAttacking.canBePickedUp())
                ? GetItemName(charInteract)
                : null;
        }

        private static string GetItemName(CharInteract charInteract)
        {
            if (charInteract.isInside() && charInteract.insideHouseDetails == null)
            {
                return null;
            }

            int pickUpItemIDForTile = GetPickUpItemIDForTile(charInteract.selectedTile, charInteract);
            if (pickUpItemIDForTile >= 0 && pickUpItemIDForTile < Inventory.inv.allItems.Length)
            {
                return Inventory.inv.allItems[pickUpItemIDForTile].getInvItemName();
            }

            return null;
        }

        private static int GetPickUpItemIDForTile(Vector2 tilePos, CharInteract charInteract)
        {
            int num = (int)tilePos.x;
            int num2 = (int)tilePos.y;

            HouseDetails insideHouseDetails = charInteract.insideHouseDetails;
            int[,] array = ((insideHouseDetails != null) ? insideHouseDetails.houseMapOnTile : null) ?? WorldManager.manageWorld.onTileMap;

            HouseDetails insideHouseDetails2 = charInteract.insideHouseDetails;
            int[,] array2 = ((insideHouseDetails2 != null) ? insideHouseDetails2.houseMapOnTileStatus : null) ?? WorldManager.manageWorld.onTileStatusMap;

            if (array.GetLength(0) <= num || array.GetLength(1) <= num2)
            {
                return -1;
            }

            int num3 = array[num, num2];
            int num4 = array2[num, num2];

            int onTopPos = charInteract.objectAttacking.returnClosestPlacedPositionId(charInteract.tileHighlighter.position);
            if (charInteract.objectAttacking.canBePlaceOn())
            {
                ItemOnTop itemOnTopInPosition = ItemOnTopManager.manage.getItemOnTopInPosition(onTopPos, num, num2, charInteract.insideHouseDetails);
                if (itemOnTopInPosition != null)
                {
                    num3 = itemOnTopInPosition.getTileObjectId();
                    num4 = itemOnTopInPosition.getStatus();
                }
            }

            if (WorldManager.manageWorld.allObjectSettings[num3].dropsStatusNumberOnDeath)
            {
                return num4;
            }

            if (num4 > 0 && WorldManager.manageWorld.allObjectSettings[num3].statusObjectsPickUpFirst.Length != 0)
            {
                return GetInventoryItemID(WorldManager.manageWorld.allObjectSettings[num3].statusObjectsPickUpFirst[num4]);
            }

            return GetInventoryItemID(WorldManager.manageWorld.allObjectSettings[num3].dropsItemOnDeath);
        }

        private static int GetInventoryItemID(InventoryItem forItem)
        {
            return (forItem is null)
                ? -1
                : forItem.getItemId();
        }
    }
}
