using Rimus.Scripts.Characters.TargetSelection;
using Rimus.Scripts.Tools.GameContentSystem;
using UnityEngine;

namespace Rimus.Scripts.Characters
{
    [CreateAssetMenu(menuName = "Rimus/Characters/Skill Definition")]
    public class SkillDefinition : ScriptableObject, IGameContent
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private AttackSelectorType _selectorType = AttackSelectorType.RadialSector;
        [SerializeField] private SkillEffectType _effectType = SkillEffectType.Damage;
        [SerializeField, Min(0)] private int _power = 1;
        [SerializeField] private bool _addAttackStatToPower = true;
        [SerializeField] private bool _addMagicAttackStatToPower;
        [SerializeField, Min(1)] private int _maxTargets = 1;
        [SerializeField] private bool _clearSelectionAfterCast = true;

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string DisplayName => string.IsNullOrWhiteSpace(_displayName) ? name : _displayName;
        public AttackSelectorType SelectorType => _selectorType;
        public SkillEffectType EffectType => _effectType;
        public int Power => _power;
        public bool AddAttackStatToPower => _addAttackStatToPower;
        public bool AddMagicAttackStatToPower => _addMagicAttackStatToPower;
        public int MaxTargets => _maxTargets;
        public bool ClearSelectionAfterCast => _clearSelectionAfterCast;
    }
}
