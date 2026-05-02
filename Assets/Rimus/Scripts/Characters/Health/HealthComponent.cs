using System;
using UnityEngine;

namespace Rimus.Scripts.Characters
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private StatsComponent _stats;
        [SerializeField] private bool _initializeToMaxOnAwake = true;

        private int _currentHp;
        private bool _isDead;

        public event Action<int, int> OnHpChanged;
        public event Action<int> OnDamaged;
        public event Action<int> OnHealed;
        public event Action OnDeath;
        public event Action OnRevived;

        public int CurrentHp => _currentHp;
        public int MaxHp => _stats != null ? _stats.CurrentStats.MaxHp : 0;
        public bool IsDead => _isDead;
        public bool IsAlive => !_isDead;
        public float NormalizedHp => MaxHp <= 0 ? 0f : _currentHp / (float)MaxHp;

        private void Awake()
        {
            _stats ??= GetComponent<StatsComponent>();

            if (_stats != null)
            {
                _stats.OnStatsChanged += OnStatsChanged;
            }

            if (_initializeToMaxOnAwake)
            {
                RestoreToFull();
            }
            else
            {
                ClampToMaxHp();
            }
        }

        private void OnDestroy()
        {
            if (_stats != null)
            {
                _stats.OnStatsChanged -= OnStatsChanged;
            }
        }

        public void RestoreToFull()
        {
            _isDead = false;
            _currentHp = MaxHp;
            NotifyHpChanged();
        }

        public void TakeDamage(int amount)
        {
            if (_isDead || amount <= 0)
            {
                return;
            }

            _currentHp = Mathf.Max(0, _currentHp - amount);
            OnDamaged?.Invoke(amount);
            NotifyHpChanged();

            if (_currentHp == 0)
            {
                _isDead = true;
                OnDeath?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || MaxHp <= 0)
            {
                return;
            }

            bool wasDead = _isDead;
            _isDead = false;
            _currentHp = Mathf.Clamp(_currentHp + amount, 0, MaxHp);
            OnHealed?.Invoke(amount);
            NotifyHpChanged();

            if (wasDead && _currentHp > 0)
            {
                OnRevived?.Invoke();
            }
        }

        public void SetCurrentHp(int value)
        {
            bool wasDead = _isDead;
            _currentHp = Mathf.Clamp(value, 0, MaxHp);
            _isDead = _currentHp <= 0;
            NotifyHpChanged();

            if (!wasDead && _isDead)
            {
                OnDeath?.Invoke();
            }
            else if (wasDead && !_isDead)
            {
                OnRevived?.Invoke();
            }
        }

        private void OnStatsChanged(CharacterStats _)
        {
            ClampToMaxHp();
            NotifyHpChanged();
        }

        private void ClampToMaxHp()
        {
            _currentHp = Mathf.Clamp(_currentHp, 0, MaxHp);
            _isDead = _currentHp <= 0 && MaxHp > 0;
        }

        private void NotifyHpChanged()
        {
            OnHpChanged?.Invoke(_currentHp, MaxHp);
        }
    }
}
