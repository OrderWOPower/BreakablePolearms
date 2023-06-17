using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BreakablePolearms
{
    [HarmonyPatch(typeof(Mission), "GetAttackCollisionResults")]
    public class BreakablePolearmsMissionBehavior : MissionBehavior
    {
        private static Agent _attacker, _victim;
        private static float _hitSpeed;

        private readonly Dictionary<Agent, int> _weaponHitPoints;
        private readonly List<MissionWeapon> _brokenWeapons;
        private readonly bool[] _hasDisplayedHitPoints;

        private SoundEvent _breakSound;

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        private static void Postfix(CombatLogData __result, Agent attackerAgent, Agent victimAgent)
        {
            _attacker = attackerAgent;
            _victim = victimAgent;
            _hitSpeed = __result.HitSpeed;
        }

        public BreakablePolearmsMissionBehavior()
        {
            _weaponHitPoints = new Dictionary<Agent, int>();
            _brokenWeapons = new List<MissionWeapon>();
            _hasDisplayedHitPoints = new bool[3];
        }

        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            if (agent.IsHuman)
            {
                _weaponHitPoints.Add(agent, 0);
            }
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow) => _weaponHitPoints.Remove(affectedAgent);

        public override void OnMeleeHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
        {
            int affectorWeaponSlotOrMissileIndex = collisionData.AffectorWeaponSlotOrMissileIndex;
            MissionWeapon weapon = affectorWeaponSlotOrMissileIndex >= 0 ? attacker.Equipment[affectorWeaponSlotOrMissileIndex] : MissionWeapon.Invalid;
            WeaponComponentData currentUsageItem = weapon.CurrentUsageItem;
            BreakablePolearmsSettings settings = BreakablePolearmsSettings.Instance;

            // Determine what types of polearms to deal damage to.
            if (_weaponHitPoints.ContainsKey(attacker) && !weapon.HasAnyUsageWithWeaponClass(WeaponClass.Javelin) && currentUsageItem != null && currentUsageItem.IsPolearm && ((currentUsageItem.SwingDamageType == DamageTypes.Invalid && settings.ShouldDamageNonSwingingPolearms) || (currentUsageItem.SwingDamageType != DamageTypes.Invalid && settings.ShouldDamageSwingingPolearms)))
            {
                // Initialize a polearm's HP based on its handling.
                int initialHitPoints = currentUsageItem.Handling * (currentUsageItem.SwingDamageType == DamageTypes.Invalid ? settings.NonSwingingPolearmHitPointsMultiplier : settings.SwingingPolearmHitPointsMultiplier);
                int currentHitPoints = _weaponHitPoints[attacker] > 0 ? _weaponHitPoints[attacker] : initialHitPoints;
                // If a polearm hits an agent, deal damage to the polearm equal to 1 times the damage absorbed by armor. If a polearm hits a shield or an entity, deal damage to the polearm equal to 10 times the damage inflicted.
                int damageToWeapon = (int)((collisionData.AttackBlockedWithShield || collisionData.EntityExists ? collisionData.InflictedDamage * 10 : collisionData.AbsorbedByArmor) * (attacker.IsMainAgent ? settings.DamageToPolearmsForPlayersMultiplier : settings.DamageToPolearmsForNonPlayersMultiplier));
                
                if (attacker == _attacker && victim == _victim)
                {
                    // Increase damage to the polearm based on relative movement speed.
                    damageToWeapon += (int)(damageToWeapon * (_hitSpeed * settings.SpeedBasedDamageIncrementToPolearmsMultiplier));
                }

                // Decrease damage to the polearm based on the wielder's Polearm skill.
                damageToWeapon -= (int)(damageToWeapon * MathF.Min(attacker.Character.GetSkillValue(DefaultSkills.Polearm) * (settings.SkillBasedDamageDecrementToPolearmsMultiplier / 100f), 1f));
                currentHitPoints -= damageToWeapon;

                _weaponHitPoints[attacker] = currentHitPoints;

                if (currentHitPoints <= 0)
                {
                    _brokenWeapons.Add(weapon);
                }

                if (attacker.IsMainAgent)
                {
                    int hitPointsPercent = MathF.Ceiling(currentHitPoints / (initialHitPoints / 100f));

                    if (hitPointsPercent > 0)
                    {
                        if (hitPointsPercent <= 75 && hitPointsPercent > 50)
                        {
                            if (!_hasDisplayedHitPoints[0])
                            {
                                InformationManager.DisplayMessage(new InformationMessage("Polearm HP: " + hitPointsPercent + "%", Color.Lerp(new Color(0f, 0.75f, 0f, 1f), new Color(0.75f, 0.75f, 0f, 1f), (100 - hitPointsPercent) / 50f)));
                            }

                            _hasDisplayedHitPoints[0] = true;
                        }
                        else if (hitPointsPercent <= 50 && hitPointsPercent > 25)
                        {
                            if (!_hasDisplayedHitPoints[1])
                            {
                                InformationManager.DisplayMessage(new InformationMessage("Polearm HP: " + hitPointsPercent + "%", Color.Lerp(new Color(0.75f, 0.75f, 0f, 1f), new Color(0.75f, 0f, 0f, 1f), (50 - hitPointsPercent) / 50f)));
                            }

                            _hasDisplayedHitPoints[1] = true;
                        }
                        else if (hitPointsPercent <= 25)
                        {
                            if (!_hasDisplayedHitPoints[2])
                            {
                                InformationManager.DisplayMessage(new InformationMessage("Polearm HP: " + hitPointsPercent + "%", Color.Lerp(new Color(0.75f, 0.75f, 0f, 1f), new Color(0.75f, 0f, 0f, 1f), (50 - hitPointsPercent) / 50f)));
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

        public override void OnMissionTick(float dt)
        {
            foreach (Agent agent in Mission.Agents.Where(a => a.IsHuman && _brokenWeapons.Contains(a.WieldedWeapon)))
            {
                // If a polearm is broken, remove it from the wielder and play a breaking sound.
                _brokenWeapons.Remove(agent.WieldedWeapon);

                agent.RemoveEquippedWeapon(agent.GetWieldedItemIndex(Agent.HandIndex.MainHand));

                _breakSound = SoundEvent.CreateEvent(SoundEvent.GetEventIdFromString("event:/mission/combat/shield/broken"), Mission.Scene);
                _breakSound.PlayInPosition(agent.Position);
            }
        }

        public override void OnItemDrop(Agent agent, SpawnedItemEntity item)
        {
            MissionWeapon weapon = item.WeaponCopy;
            WeaponComponentData currentUsageItem = weapon.CurrentUsageItem;
            BreakablePolearmsSettings settings = BreakablePolearmsSettings.Instance;

            if (_weaponHitPoints.ContainsKey(agent) && !weapon.HasAnyUsageWithWeaponClass(WeaponClass.Javelin) && currentUsageItem != null && currentUsageItem.IsPolearm && ((currentUsageItem.SwingDamageType == DamageTypes.Invalid && settings.ShouldDamageNonSwingingPolearms) || (currentUsageItem.SwingDamageType != DamageTypes.Invalid && settings.ShouldDamageSwingingPolearms)))
            {
                _weaponHitPoints[agent] = 0;
            }
        }
    }
}
