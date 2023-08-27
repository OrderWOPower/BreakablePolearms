﻿using MCM.Abstractions.Attributes;
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

        [SettingPropertyBool("{=BreakableOpt001}Toggle Non-Swinging Polearms", Order = 0, RequireRestart = false, HintText = "{=BreakableOpt001Hint}Deal damage to non-swinging polearms. Enabled by default.", IsToggle = true)]
        [SettingPropertyGroup("{=BreakableOptG001}Non-Swinging Polearms", GroupOrder = 0)]
        public bool ShouldDamageNonSwingingPolearms { get; set; } = true;

        [SettingPropertyInteger("{=BreakableOpt002}HP of Non-Swinging Polearms", 1, 100, "0", Order = 1, RequireRestart = false, HintText = "{=BreakableOpt002Hint}Multiplier for a non-swinging polearm's HP based on its handling. Default is 20.")]
        [SettingPropertyGroup("{=BreakableOptG001}Non-Swinging Polearms", GroupOrder = 0)]
        public int NonSwingingPolearmHitPointsMultiplier { get; set; } = 20;

        [SettingPropertyBool("{=BreakableOpt003}Toggle Swinging Polearms", Order = 0, RequireRestart = false, HintText = "{=BreakableOpt003Hint}Deal damage to swinging polearms. Enabled by default.", IsToggle = true)]
        [SettingPropertyGroup("{=BreakableOptG002}Swinging Polearms", GroupOrder = 1)]
        public bool ShouldDamageSwingingPolearms { get; set; } = true;

        [SettingPropertyInteger("{=BreakableOpt004}HP of Swinging Polearms", 1, 100, "0", Order = 1, RequireRestart = false, HintText = "{=BreakableOpt004Hint}Multiplier for a swinging polearm's HP based on its handling. Default is 40.")]
        [SettingPropertyGroup("{=BreakableOptG002}Swinging Polearms", GroupOrder = 1)]
        public int SwingingPolearmHitPointsMultiplier { get; set; } = 40;

        [SettingPropertyFloatingInteger("{=BreakableOpt005}Damage to Polearms for PCs", 0.0f, 10.0f, "0.0", Order = 0, RequireRestart = false, HintText = "{=BreakableOpt005Hint}Multiplier for damage to polearms for player characters. Default is 1.0.")]
        [SettingPropertyGroup("{=BreakableOptG003}Multipliers", GroupOrder = 2)]
        public float DamageToPolearmsForPlayersMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("{=BreakableOpt006}Damage to Polearms for NPCs", 0.0f, 10.0f, "0.0", Order = 1, RequireRestart = false, HintText = "{=BreakableOpt006Hint}Multiplier for damage to polearms for non-player characters. Default is 1.0.")]
        [SettingPropertyGroup("{=BreakableOptG003}Multipliers", GroupOrder = 2)]
        public float DamageToPolearmsForNonPlayersMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("{=BreakableOpt007}Speed-based Damage Increment to Polearms", 0.0f, 1.0f, "#0%", Order = 2, RequireRestart = false, HintText = "{=BreakableOpt007Hint}Multiplier for damage increment to polearms based on relative movement speed. Default is 50%.")]
        [SettingPropertyGroup("{=BreakableOptG003}Multipliers", GroupOrder = 2)]
        public float SpeedBasedDamageIncrementToPolearmsMultiplier { get; set; } = 0.5f;

        [SettingPropertyFloatingInteger("{=BreakableOpt008}Skill-based Damage Decrement to Polearms", 0.00f, 1.00f, "#0.00\\%", Order = 3, RequireRestart = false, HintText = "{=BreakableOpt008Hint}Multiplier for damage decrement to polearms based on Polearm skill. Default is 0.33%.")]
        [SettingPropertyGroup("{=BreakableOptG003}Multipliers", GroupOrder = 2)]
        public float SkillBasedDamageDecrementToPolearmsMultiplier { get; set; } = 0.33f;
    }
}
