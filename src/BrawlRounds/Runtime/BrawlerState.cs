using System.Collections.Generic;
using UnityEngine;
using BrawlRounds.Generated;

namespace BrawlRounds.Runtime
{
    internal sealed class BrawlerState : MonoBehaviour
    {
        public Player Player { get; private set; }
        public float Percent { get; set; }
        public WeaponDefinition Weapon { get; set; }
        public float LastAttackAt { get; set; }
        public float OutgoingKnockback { get; private set; } = 1f;
        public float IncomingKnockback { get; private set; } = 1f;
        public float AttackCooldown { get; private set; } = 1f;
        public float WeaponRange { get; private set; } = 1f;
        private readonly Dictionary<string, CardEffect> cardEffects = new Dictionary<string, CardEffect>();

        private void Awake() { Player = GetComponent<Player>(); }
        public void ResetPoint() { Percent = 0f; Weapon = null; LastAttackAt = -100f; }
        public void SetCard(CardEffect effect) { cardEffects[effect.Id] = effect; Recalculate(); }
        public void RemoveCard(string id) { cardEffects.Remove(id); Recalculate(); }
        private void Recalculate()
        {
            OutgoingKnockback=1f; IncomingKnockback=1f; AttackCooldown=1f; WeaponRange=1f;
            foreach (var e in cardEffects.Values)
            {
                OutgoingKnockback *= e.OutgoingKnockback;
                IncomingKnockback *= e.IncomingKnockback;
                AttackCooldown *= e.AttackCooldown;
                WeaponRange *= e.WeaponRange;
            }
        }
    }
}
