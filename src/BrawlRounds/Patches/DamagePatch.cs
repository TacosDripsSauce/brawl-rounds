using HarmonyLib;
using UnityEngine;
using BrawlRounds.Runtime;

namespace BrawlRounds.Patches
{
    [HarmonyPatch(typeof(HealthHandler),"DoDamage")]
    internal static class DamagePatch
    {
        private static bool Prefix(HealthHandler __instance, ref Vector2 damage, Vector2 position, Color blinkColor, GameObject damagingWeapon, Player damagingPlayer, bool healthRemoval, ref bool lethal, bool ignoreBlock)
        {
            if(!BrawlRuntime.Enabled)return true;
            CharacterData data=null; try{data=Traverse.Create(__instance).Field("data").GetValue<CharacterData>();}catch{}
            if(data==null||data.player==null)return true;
            float pct=Mathf.Max(1f,damage.magnitude/Mathf.Max(1f,data.maxHealth)*100f); float kb=5f;
            if(damagingPlayer!=null)
            {
                var s=BrawlRuntime.State(damagingPlayer); if(s.Weapon!=null){pct=s.Weapon.DamagePercent;kb=s.Weapon.BaseKnockback;}
            }
            BrawlRuntime.ApplyBrawlerHit(data.player,damagingPlayer,pct,damage,kb);
            damage=Vector2.zero; lethal=false;
            return false;
        }
    }
}
