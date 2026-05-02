using Rimus.Scripts.Characters.TargetSelection;
using UnityEngine;

namespace Rimus.Scripts.Characters
{
    public class CharacterEntity : MonoBehaviour
    {
        [SerializeField] private CharacterDefinition _definition;
        [SerializeField] private StatsComponent _stats;
        [SerializeField] private HealthComponent _health;
        [SerializeField] private Targetable _targetable;
        [SerializeField] private AttackSelector _attackSelector;
        [SerializeField] private SkillCaster _skillCaster;
        [SerializeField] private CharacterView _view;
        
        
        [SerializeField] private bool IsControlledByPlayer;

        public CharacterDefinition Definition => _definition;
        public StatsComponent Stats => _stats;
        public HealthComponent Health => _health;
        public Targetable Targetable => _targetable;
        public AttackSelector AttackSelector => _attackSelector;
        public SkillCaster SkillCaster => _skillCaster;
        public CharacterView View => _view;

        private void Awake()
        {
            _stats ??= GetComponent<StatsComponent>();
            _health ??= GetComponent<HealthComponent>();
            _targetable ??= GetComponent<Targetable>();
            _attackSelector ??= GetComponent<AttackSelector>();
            _skillCaster ??= GetComponent<SkillCaster>();

            if (_stats != null && _definition != null)
            {
                _stats.SetDefinition(_definition);
            }
        }

        public void SetDefinition(CharacterDefinition definition)
        {
            _definition = definition;

            if (_stats != null)
            {
                _stats.SetDefinition(definition);
            }

            if (_skillCaster != null)
            {
                _skillCaster.LoadDefaultSkillsFromDefinition();
            }
        }

        public void StartTurn()
        {
            
        }
    }
}
