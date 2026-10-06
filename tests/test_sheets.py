import json, unittest
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]

def sheet(name):
    return json.loads((ROOT/'sheets'/f'{name}.json').read_text())['rows']

class SheetTests(unittest.TestCase):
    def test_first_release_has_exact_weapon_set(self):
        self.assertEqual({r['id'] for r in sheet('weapons')},{'sword','spear','pistol','shotgun'})

    def test_melee_and_ranged_are_both_present(self):
        kinds={r['kind'] for r in sheet('weapons')}
        self.assertEqual(kinds,{'melee','ranged'})

    def test_weapon_numbers_are_playable_positive_values(self):
        for r in sheet('weapons'):
            self.assertGreater(r['damagePercent'],0)
            self.assertGreater(r['baseKnockback'],0)
            self.assertGreater(r['range'],0)
            self.assertGreater(r['cooldown'],0)
            self.assertGreaterEqual(r['projectiles'],1)

    def test_knockback_curve_rises_with_percent(self):
        # Mirrors ApplyBrawlerHit's core force curve, excluding card multipliers.
        for r in sheet('weapons'):
            f0=r['baseKnockback']*(1+0/95)
            f50=r['baseKnockback']*(1+50/95)
            f100=r['baseKnockback']*(1+100/95)
            self.assertLess(f0,f50)
            self.assertLess(f50,f100)

    def test_cards_cover_the_requested_axes(self):
        cards=sheet('cards')
        self.assertEqual(len(cards),8)
        self.assertTrue(any(r['movementSpeedMultiplier'] != 1 for r in cards))
        self.assertTrue(any(r['extraJumps'] > 0 for r in cards))
        self.assertTrue(any(r['outgoingKnockbackMultiplier'] != 1 for r in cards))
        self.assertTrue(any(r['attackCooldownMultiplier'] != 1 for r in cards))
        self.assertTrue(any(r['weaponRangeMultiplier'] != 1 for r in cards))

    def test_project_is_real_rounds_mod_and_1v1(self):
        p=json.loads((ROOT/'sheets'/'project.json').read_text())
        self.assertEqual(p['hostGame'],'ROUNDS')
        self.assertEqual(p['requiredGames'],['ROUNDS'])
        self.assertEqual(p['players']['min'],2)
        self.assertEqual(p['players']['max'],2)

if __name__=='__main__': unittest.main()
