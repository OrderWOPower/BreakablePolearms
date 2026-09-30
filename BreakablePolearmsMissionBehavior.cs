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
				agent.AddComponent(new BreakablePolearmsAgentComponent(agent));

				for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; index++)
				{
					if (BreakablePolearmsHelper.IsWeaponBreakable(agent.Equipment[index]))
					{
						// Initialize a polearm's HP.
						agent.ChangeWeaponHitPoints(index, (short)BreakablePolearmsHelper.MaxHitPoints(agent.Equipment[index]));
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
			if (BreakablePolearmsHelper.IsWeaponBreakable(weapon) && ((weapon.CurrentUsageItem.SwingDamageType == DamageTypes.Invalid && settings.ShouldDamageNonSwingingPolearms) || (weapon.CurrentUsageItem.SwingDamageType != DamageTypes.Invalid && settings.ShouldDamageSwingingPolearms)))
			{
				// Deal damage to the polearm equal to half the damage inflicted plus the damage absorbed by armor.
				int damageToWeapon = (collisionData.InflictedDamage / 2) + MathF.Max(collisionData.AbsorbedByArmor, 0), hitPoints;

				// Decrease damage to the polearm based on the wielder's Polearm skill.
				damageToWeapon -= (int)(damageToWeapon * MathF.Min(attacker.Character.GetSkillValue(DefaultSkills.Polearm) * (settings.SkillBasedDamageDecrementToPolearmsMultiplier / 100f), 1f));
				damageToWeapon = (int)(damageToWeapon * (attacker.IsMainAgent ? settings.DamageToPolearmsForPlayersMultiplier : settings.DamageToPolearmsForNonPlayersMultiplier));
				hitPoints = MathF.Max(0, weapon.HitPoints - damageToWeapon);
				attacker.ChangeWeaponHitPoints(attacker.GetPrimaryWieldedItemIndex(), (short)hitPoints);
				attacker.GetComponent<BreakablePolearmsAgentComponent>().DamageWeapon();

				if (hitPoints == 0)
				{
					BreakWeapon(attacker);
				}
			}
		}

		public override void OnMissionTick(float dt)
		{
			if (Agent.Main != null && BreakablePolearmsMixin.MixinWeakReference != null && BreakablePolearmsMixin.MixinWeakReference.TryGetTarget(out BreakablePolearmsMixin mixin))
			{
				MissionWeapon weapon = BreakablePolearmsHelper.GetWieldedWeapon(Agent.Main);

				if (BreakablePolearmsHelper.IsWeaponBreakable(weapon))
				{
					mixin.UpdateWeaponStatuses(weapon.HitPoints, BreakablePolearmsHelper.MaxHitPoints(weapon));
				}
				else
				{
					mixin.UpdateWeaponStatuses(0, 1);
				}
			}
		}

		private async void BreakWeapon(Agent agent)
		{
			await Task.Delay(1);

			if (agent != null && agent.GetPrimaryWieldedItemIndex() != EquipmentIndex.None && BreakablePolearmsHelper.IsWeaponBreakable(BreakablePolearmsHelper.GetWieldedWeapon(agent)))
			{
				// Find the frame of the agent's main hand.
				MatrixFrame mainHandFrame = agent.AgentVisuals.GetGlobalFrame().TransformToParent(agent.AgentVisuals.GetSkeleton().GetBoneEntitialFrameWithIndex(agent.Monster.MainHandItemBoneIndex));

				// If a polearm is broken, remove it from the wielder, emit wood breaking particles and play a breaking sound.
				agent.RemoveEquippedWeapon(agent.GetPrimaryWieldedItemIndex());
				Mission.AddParticleSystemBurstByName("psys_game_wood_splinter_a", mainHandFrame, false);
				Mission.AddParticleSystemBurstByName("psys_game_wood_splinter_a", mainHandFrame, false);
				Mission.AddParticleSystemBurstByName("psys_game_wood_dust_a", mainHandFrame, false);
				Mission.MakeSound(_breakSoundIndex, mainHandFrame.origin, false, true, -1, -1);
			}
		}
	}
}
