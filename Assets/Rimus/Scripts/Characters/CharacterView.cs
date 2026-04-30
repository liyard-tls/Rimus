using System;
using Rimus.Scripts.Characters.TargetSelection;
using UnityEngine;

namespace Rimus.Scripts.Characters
{
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private CharacterAnimator _animator;
        [SerializeField] private Targetable _targetable;

        void Start() => Initialize();
        
        public void Initialize()
        {
            _animator ??= GetComponent<CharacterAnimator>();
            _targetable ??= GetComponent<Targetable>();
            
            _animator.PlayAnimation("idle");

            _targetable.OnTargeted.AddListener(OnTargeted);
        }

        private void OnDestroy()
        {
            _targetable.OnTargeted.RemoveListener(OnTargeted);
        }

        private void OnTargeted(bool isTargeted)
        {
            if (isTargeted)
            {
                _animator.PlayAnimation("targeted");
            }
            else
            {
                _animator.PlayAnimation("idle");
            }
        }
    }
}