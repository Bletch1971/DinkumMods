using HarmonyLib;

namespace CritterDisplay
{
    internal class CritterDisplayPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AnimalManager), "spawnFreeAnimal")]
        private static void spawnFreeAnimalPatch(AnimalAI __result)
        {
            Plugin.animalAIs.Add(__result);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NetworkNavMesh), "UnSpawnAnAnimal")]
        private static void unSpawnAnAnimalPatch(ref AnimalAI despawnMe)
        {
            Plugin.animalAIs.Remove(despawnMe);
        }
    }
}
