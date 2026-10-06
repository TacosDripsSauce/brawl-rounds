using System;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using UnboundLib.Networking;

namespace BrawlRounds.Runtime
{
    internal static class BrawlRuntime
    {
        private const string PickupEvent="BrawlRounds_Pickup_v1";
        private const string MeleeEvent="BrawlRounds_Melee_v1";
        public static bool Enabled=true;

        public static void InitNetwork()
        {
            NetworkingManager.RegisterEvent(PickupEvent, OnPickupEvent);
            NetworkingManager.RegisterEvent(MeleeEvent, OnMeleeEvent);
        }
        public static BrawlerState State(Player p)
        {
            var s=p.GetComponent<BrawlerState>(); if(s==null) s=p.gameObject.AddComponent<BrawlerState>();
            if(p.GetComponent<MovementController>()==null) p.gameObject.AddComponent<MovementController>();
            return s;
        }
        public static Player FindPlayer(int id)
        {
            if(PlayerManager.instance==null) return null;
            foreach(var p in PlayerManager.instance.players) if(p!=null && p.playerID==id) return p;
            return null;
        }
        public static void ResetPoint()
        {
            if(PlayerManager.instance!=null) foreach(var p in PlayerManager.instance.players) if(p!=null) State(p).ResetPoint();
            PickupSpawner.Spawn();
        }
        public static void RequestPickup(Player p,string weaponId,int slot)
        {
            if(p==null || !p.data.view.IsMine) return;
            NetworkingManager.RaiseEvent(PickupEvent,p.playerID,weaponId,slot);
        }
        private static void OnPickupEvent(object[] data)
        {
            if(data==null || data.Length<3) return; var p=FindPlayer(Convert.ToInt32(data[0])); var def=WeaponCatalog.Get((string)data[1]); if(p==null||def==null)return;
            Equip(p,def); PickupSpawner.HideSlot(Convert.ToInt32(data[2]));
        }
        public static void Equip(Player p, BrawlRounds.Generated.WeaponDefinition def)
        {
            var s=State(p); s.Weapon=def;
            var gun=p.data.weaponHandler.gun; if(gun==null)return;
            gun.numberOfProjectiles=def.Kind=="ranged"?def.Projectiles:1;
            gun.spread=def.Kind=="ranged"?def.Spread:0f;
            gun.attackSpeed=Mathf.Max(.05f,def.Cooldown*s.AttackCooldown);
            gun.knockback=def.BaseKnockback;
            gun.damage=.12f;
        }
        public static bool TryMelee(Gun gun)
        {
            if(gun==null || gun.player==null) return false; var s=State(gun.player); var w=s.Weapon;
            if(w==null) return true; // suppress attacks while unarmed
            if(w.Kind!="melee") return false;
            if(Time.time<s.LastAttackAt+w.Cooldown*s.AttackCooldown) return true;
            s.LastAttackAt=Time.time;
            if(!gun.player.data.view.IsMine) return true;
            var aim=gun.player.data.input.aimDirection; if(aim.sqrMagnitude<.01f) aim=Vector2.right;
            var origin=(Vector2)gun.player.transform.position; Player best=null; float dist=w.Range*s.WeaponRange;
            foreach(var p in PlayerManager.instance.players)
            {
                if(p==null||p==gun.player||p.data.dead)continue; var delta=(Vector2)p.transform.position-origin;
                if(delta.magnitude<=dist && Vector2.Dot(delta.normalized,aim.normalized)>.15f){best=p;dist=delta.magnitude;}
            }
            if(best!=null) NetworkingManager.RaiseEvent(MeleeEvent,best.playerID,gun.player.playerID,aim.x,aim.y,w.Id);
            return true;
        }
        private static void OnMeleeEvent(object[] data)
        {
            if(data==null||data.Length<5)return; var target=FindPlayer(Convert.ToInt32(data[0])); var attacker=FindPlayer(Convert.ToInt32(data[1])); var w=WeaponCatalog.Get((string)data[4]);
            if(target==null||w==null)return; ApplyBrawlerHit(target,attacker,w.DamagePercent,new Vector2(Convert.ToSingle(data[2]),Convert.ToSingle(data[3])),w.BaseKnockback);
        }
        public static void ApplyBrawlerHit(Player target,Player attacker,float damagePercent,Vector2 direction,float baseKnockback)
        {
            if(target==null||target.data.dead)return; var t=State(target); var a=attacker==null?null:State(attacker);
            t.Percent=Mathf.Clamp(t.Percent+Mathf.Max(0f,damagePercent),0f,999f);
            float force=baseKnockback*(1f+t.Percent/95f)*t.IncomingKnockback*(a==null?1f:a.OutgoingKnockback);
            if(target.data.view.IsMine)
            {
                var dir=direction.sqrMagnitude>.01f?direction.normalized:Vector2.up;
                try { Traverse.Create(target.data.playerVel).Method("AddForce",new object[]{dir*force*42f}).GetValue(); } catch { }
            }
        }
        public static void TickPlayer(Player p)
        {
            if(!Enabled||p==null||p.data==null||p.data.dead)return; State(p);
            if(!p.data.view.IsMine)return; var cam=Camera.main; if(cam==null)return; var v=cam.WorldToViewportPoint(p.transform.position);
            if(v.x<-.12f||v.x>1.12f||v.y<-.18f||v.y>1.18f)
            {
                var dir=((Vector2)p.transform.position-(Vector2)cam.transform.position).normalized;
                try { p.data.view.RPC("RPCA_Die",RpcTarget.All,new object[]{dir}); } catch { }
            }
        }
    }
}
