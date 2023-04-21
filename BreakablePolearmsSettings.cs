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

        [SettingPropertyInteger("HP of Non-Swinging Polearms", 1, 100, "0", Order = 1, RequireRestart = false, HintText = "Multipler for a non-swinging polearm's HP based on its handling. Default is 20.")]
        [SettingPropertyGroup("Non-Swinging Polearms", GroupOrder = 0)]
        public int NonSwingingPolearmHitPointsMultiplier { get; set; } = 20;

        [SettingPropertyBool("Toggle Swinging Polearms", Order = 0, RequireRestart = false, HintText = "Deal damage to swinging polearms. Enabled by default.", IsToggle = true)]
        [SettingPropertyGroup("Swinging Polearms", GroupOrder = 1)]
        public bool ShouldDamageSwingingPolearms { get; set; } = true;

        [SettingPropertyInteger("HP of Swinging Polearms", 1, 100, "0", Order = 1, RequireRestart = false, HintText = "Multipler for a swinging polearm's HP based on its handling. Default is 40.")]
        [SettingPropertyGroup("Swinging Polearms", GroupOrder = 1)]
        public int SwingingPolearmHitPointsMultiplier { get; set; } = 40;

        [SettingPropertyFloatingInteger("Damage to Polearms for PCs", 0.0f, 10.0f, "0.0", Order = 0, RequireRestart = false, HintText = "Multiplier for damage to polearms for player characters. Default is 1.0.")]
        [SettingPropertyGroup("Multipliers", GroupOrder = 2)]
        public float DamageToPolearmsForPlayersMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("Damage to Polearms for NPCs", 0.0f, 10.0f, "0.0", Order = 1, RequireRestart = false, HintText = "Multiplier for damage to polearms for non-player characters. Default is 1.0.")]
        [SettingPropertyGroup("Multipliers", GroupOrder = 2)]
        public float DamageToPolearmsForNonPlayersMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("Speed-based Damage Increment to Polearms", 0.0f, 1.0f, "#0%", Order = 2, RequireRestart = false, HintText = "Multiplier for damage increment to polearms based on relative movement speed. Default is 10%.")]
        [SettingPropertyGroup("Multipliers", GroupOrder = 2)]
        public float SpeedBasedDamageIncrementToPolearmsMultiplier { get; set; } = 0.1f;

        [SettingPropertyFloatingInteger("Skill-based Damage Decrement to Polearms", 0.00f, 1.00f, "#0.00\\%", Order = 3, RequireRestart = false, HintText = "Multiplier for damage decrement to polearms based on Polearm skill. Default is 0.33%.")]
        [SettingPropertyGroup("Multipliers", GroupOrder = 2)]
        public float SkillBasedDamageDecrementToPolearmsMultiplier { get; set; } = 0.33f;
    }
}
