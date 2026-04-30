using Rimus.Scripts.Characters.TargetSelection;
using UnityEngine;

namespace Rimus.Scripts.Characters
{
    public class CharacterPresenter : MonoBehaviour
    {
        [SerializeField] private CharacterView _view;
        [SerializeField] private HealthComponent _health;
        [SerializeField] private Targetable _targetable;

        private void Awake()
        {
            _view ??= GetComponent<CharacterView>();
            _health ??= GetComponent<HealthComponent>();
            _targetable ??= GetComponent<Targetable>();

            if (_view != null)
            {
                _view.Initialize();
            }
        }

        private void OnEnable()
        {
            if (_targetable != null)
            {
                _targetable.OnTargeted.AddListener(OnTargetedChanged);
            }

            if (_health != null)
            {
                _health.OnDamaged += OnDamaged;
                _health.OnHealed += OnHealed;
                _health.OnDeath += OnDeath;
                _health.OnRevived += OnRevived;
                _health.OnHpChanged += OnHpChanged;
            }

            if (_health != null)
            {
                _view?.SetDead(_health.IsDead);
            }
        }

        private void OnDisable()
        {
            if (_targetable != null)
            {
                _targetable.OnTargeted.RemoveListener(OnTargetedChanged);
            }

            if (_health != null)
            {
                _health.OnDamaged -= OnDamaged;
                _health.OnHealed -= OnHealed;
                _health.OnDeath -= OnDeath;
                _health.OnRevived -= OnRevived;
                _health.OnHpChanged -= OnHpChanged;
            }
        }

        private void OnTargetedChanged(bool isTargeted)
        {
            _view?.SetTargeted(isTargeted);
        }

        private void OnDamaged(int _)
        {
            _view?.PlayDamaged();
        }

        private void OnHealed(int _)
        {
            _view?.PlayHealed();
        }

        private void OnDeath()
        {
            _view?.SetDead(true);
        }

        private void OnRevived()
        {
            _view?.SetDead(false);
        }

        private void OnHpChanged(int currentHp, int _)
        {
            if (_health == null)
            {
                return;
            }

            _view?.SetDead(currentHp <= 0);
        }
    }
}
