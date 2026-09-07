using TaleWorlds.MountAndBlade;

namespace BreakablePolearms
{
	public class BreakablePolearmsAgentComponent : AgentComponent
	{
		private int _weaponHitPoints;

		public bool IsWeaponTakingDamage { get; private set; }

		public BreakablePolearmsAgentComponent(Agent agent) : base(agent) { }

		public override void OnTick(float dt)
		{
			MissionWeapon weapon = BreakablePolearmsHelper.GetWieldedWeapon(Agent);

			if (BreakablePolearmsHelper.IsWeaponBreakable(weapon))
			{
				if (!IsWeaponTakingDamage)
				{
					if (weapon.HitPoints >= _weaponHitPoints)
					{
						_weaponHitPoints = weapon.HitPoints;
					}
					else
					{
						// If a polearm's HP decreases not due to damage, change it back to its previous value.
						Agent.ChangeWeaponHitPoints(Agent.GetPrimaryWieldedItemIndex(), (short)_weaponHitPoints);
					}
				}
				else
				{
					IsWeaponTakingDamage = false;

					_weaponHitPoints = weapon.HitPoints;
				}
			}
			else
			{
				IsWeaponTakingDamage = false;

				_weaponHitPoints = 0;
			}
		}

		public void DamageWeapon() => IsWeaponTakingDamage = true;
	}
}
