using TaleWorlds.MountAndBlade;

namespace BreakablePolearms
{
	public class BreakablePolearmsAgentComponent : AgentComponent
	{
		private bool _isWeaponTakingDamage;
		private int _weaponIndex, _weaponHitPoints;

		public BreakablePolearmsAgentComponent(Agent agent) : base(agent)
		{
			_isWeaponTakingDamage = false;
			_weaponIndex = -1;
			_weaponHitPoints = 0;
		}

		public override void OnTick(float dt)
		{
			MissionWeapon weapon = BreakablePolearmsHelper.GetWieldedWeapon(Agent);

			if (BreakablePolearmsHelper.IsWeaponBreakable(weapon))
			{
				if ((int)Agent.GetPrimaryWieldedItemIndex() == _weaponIndex)
				{
					if (!_isWeaponTakingDamage)
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
						_isWeaponTakingDamage = false;
						_weaponHitPoints = weapon.HitPoints;
					}
				}
				else
				{
					_isWeaponTakingDamage = false;
					_weaponIndex = (int)Agent.GetPrimaryWieldedItemIndex();
					_weaponHitPoints = 0;
				}

			}
			else
			{
				_isWeaponTakingDamage = false;
				_weaponIndex = -1;
				_weaponHitPoints = 0;
			}
		}

		public void DamageWeapon() => _isWeaponTakingDamage = true;
	}
}
