using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection;

namespace BreakablePolearms
{
    [ViewModelMixin]
    public class BreakablePolearmsMixin : BaseViewModelMixin<MissionAgentStatusVM>
    {
        private string _weaponHitPoints;
        private bool _isWeaponHitPointsVisible;
        private bool _isWeaponHitPointsAlertEnabled;

        [DataSourceProperty]
        public string WeaponHitPoints
        {
            get => _weaponHitPoints;

            set
            {
                if (value != _weaponHitPoints)
                {
                    _weaponHitPoints = value;

                    ViewModel?.OnPropertyChangedWithValue(value, "WeaponHitPoints");
                }
            }
        }

        [DataSourceProperty]
        public bool IsWeaponHitPointsVisible
        {
            get => _isWeaponHitPointsVisible;

            set
            {
                if (value != _isWeaponHitPointsVisible)
                {
                    _isWeaponHitPointsVisible = value;

                    ViewModel?.OnPropertyChangedWithValue(value, "IsWeaponHitPointsVisible");
                }
            }
        }

        [DataSourceProperty]
        public bool IsWeaponHitPointsAlertEnabled
        {
            get => _isWeaponHitPointsAlertEnabled;

            set
            {
                if (value != _isWeaponHitPointsAlertEnabled)
                {
                    _isWeaponHitPointsAlertEnabled = value;

                    ViewModel?.OnPropertyChangedWithValue(value, "IsWeaponHitPointsAlertEnabled");
                }
            }
        }

        public static WeakReference<BreakablePolearmsMixin> MixinWeakReference { get; set; }

        public BreakablePolearmsMixin(MissionAgentStatusVM missionAgentStatusVM) : base(missionAgentStatusVM) => MixinWeakReference = new WeakReference<BreakablePolearmsMixin>(this);

        public void UpdateWeaponStatuses(int currentHitPoints, int initialHitPoints)
        {
            int hitPointsPercentage = MathF.Ceiling(currentHitPoints / (initialHitPoints / 100f));

            WeaponHitPoints = hitPointsPercentage.ToString() + "%";
            IsWeaponHitPointsVisible = hitPointsPercentage > 0;
            IsWeaponHitPointsAlertEnabled = hitPointsPercentage <= 20;
        }
    }
}
