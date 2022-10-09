using HarmonyLib;
using UnityEngine;

namespace InventoryTooltips
{
    internal class InventoryTooltipsPatches
    {
        [HarmonyPatch(typeof(Inventory), "fillHoverDescription")]
        [HarmonyPostfix]
        private static void fillHoverDescriptionPatch(ref Inventory __instance, InventorySlot rollOverSlot)
        {
            if (!Plugin.instance._enabled.Value)
                return;

            InventoryItem itemInSlot = rollOverSlot.itemInSlot;
            var itemTooltip = Plugin.AlreadyPatched
                ? __instance.InvDescriptionText.text
                : itemInSlot.getItemDescription(__instance.getInvItemId(itemInSlot));

            itemTooltip += GetBuyPrice(__instance, rollOverSlot);
            itemTooltip += GetSellPrice(__instance, rollOverSlot);
            itemTooltip += GetMuseumDonated(__instance, rollOverSlot);

            __instance.InvDescriptionText.text = itemTooltip;
        }

        private static string GetBuyPrice(Inventory inventory, InventorySlot slot)
        {
            var itemInSlot = slot.itemInSlot;
            var amount = itemInSlot.value * 2;

            if (Plugin.instance.DisplayType == DisplayType.Stack || Plugin.instance.DisplayType == DisplayType.StackWithLicence)
            {
                amount *= slot.stack;
            }

            return UIAnimationManager.manage.moneyAmountColorTag($"\n[Buy] <sprite=11>{amount:n0}");
        }

        private static string GetSellPrice(Inventory inventory, InventorySlot slot)
        {
            var itemInSlot = slot.itemInSlot;
            var amount = (float)itemInSlot.value;

            if (Plugin.instance.DisplayType == DisplayType.ItemWithLicence || Plugin.instance.DisplayType == DisplayType.StackWithLicence)
            {
                var level = (float)LicenceManager.manage.allLicences[8].getCurrentLevel();
                var percentage = 1 + ((float)level * 0.05f);
                amount = Mathf.RoundToInt(amount * percentage);
            }

            if (Plugin.instance.DisplayType == DisplayType.Stack || Plugin.instance.DisplayType == DisplayType.StackWithLicence)
            {
                amount *= slot.stack;
            }

            return UIAnimationManager.manage.moneyAmountColorTag($"\n[Sell] <sprite=11>{amount:n0}");
        }

        private static string GetMuseumDonated(Inventory inventory, InventorySlot slot)
        {
            var itemInSlot = slot.itemInSlot;

            if (itemInSlot.bug)
            {
                var index = MuseumManager.manage.allBugs.IndexOf(itemInSlot);
                if (MuseumManager.manage.bugsDonated[index])
                {
                    return "\n<sprite=17> Donated to Museum";
                }
            }
            else if (itemInSlot.fish)
            {
                var index = MuseumManager.manage.allFish.IndexOf(itemInSlot);
                if (MuseumManager.manage.fishDonated[index])
                {
                    return "\n<sprite=17> Donated to Museum";
                }
            }
            else if (itemInSlot.underwaterCreature)
            {
                var index = MuseumManager.manage.allUnderWaterCreatures.IndexOf(itemInSlot);
                if (MuseumManager.manage.underWaterCreaturesDonated[index])
                {
                    return "\n<sprite=17> Donated to Museum";
                }
            }

            return "";
        }
    }
}
