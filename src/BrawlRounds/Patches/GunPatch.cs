using HarmonyLib;
using BrawlRounds.Runtime;
namespace BrawlRounds.Patches
{
    [HarmonyPatch(typeof(Gun),"DoAttack")]
    internal static class GunPatch
    {
        private static bool Prefix(Gun __instance) { return !BrawlRuntime.TryMelee(__instance); }
    }
}
