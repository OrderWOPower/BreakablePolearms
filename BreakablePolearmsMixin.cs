using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection;

namespace BreakablePolearms
{
    [ViewModelMixin]
    public class BreakablePolearmsMixin : BaseViewModelMixin<MissionAgentStatusVM>
    {
        private string _weaponHitPoints;
        private bool _isWeaponHitPointsVisible, _isWeaponHitPointsAlertEnabled;

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

        public void UpdateWeaponStatuses(int hitPoints, int maxHitPoints)
        {
            int hitPointsPercentage = MathF.Ceiling(hitPoints / (maxHitPoints / 100f));

            MBTextManager.SetTextVariable("NUMBER", hitPointsPercentage.ToString());

            WeaponHitPoints = new TextObject("{=gYATKZJp}{NUMBER}%").ToString();
            IsWeaponHitPointsVisible = hitPointsPercentage > 0;
            IsWeaponHitPointsAlertEnabled = hitPointsPercentage <= 20;
        }
    }
}
