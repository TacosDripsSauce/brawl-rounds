using UnityEngine;

namespace BrawlRounds.Runtime
{
    internal sealed class PickupBehaviour : MonoBehaviour
    {
        public string WeaponId;
        public int Slot;
        private float activeAt;
        public void HideFor(float seconds) { activeAt=Time.time+seconds; SetVisible(false); }
        public void ActivateNow() { activeAt=0f; SetVisible(true); }
        private void Update()
        {
            if (Time.time<activeAt) return;
            SetVisible(true);
            if (PlayerManager.instance == null) return;
            foreach (var p in PlayerManager.instance.players)
            {
                if (p == null || p.data == null || p.data.dead || !p.data.view.IsMine) continue;
                if (Vector2.Distance(transform.position,p.transform.position)<0.85f)
                {
                    BrawlRuntime.RequestPickup(p, WeaponId, Slot);
                    HideFor(7f);
                    return;
                }
            }
        }
        private void SetVisible(bool value) { var r=GetComponent<SpriteRenderer>(); if (r!=null) r.enabled=value; }
    }
}
