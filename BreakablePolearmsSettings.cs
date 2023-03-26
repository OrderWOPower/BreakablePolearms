using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace BreakablePolearms
{
    public class BreakablePolearmsSettings : AttributeGlobalSettings<BreakablePolearmsSettings>
    {
        public override string Id => "BreakablePolearms";

        public override string DisplayName => "Breakable Polearms";

        public override string FolderName => "BreakablePolearms";

        public override string FormatType => "json2";

        [SettingPropertyBool("Toggle Non-Swinging Polearms", Order = 0, RequireRestart = false, HintText = "Deal damage to non-swinging polearms. Enabled by default.", IsToggle = true)]
        [SettingPropertyGroup("Non-Swinging Polearms", GroupOrder = 0)]
        public bool ShouldDamageNonSwingingPolearms { get; set; } = true;

        [SettingPropertyInteger("HP of Non-Swinging Polearms", 1, 100, "0", Order = 1, RequireRestart = false, HintText = "Multipler for a non-swinging polearm's HP relative to its handling. Default is 20.")]
        [SettingPropertyGroup("Non-Swinging Polearms", GroupOrder = 0)]
        public int NonSwingingPolearmHitPointsMultiplier { get; set; } = 20;

        [SettingPropertyBool("Toggle Swinging Polearms", Order = 0, RequireRestart = false, HintText = "Deal damage to swinging polearms. Enabled by default.", IsToggle = true)]
        [SettingPropertyGroup("Swinging Polearms", GroupOrder = 1)]
        public bool ShouldDamageSwingingPolearms { get; set; } = true;

        [SettingPropertyInteger("HP of Swinging Polearms", 1, 100, "0", Order = 1, RequireRestart = false, HintText = "Multipler for a swinging polearm's HP relative to its handling. Default is 40.")]
        [SettingPropertyGroup("Swinging Polearms", GroupOrder = 1)]
        public int SwingingPolearmHitPointsMultiplier { get; set; } = 40;

        [SettingPropertyFloatingInteger("Damage to Polearms", 0.0f, 10.0f, "0.0", Order = 0, RequireRestart = false, HintText = "Multiplier for damage to polearms. Default is 1.0.")]
        [SettingPropertyGroup("Multipliers", GroupOrder = 2)]
        public float DamageToPolearmsMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("Damage Reduction to Polearms", 0.00f, 0.33f, "#0.00\\%", Order = 1, RequireRestart = false, HintText = "Multiplier for damage reduction to polearms relative to Polearm skill. Default is 0.33%.")]
        [SettingPropertyGroup("Multipliers", GroupOrder = 2)]
        public float DamageReductionToPolearmsMultiplier { get; set; } = 0.33f;
    }
}
