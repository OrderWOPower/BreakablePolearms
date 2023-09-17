using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BreakablePolearms
{
    // This mod makes polearms breakable.
    public class BreakablePolearmsSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad() => new Harmony("mod.bannerlord.breakablepolearms").PatchAll();

        public override void OnBeforeMissionBehaviorInitialize(Mission mission)
        {
            if (mission.SceneName != "training_field_2")
            {
                mission.AddMissionBehavior(new BreakablePolearmsMissionBehavior());
            }
        }
    }
}
