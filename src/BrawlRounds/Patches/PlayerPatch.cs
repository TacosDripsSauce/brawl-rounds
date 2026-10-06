using HarmonyLib;
using BrawlRounds.Runtime;
namespace BrawlRounds.Patches
{
    [HarmonyPatch(typeof(Player),"Update")]
    internal static class PlayerPatch
    {
        private static void Postfix(Player __instance){BrawlRuntime.TickPlayer(__instance);}
    }
}
