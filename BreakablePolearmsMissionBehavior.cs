using HarmonyLib;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BreakablePolearms
{
    [HarmonyPatch(typeof(Mission), "GetAttackCollisionResults")]
    public class BreakablePolearmsMissionBehavior : MissionBehavior
    {
        private static Agent _attacker, _victim;
        private static float _hitSpeed;

        private readonly int _breakSoundIndex;

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        private static void Postfix(CombatLogData __result, Agent attackerAgent, Agent victimAgent)
        {
            _attacker = attackerAgent;
            _victim = victimAgent;
            _hitSpeed = __result.HitSpeed;
        }

        public BreakablePolearmsMissionBehavior() => _breakSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/combat/shield/broken");

        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            if (agent.IsHuman)
            {
                for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; index++)
                {
                    MissionWeapon weapon = agent.Equipment[index];

                    if (IsWeaponBreakable(weapon))
                    {
                        // Initialize a polearm's HP based on its handling.
                        agent.ChangeWeaponHitPoints(index, (short)(weapon.CurrentUsageItem.Handling * (weapon.CurrentUsageItem.SwingDamageType == DamageTypes.Invalid ? BreakablePolearmsSettings.Instance.NonSwingingPolearmHitPointsMultiplier : BreakablePolearmsSettings.Instance.SwingingPolearmHitPointsMultiplier)));
                    }
                }
            }
        }

        public override void OnMeleeHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
        {
            int affectorWeaponSlotOrMissileIndex = collisionData.AffectorWeaponSlotOrMissileIndex;
            MissionWeapon weapon = affectorWeaponSlotOrMissileIndex >= 0 ? attacker.Equipment[affectorWeaponSlotOrMissileIndex] : MissionWeapon.Invalid;
            BreakablePolearmsSettings settings = BreakablePolearmsSettings.Instance;

            // Determine what types of polearms to deal damage to.
            if (IsWeaponBreakable(weapon) && ((weapon.CurrentUsageItem.SwingDamageType == DamageTypes.Invalid && settings.ShouldDamageNonSwingingPolearms) || (weapon.CurrentUsageItem.SwingDamageType != DamageTypes.Invalid && settings.ShouldDamageSwingingPolearms)))
            {
                // If a polearm hits an agent, deal damage to the polearm equal to 1 times the damage absorbed by armor. If a polearm hits a shield or an entity, deal damage to the polearm equal to 10 times the damage inflicted.
                int damageToWeapon = collisionData.AttackBlockedWithShield || collisionData.EntityExists ? collisionData.InflictedDamage * 10 : collisionData.AbsorbedByArmor;

                if (attacker == _attacker && victim == _victim)
                {
                    // Increase damage to the polearm based on relative movement speed.
                    damageToWeapon += (int)(damageToWeapon * (_hitSpeed * settings.SpeedBasedDamageIncrementToPolearmsMultiplier));
                }

                // Decrease damage to the polearm based on the wielder's Polearm skill.
                damageToWeapon -= (int)(damageToWeapon * MathF.Min(attacker.Character.GetSkillValue(DefaultSkills.Polearm) * (settings.SkillBasedDamageDecrementToPolearmsMultiplier / 100f), 1f));
                damageToWeapon = (int)(damageToWeapon * (attacker.IsMainAgent ? settings.DamageToPolearmsForPlayersMultiplier : settings.DamageToPolearmsForNonPlayersMultiplier));
                attacker.ChangeWeaponHitPoints((EquipmentIndex)affectorWeaponSlotOrMissileIndex, (short)MathF.Max(0, weapon.HitPoints - damageToWeapon));
            }
        }

        public override void OnMissionTick(float dt)
        {
            foreach (Agent agent in Mission.Agents.Where(a => a.IsHuman && a.WieldedWeapon.HitPoints == 0 && a.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.ExtraWeaponSlot && IsWeaponBreakable(a.WieldedWeapon)))
            {
                // If a polearm is broken, remove it from the wielder and play a breaking sound.
                agent.RemoveEquippedWeapon(agent.GetWieldedItemIndex(Agent.HandIndex.MainHand));
                Mission.MakeSound(_breakSoundIndex, agent.Position, false, true, -1, -1);
            }

            if (Agent.Main != null && BreakablePolearmsMixin.MixinWeakReference != null && BreakablePolearmsMixin.MixinWeakReference.TryGetTarget(out BreakablePolearmsMixin mixin))
            {
                MissionWeapon weapon = Agent.Main.WieldedWeapon;

                if (IsWeaponBreakable(weapon))
                {
                    mixin.UpdateWeaponStatuses(weapon.HitPoints, weapon.CurrentUsageItem.Handling * (weapon.CurrentUsageItem.SwingDamageType == DamageTypes.Invalid ? BreakablePolearmsSettings.Instance.NonSwingingPolearmHitPointsMultiplier : BreakablePolearmsSettings.Instance.SwingingPolearmHitPointsMultiplier));
                }
                else
                {
                    mixin.UpdateWeaponStatuses(0, 1);
                }
            }
        }

        private bool IsWeaponBreakable(MissionWeapon weapon) => !weapon.HasAnyUsageWithWeaponClass(WeaponClass.Javelin) && weapon.CurrentUsageItem != null && weapon.CurrentUsageItem.IsPolearm;
    }
}
