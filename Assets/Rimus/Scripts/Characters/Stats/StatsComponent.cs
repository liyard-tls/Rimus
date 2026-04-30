using System;
using UnityEngine;

namespace Rimus.Scripts.Characters
{
    public class StatsComponent : MonoBehaviour
    {
        [SerializeField] private CharacterDefinition _definition;
        [SerializeField] private bool _useDefinitionBaseStats = true;
        [SerializeField] private CharacterStats _baseStatsOverride = CharacterStats.Default;
        [SerializeField] private CharacterStats _flatBonusStats;
        [SerializeField] private float _multiplicativeBonus = 1f;

        private CharacterStats _currentStats;

        public event Action<CharacterStats> OnStatsChanged;

        public CharacterDefinition Definition => _definition;
        public CharacterStats CurrentStats => _currentStats;
        public CharacterStats BaseStats => ResolveBaseStats();

        private void Awake()
        {
            RecalculateStats();
        }

        private void OnValidate()
        {
            _multiplicativeBonus = Mathf.Max(0f, _multiplicativeBonus);
            RecalculateStats();
        }

        public void SetDefinition(CharacterDefinition definition)
        {
            _definition = definition;
            RecalculateStats();
        }

        public void SetFlatBonusStats(CharacterStats flatBonusStats)
        {
            _flatBonusStats = flatBonusStats;
            RecalculateStats();
        }

        public void SetMultiplicativeBonus(float multiplicativeBonus)
        {
            _multiplicativeBonus = Mathf.Max(0f, multiplicativeBonus);
            RecalculateStats();
        }

        public void RecalculateStats()
        {
            CharacterStats resolvedBaseStats = ResolveBaseStats();
            CharacterStats finalStats = CharacterStats.Scale(resolvedBaseStats + _flatBonusStats, _multiplicativeBonus).ClampMinimums();
            _currentStats = finalStats;
            OnStatsChanged?.Invoke(_currentStats);
        }

        private CharacterStats ResolveBaseStats()
        {
            if (_useDefinitionBaseStats && _definition != null)
            {
                return _definition.BaseStats;
            }

            return _baseStatsOverride;
        }
    }
}
