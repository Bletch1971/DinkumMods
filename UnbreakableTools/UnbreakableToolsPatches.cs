using HarmonyLib;
using System;
using System.Collections.Generic;

namespace UnbreakableTools
{
    internal class UnbreakableToolsPatches
    {
        [HarmonyPatch(typeof(Inventory), "useItemWithFuel")]
        [HarmonyPrefix]
        private static bool useItemWithFuelPatch()
        {
            var toolType = Plugin.instance.GetToolType();
            var inventorySlot = Inventory.inv.invSlots[Inventory.inv.selectedSlot];
            var inventoryItem = Inventory.inv.allItems[inventorySlot.itemNo];

            if (inventorySlot.itemNo >= 0 && inventorySlot.stack == 1 && inventoryItem.hasFuel && inventoryItem.isATool)
            {
                if (inventoryItem.isRepairable && toolType is ToolType.HandTools or ToolType.Both)
                {
                    return false;
                }
                if (inventoryItem.isPowerTool && toolType is ToolType.PowerTools or ToolType.Both)
                {
                    return false;
                }
            }

            return true;
        }

        [HarmonyPatch(typeof(CharInteract), "checkIfCanDamage")]
        [HarmonyPrefix]
        private static bool checkIfCanDamagePatch()
        {
            var toolType = Plugin.instance.GetToolType();
            var inventorySlot = Inventory.inv.invSlots[Inventory.inv.selectedSlot];
            var inventoryItem = Inventory.inv.allItems[inventorySlot.itemNo];

            if (inventorySlot.itemNo >= 0 && inventorySlot.stack == 1 && inventoryItem.hasFuel && inventoryItem.isATool)
            {
                if (inventoryItem.isRepairable && toolType is ToolType.HandTools or ToolType.Both)
                {
                    return false;
                }
                if (inventoryItem.isPowerTool && toolType is ToolType.PowerTools or ToolType.Both)
                {
                    return false;
                }
            }

            return true;
        }

        [HarmonyPatch(typeof(MeleeAttacks), "attackAndDealDamage")]
        [HarmonyPrefix]
        private static bool attackAndDealDamagePatch()
        {
            var toolType = Plugin.instance.GetToolType();
            var inventorySlot = Inventory.inv.invSlots[Inventory.inv.selectedSlot];
            var inventoryItem = Inventory.inv.allItems[inventorySlot.itemNo];

            if (inventorySlot.itemNo >= 0 && inventorySlot.stack == 1 && inventoryItem.hasFuel && inventoryItem.isATool)
            {
                if (inventoryItem.isRepairable && toolType is ToolType.HandTools or ToolType.Both)
                {
                    NotificationManager.manage.createChatNotification(inventoryItem.itemName + " is broken. You need to repair it to keep using it.", false);
                    return false;
                }
                if (inventoryItem.isPowerTool && toolType is ToolType.PowerTools or ToolType.Both)
                {
                    NotificationManager.manage.createChatNotification(inventoryItem.itemName + " is out of power. You need to charge it to keep using it.", false);
                    return false;
                }
            }

            return true;
        }
    }
}
