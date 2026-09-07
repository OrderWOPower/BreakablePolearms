using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BreakablePolearms
{
	public static class BreakablePolearmsHelper
	{
		public static bool IsWeaponBreakable(MissionWeapon weapon) => !weapon.IsEmpty && !weapon.HasAnyUsageWithWeaponClass(WeaponClass.Javelin) && weapon.CurrentUsageItem.IsPolearm && weapon.CurrentUsageItem.WeaponLength >= BreakablePolearmsSettings.Instance.MinPolearmLength;

		public static int MaxHitPoints(MissionWeapon weapon)
		{
			int hitPoints = weapon.CurrentUsageItem.SwingDamageType == DamageTypes.Invalid ? BreakablePolearmsSettings.Instance.NonSwingingPolearmHitPoints : BreakablePolearmsSettings.Instance.SwingingPolearmHitPoints;

			// Increase the polearm's HP by 0.5% for every point of handling.
			hitPoints += (int)(hitPoints * (weapon.GetWeaponComponentDataForUsage(0).Handling / 200f));
			// Increase the polearm's HP by 5% for every tier above 1.
			hitPoints += (int)(hitPoints * ((int)weapon.Item.Tier / 20f));

			return hitPoints;
		}

		public static MissionWeapon GetWieldedWeapon(Agent agent)
		{
			try
			{
				return agent.WieldedWeapon;
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(ex.ToString()));

				return MissionWeapon.Invalid;
			}
		}
	}
}
