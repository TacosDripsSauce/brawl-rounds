using System.Collections.Generic;
using UnityEngine;

namespace BrawlRounds.Runtime
{
    internal static class PickupSpawner
    {
        private static readonly List<GameObject> live=new List<GameObject>();
        private static Sprite square;
        public static void Clear() { foreach(var g in live) if(g!=null) Object.Destroy(g); live.Clear(); }
        public static void Spawn()
        {
            Clear(); var cam=Camera.main; if(cam==null) return;
            var weapons=new[]{"sword","spear","pistol","shotgun"};
            var view=new[]{new Vector2(.20f,.62f),new Vector2(.40f,.42f),new Vector2(.60f,.42f),new Vector2(.80f,.62f)};
            for(int i=0;i<weapons.Length;i++)
            {
                var w=cam.ViewportToWorldPoint(new Vector3(view[i].x,view[i].y,Mathf.Abs(cam.transform.position.z)));
                var go=new GameObject("BrawlRoundsPickup_"+weapons[i]); go.transform.position=new Vector3(w.x,w.y,0); go.transform.localScale=new Vector3(.45f,.18f,1);
                var sr=go.AddComponent<SpriteRenderer>(); sr.sprite=Square(); sr.color=new Color(.95f,.82f,.18f,.95f);
                var pb=go.AddComponent<PickupBehaviour>(); pb.WeaponId=weapons[i]; pb.Slot=i; live.Add(go);
            }
        }
        public static void HideSlot(int slot)
        {
            if(slot>=0 && slot<live.Count && live[slot]!=null) live[slot].GetComponent<PickupBehaviour>().HideFor(7f);
        }
        private static Sprite Square()
        {
            if(square!=null) return square; var t=new Texture2D(1,1); t.SetPixel(0,0,Color.white); t.Apply(); square=Sprite.Create(t,new Rect(0,0,1,1),new Vector2(.5f,.5f),1f); return square;
        }
    }
}
