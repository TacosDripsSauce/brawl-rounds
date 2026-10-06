using System.Collections;
using BepInEx;
using HarmonyLib;
using UnboundLib;
using UnboundLib.GameModes;
using BrawlRounds.Runtime;
using UnityEngine;

namespace BrawlRounds
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string ModId="thms.smnt.brawlrounds";
        public const string ModName="Brawl ROUNDS";
        public const string Version="0.1.0";
        private Harmony harmony;
        private void Awake()
        {
            harmony=new Harmony(ModId); harmony.PatchAll(); BrawlRuntime.InitNetwork();
            GameModeManager.AddHook(GameModeHooks.HookRoundStart,OnRoundStart);
            Logger.LogInfo("Brawl ROUNDS loaded: platform-fighter mode enabled.");
        }
        private void Start()
        {
            CardRegistry.RegisterAll();
        }
        private IEnumerator OnRoundStart(IGameModeHandler gm)
        {
            yield return new WaitForSeconds(.35f); BrawlRuntime.ResetPoint();
        }
        private void OnGUI()
        {
            if(PlayerManager.instance==null)return; float x=18f;
            foreach(var p in PlayerManager.instance.players)
            {
                if(p==null)continue; var s=BrawlRuntime.State(p);
                GUI.Label(new Rect(x,Screen.height-56,250,38),$"P{p.playerID+1}  {s.Percent:0}%  {(s.Weapon==null?"UNARMED":s.Weapon.DisplayName)}"); x+=260f;
            }
        }
        private void OnDestroy(){ if(harmony!=null)harmony.UnpatchSelf(); PickupSpawner.Clear(); }
    }
}
