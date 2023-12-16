using Bannerlord.UIExtenderEx;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BreakablePolearms
{
    // This mod makes polearms breakable.
    public class BreakablePolearmsSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            UIExtender uiExtender = new UIExtender("BreakablePolearms");

            uiExtender.Register(typeof(BreakablePolearmsSubModule).Assembly);
            uiExtender.Enable();
            new Harmony("mod.bannerlord.breakablepolearms").PatchAll();
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
