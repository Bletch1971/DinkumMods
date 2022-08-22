using HarmonyLib;

namespace CritterDisplay
{
    [HarmonyPatch(typeof(AnimalManager), "spawnFreeAnimal")]
    internal class SpawnFreeAnimalPatch
    {
        [HarmonyPostfix]
        private static void spawnFreeAnimalPatch(AnimalAI __result)
        {
            Plugin.animalAIs.Add(__result);
        }
    }
}
