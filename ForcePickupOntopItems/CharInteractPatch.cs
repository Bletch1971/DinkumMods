using HarmonyLib;

namespace ForcePickupOntopItems
{
    [HarmonyPatch(typeof(CharInteract), "pickUpTileObject")]
    internal class CharInteractPatch
    {
        [HarmonyPostfix]
        private static void pickUpTileObjectPatch(ref CharInteract __instance)
        {
            if (!Plugin.instance.Enabled)
                return;

            Plugin.LogInfo("Starting pickUpTileObject");

            if (!__instance.placingDeed && __instance.objectAttacking && __instance.objectAttacking.canBePickedUp())
            {
                var itemToPickup = __instance.objectAttacking;

                Plugin.LogDebug($"itemToPickup: {itemToPickup.tileObjectId}; {itemToPickup.name}; {itemToPickup.xPos}; {itemToPickup.yPos};");

                var houseDetails = __instance.insidePlayerHouse ? __instance.insideHouseDetails : null;

                if (itemToPickup.canBePlaceOn() && ItemOnTopManager.manage.hasItemsOnTop(itemToPickup.xPos, itemToPickup.yPos, houseDetails))
                {
                    var onTopItems = ItemOnTopManager.manage.getAllItemsOnTop(itemToPickup.xPos, itemToPickup.yPos, houseDetails);

                    foreach (var onTopItem in onTopItems)
                    {
                        Plugin.LogDebug($"onTopItem: {onTopItem.itemId}; {onTopItem.onTopPosition}; {onTopItem.houseX}; {onTopItem.houseY}; {onTopItem.sittingOnX}; {onTopItem.sittingOnY}");

                        ItemOnTop itemOnTopInPosition = ItemOnTopManager.manage.getItemOnTopInPosition(onTopItem.onTopPosition, itemToPickup.xPos, itemToPickup.yPos, houseDetails);
                        if (!WorldManager.manageWorld.allObjectSettings[itemOnTopInPosition.getTileObjectId()].pickUpRequiresEmptyPocket)
                        {
                            __instance.CmdPickUpObjectOnTopOfInside(onTopItem.onTopPosition, itemToPickup.xPos, itemToPickup.yPos);
                        }
                        else if (Inventory.inv.addItemToInventory(itemOnTopInPosition.getStatus(), 1, true))
                        {
                            __instance.CmdPickUpObjectOnTopOfInside(onTopItem.onTopPosition, itemToPickup.xPos, itemToPickup.yPos);
                        }
                        else
                        {
                            NotificationManager.manage.turnOnPocketsFullNotification(false);
                        }
                    }
                }
            }

            Plugin.LogInfo("Finishing pickUpTileObject");
        }
    }
}
