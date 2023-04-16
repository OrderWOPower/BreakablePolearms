using HarmonyLib;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BreakablePolearms
{
    [HarmonyPatch(typeof(Mission), "GetAttackCollisionResults")]
    public class BreakablePolearmsMissionBehavior : MissionBehavior
    {
        private static Agent _attacker;
        private static Agent _victim;
        private static float _hitSpeed;
        private readonly Dictionary<Agent, int> _weaponHitPoints = new Dictionary<Agent, int>();
        private readonly List<MissionWeapon> _brokenWeapons = new List<MissionWeapon>();
        private readonly bool[] _hasDisplayedHitPoints = new bool[3];
        private SoundEvent _breakSound;

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        private static void Postfix(CombatLogData __result, Agent attackerAgent, Agent victimAgent)
        {
            _attacker = attackerAgent;
            _victim = victimAgent;
            _hitSpeed = __result.HitSpeed;
        }

        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            if (agent.IsHuman)
            {
                _weaponHitPoints.Add(agent, 0);
            }
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow) => _weaponHitPoints.Remove(affectedAgent);

        // Determine what types of polearms to deal damage to.
        // Initialize a polearm's HP based on its handling.
        // If a polearm hits an agent, deal damage to the polearm equal to 1 times the damage absorbed by armor.
        // If a polearm hits a shield or an entity, deal damage to the polearm equal to 10 times the damage inflicted.
        // Increase damage to the polearm based on relative movement speed.
        // Decrease damage to the polearm based on the wielder's Polearm skill.
        public override void OnMeleeHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
        {
            int affectorWeaponSlotOrMissileIndex = collisionData.AffectorWeaponSlotOrMissileIndex;
            MissionWeapon weapon = affectorWeaponSlotOrMissileIndex >= 0 ? attacker.Equipment[affectorWeaponSlotOrMissileIndex] : MissionWeapon.Invalid;
            WeaponComponentData currentUsageItem = weapon.CurrentUsageItem;
            BreakablePolearmsSettings settings = BreakablePolearmsSettings.Instance;
            if (currentUsageItem != null && currentUsageItem.IsPolearm && ((currentUsageItem.SwingDamageType == DamageTypes.Invalid && settings.ShouldDamageNonSwingingPolearms) || (currentUsageItem.SwingDamageType != DamageTypes.Invalid && settings.ShouldDamageSwingingPolearms)) && _weaponHitPoints.ContainsKey(attacker))
            {
                int initialHitPoints = currentUsageItem.Handling * (currentUsageItem.SwingDamageType == DamageTypes.Invalid ? settings.NonSwingingPolearmHitPointsMultiplier : settings.SwingingPolearmHitPointsMultiplier);
                int currentHitPoints = _weaponHitPoints[attacker] > 0 ? _weaponHitPoints[attacker] : initialHitPoints;
                int damage = (int)((collisionData.AttackBlockedWithShield || collisionData.EntityExists ? collisionData.InflictedDamage * 10 : collisionData.AbsorbedByArmor) * (attacker.IsMainAgent ? settings.DamageToPolearmsForPlayersMultiplier : settings.DamageToPolearmsForNonPlayersMultiplier));
                if (attacker == _attacker && victim == _victim)
                {
                    damage += (int)(damage * (_hitSpeed * settings.SpeedBasedDamageIncrementToPolearmsMultiplier));
                }
                damage -= (int)(damage * MathF.Min(attacker.Character.GetSkillValue(DefaultSkills.Polearm) * (settings.SkillBasedDamageDecrementToPolearmsMultiplier / 100f), 1f));
                currentHitPoints -= damage;
                _weaponHitPoints[attacker] = currentHitPoints;
                if (currentHitPoints <= 0)
                {
                    _brokenWeapons.Add(weapon);
                }
                if (attacker.IsMainAgent)
                {
                    int percent = MathF.Ceiling(currentHitPoints / (initialHitPoints / 100f));
                    if (percent > 0)
                    {
                        if (percent <= 75 && percent > 50)
                        {
                            if (!_hasDisplayedHitPoints[0])
                            {
                                InformationManager.DisplayMessage(new InformationMessage("Polearm HP: " + percent + "%", Color.Lerp(new Color(0f, 0.75f, 0f, 1f), new Color(0.75f, 0.75f, 0f, 1f), (100 - percent) / 50f)));
                            }
                            _hasDisplayedHitPoints[0] = true;
                        }
                        else if (percent <= 50 && percent > 25)
                        {
                            if (!_hasDisplayedHitPoints[1])
                            {
                                InformationManager.DisplayMessage(new InformationMessage("Polearm HP: " + percent + "%", Color.Lerp(new Color(0.75f, 0.75f, 0f, 1f), new Color(0.75f, 0f, 0f, 1f), (50 - percent) / 50f)));
                            }
                            _hasDisplayedHitPoints[1] = true;
                        }
                        else if (percent <= 25)
                        {
                            if (!_hasDisplayedHitPoints[2])
                            {
                                InformationManager.DisplayMessage(new InformationMessage("Polearm HP: " + percent + "%", Color.Lerp(new Color(0.75f, 0.75f, 0f, 1f), new Color(0.75f, 0f, 0f, 1f), (50 - percent) / 50f)));
                            }
                            _hasDisplayedHitPoints[2] = true;
                        }
                    }
                    else
                    {
                        InformationManager.DisplayMessage(new InformationMessage("Polearm broken!", new Color(0.75f, 0f, 0f, 1f)));
                        _hasDisplayedHitPoints[0] = false;
                        _hasDisplayedHitPoints[1] = false;
                        _hasDisplayedHitPoints[2] = false;
                    }
                }
            }
        }

        // If a polearm is broken, remove it from the wielder and play a breaking sound.
        public override void OnMissionTick(float dt)
        {
            foreach (Agent agent in Mission.Agents)
            {
                if (agent.IsHuman)
                {
                    for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; index++)
                    {
                        MissionWeapon weapon = agent.Equipment[index];
                        if (_brokenWeapons.Contains(weapon))
                        {
                            agent.RemoveEquippedWeapon(index);
                            _brokenWeapons.Remove(weapon);
                            _breakSound = SoundEvent.CreateEvent(SoundEvent.GetEventIdFromString("event:/mission/combat/shield/broken"), Mission.Scene);
                            _breakSound.PlayInPosition(agent.Position);
                        }
                    }
                }
            }
        }
    }
}
