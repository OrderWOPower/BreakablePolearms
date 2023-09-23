using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace BreakablePolearms
{
    // Add the percentage of the polearm's HP to the player's HUD.
    [PrefabExtension("AgentStatus", "descendant::AgentAmmoTextWidget")]
    public class BreakablePolearmsPrefab : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Append;

        [PrefabExtensionText]
        public string Text => "<AgentAmmoTextWidget DataSource=\"{..}\" WidthSizePolicy=\"Fixed\" HeightSizePolicy=\"Fixed\" SuggestedWidth=\"120\" SuggestedHeight=\"40\" HorizontalAlignment=\"Left\" VerticalAlignment=\"Top\" MarginLeft=\"10\" Brush=\"AgentAmmoCount.Text\" Text=\"@WeaponHitPoints\" IsVisible=\"@IsWeaponHitPointsVisible\" IsAlertEnabled=\"@IsWeaponHitPointsAlertEnabled\"/>";
    }
}
