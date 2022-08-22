using HarmonyLib;

namespace CritterDisplay
{
    [HarmonyPatch(typeof(NetworkNavMesh), "UnSpawnAnAnimal")]
    internal class UnSpawnAnAnimalPatch
    {
        [HarmonyPrefix]
        private static void unSpawnAnAnimalPatch(ref AnimalAI despawnMe)
        {
            Plugin.animalAIs.Remove(despawnMe);
        }
    }
}
