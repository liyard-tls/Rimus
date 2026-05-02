using Rimus.Scripts.Characters.TargetSelection;
using UnityEngine;

namespace Rimus.Scripts.Characters
{
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private CharacterAnimator _animator;

        private bool _isTargeted;
        private bool _isDead;

        private void Awake()
        {
            _animator ??= GetComponent<CharacterAnimator>();
        }

        public void Initialize()
        {
            RefreshState();
        }

        public void SetTargeted(bool isTargeted)
        {
            _isTargeted = isTargeted;
            RefreshState();
        }

        public void SetDead(bool isDead)
        {
            _isDead = isDead;
            RefreshState();
        }
        
        public void PlayAnimation(string animationName)
        {
            if (_isDead || _animator == null)
            {
                return;
            }

            if (!_animator.TryPlayAnimation(animationName))
            {
                RefreshState();
            }
        }

        public void PlayDamaged()
        {
            if (_isDead || _animator == null)
            {
                return;
            }

            if (!_animator.TryPlayAnimation("hit"))
            {
                RefreshState();
            }
        }

        public void PlayHealed()
        {
            if (_isDead || _animator == null)
            {
                return;
            }

            if (!_animator.TryPlayAnimation("heal"))
            {
                RefreshState();
            }
        }

        private void RefreshState()
        {
            if (_animator == null)
            {
                return;
            }

            if (_isDead && _animator.TryPlayAnimation("dead"))
            {
                return;
            }

            if (_isTargeted && _animator.TryPlayAnimation("targeted"))
            {
                return;
            }

            _animator.TryPlayAnimation("idle");
        }
    }
}
