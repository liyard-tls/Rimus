using System;
using System.Collections.Generic;
using Rimus.Scripts.Characters.TargetSelection;
using UnityEngine;

namespace Rimus.Scripts.Characters
{
    public class SkillCaster : MonoBehaviour
    {
        [SerializeField] private CharacterEntity _entity;
        [SerializeField] private AttackSelector _attackSelector;
        [SerializeField] private StatsComponent _stats;
        [SerializeField] private List<SkillDefinition> _knownSkills = new List<SkillDefinition>();
        [SerializeField] private SkillDefinition _currentSkill;

        public event Action<SkillDefinition, IReadOnlyList<Targetable>> OnSkillCast;

        public IReadOnlyList<SkillDefinition> KnownSkills => _knownSkills;
        public SkillDefinition CurrentSkill => _currentSkill;

        private void Awake()
        {
            _entity ??= GetComponent<CharacterEntity>();
            _attackSelector ??= GetComponent<AttackSelector>();
            _stats ??= GetComponent<StatsComponent>();

            LoadDefaultSkillsFromDefinition();

            if (_currentSkill != null && _attackSelector != null)
            {
                _attackSelector.SetSelectorType(_currentSkill.SelectorType);
            }
        }

        public void LoadDefaultSkillsFromDefinition()
        {
            _entity ??= GetComponent<CharacterEntity>();
            _attackSelector ??= GetComponent<AttackSelector>();
            _stats ??= GetComponent<StatsComponent>();

            if (_entity == null || _entity.Definition == null)
            {
                return;
            }

            _knownSkills.Clear();
            IReadOnlyList<SkillDefinition> defaultSkills = _entity.Definition.DefaultSkills;
            for (int i = 0; i < defaultSkills.Count; i++)
            {
                SkillDefinition skill = defaultSkills[i];
                if (skill != null)
                {
                    _knownSkills.Add(skill);
                }
            }

            if (_knownSkills.Count > 0 && _currentSkill == null)
            {
                SelectSkill(_knownSkills[0]);
            }
        }

        public void SelectSkill(SkillDefinition skill)
        {
            _currentSkill = skill;

            if (_attackSelector != null && _currentSkill != null)
            {
                _attackSelector.SetSelectorType(_currentSkill.SelectorType);
            }
        }

        public bool TryCastCurrentSkill()
        {
            if (_currentSkill == null || _attackSelector == null)
            {
                return false;
            }

            IReadOnlyList<Targetable> selectedTargets = _attackSelector.SelectedTargets;
            if (selectedTargets == null || selectedTargets.Count == 0)
            {
                return false;
            }

            List<Targetable> affectedTargets = new List<Targetable>();
            int maxTargets = Mathf.Max(1, _currentSkill.MaxTargets);

            for (int i = 0; i < selectedTargets.Count && affectedTargets.Count < maxTargets; i++)
            {
                Targetable targetable = selectedTargets[i];
                if (targetable == null)
                {
                    continue;
                }

                HealthComponent targetHealth = targetable.GetComponentInParent<HealthComponent>();
                if (targetHealth == null)
                {
                    continue;
                }

                ApplySkillToTarget(targetHealth, _currentSkill);
                affectedTargets.Add(targetable);
            }

            if (affectedTargets.Count == 0)
            {
                return false;
            }

            OnSkillCast?.Invoke(_currentSkill, affectedTargets);

            if (_currentSkill.ClearSelectionAfterCast)
            {
                _attackSelector.ClearSelection();
            }

            return true;
        }

        private void ApplySkillToTarget(HealthComponent targetHealth, SkillDefinition skill)
        {
            int finalPower = skill.Power;

            if (_stats != null)
            {
                CharacterStats currentStats = _stats.CurrentStats;
                if (skill.AddAttackStatToPower)
                {
                    finalPower += currentStats.Attack;
                }

                if (skill.AddMagicAttackStatToPower)
                {
                    finalPower += currentStats.MagicAttack;
                }
            }

            finalPower = Mathf.Max(0, finalPower);

            switch (skill.EffectType)
            {
                case SkillEffectType.Heal:
                    targetHealth.Heal(finalPower);
                    break;
                default:
                    targetHealth.TakeDamage(finalPower);
                    break;
            }
        }
    }
}
