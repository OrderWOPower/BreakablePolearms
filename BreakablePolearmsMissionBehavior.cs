using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BreakablePolearms
{
    public class BreakablePolearmsMissionBehavior : MissionBehavior
    {
        private readonly int _breakSoundIndex;

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public BreakablePolearmsMissionBehavior() => _breakSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/combat/shield/broken");

        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            if (agent.IsHuman)
            {
                for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; index++)
                {
                    if (IsWeaponBreakable(agent.Equipment[index]))
                    {
                        // Initialize a polearm's HP.
                        agent.ChangeWeaponHitPoints(index, (short)MaxHitPoints(agent.Equipment[index]));
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
                // Deal damage to the polearm equal to half the damage inflicted plus the damage absorbed by armor.
                int damageToWeapon = (collisionData.InflictedDamage / 2) + MathF.Max(collisionData.AbsorbedByArmor, 0), hitPoints;

                // Decrease damage to the polearm based on the wielder's Polearm skill.
                damageToWeapon -= (int)(damageToWeapon * MathF.Min(attacker.Character.GetSkillValue(DefaultSkills.Polearm) * (settings.SkillBasedDamageDecrementToPolearmsMultiplier / 100f), 1f));
                damageToWeapon = (int)(damageToWeapon * (attacker.IsMainAgent ? settings.DamageToPolearmsForPlayersMultiplier : settings.DamageToPolearmsForNonPlayersMultiplier));
                hitPoints = MathF.Max(0, weapon.HitPoints - damageToWeapon);
                attacker.ChangeWeaponHitPoints(attacker.GetPrimaryWieldedItemIndex(), (short)hitPoints);

                if (hitPoints == 0)
                {
                    BreakWeapon(attacker);
                }
            }
        }

        public override void OnMissionTick(float dt)
        {
            Agent mainAgent = Agent.Main;

            if (mainAgent != null && mainAgent.IsActive() && BreakablePolearmsMixin.MixinWeakReference != null && BreakablePolearmsMixin.MixinWeakReference.TryGetTarget(out BreakablePolearmsMixin mixin))
            {
                MissionWeapon weapon = mainAgent.WieldedWeapon;

                if (IsWeaponBreakable(weapon))
                {
                    mixin.UpdateWeaponStatuses(weapon.HitPoints, MaxHitPoints(weapon));
                }
                else
                {
                    mixin.UpdateWeaponStatuses(0, 1);
                }
            }
        }

        private bool IsWeaponBreakable(MissionWeapon weapon) => !weapon.IsEmpty && !weapon.HasAnyUsageWithWeaponClass(WeaponClass.Javelin) && weapon.CurrentUsageItem.IsPolearm && weapon.CurrentUsageItem.WeaponLength >= BreakablePolearmsSettings.Instance.MinPolearmLength;

        private int MaxHitPoints(MissionWeapon weapon)
        {
            int hitPoints = weapon.CurrentUsageItem.SwingDamageType == DamageTypes.Invalid ? BreakablePolearmsSettings.Instance.NonSwingingPolearmHitPoints : BreakablePolearmsSettings.Instance.SwingingPolearmHitPoints;

            // Increase the polearm's HP by 0.5% for every point of handling.
            hitPoints += (int)(hitPoints * (weapon.GetWeaponComponentDataForUsage(0).Handling / 200f));
            // Increase the polearm's HP by 5% for every tier above 1.
            hitPoints += (int)(hitPoints * ((int)weapon.Item.Tier / 20f));

            return hitPoints;
        }

        private async void BreakWeapon(Agent agent)
        {
            await Task.Delay(1);

            if (agent != null && agent.GetPrimaryWieldedItemIndex() != EquipmentIndex.None && IsWeaponBreakable(agent.WieldedWeapon))
            {
                // If a polearm is broken, remove it from the wielder and play a breaking sound.
                agent.RemoveEquippedWeapon(agent.GetPrimaryWieldedItemIndex());
                Mission.MakeSound(_breakSoundIndex, agent.Position, false, true, -1, -1);
            }
        }
    }
}
