using Bannerlord.UIExtenderEx;
using TaleWorlds.MountAndBlade;

namespace BreakablePolearms
{
    // This mod makes polearms breakable.
    public class BreakablePolearmsSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            UIExtender uiExtender = UIExtender.Create("BreakablePolearms");

            uiExtender.Register(typeof(BreakablePolearmsSubModule).Assembly);
            uiExtender.Enable();
        }

        public override void OnBeforeMissionBehaviorInitialize(Mission mission)
        {
            // Disable damage to polearms in the training field.
            if (mission.SceneName != "training_field_2")
            {
                mission.AddMissionBehavior(new BreakablePolearmsMissionBehavior());
            }
        }
    }
}
