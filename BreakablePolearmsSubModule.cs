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

        public override void OnBeforeMissionBehaviorInitialize(Mission mission) => mission.AddMissionBehavior(new BreakablePolearmsMissionBehavior());
    }
}
