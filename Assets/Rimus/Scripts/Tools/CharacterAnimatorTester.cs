using System;
using System.Collections.Generic;
using Rimus.Scripts.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rimus.Scripts.Tools
{
    public class CharacterAnimatorTester : MonoBehaviour
    {
        [SerializeField] private CharacterAnimator _animator;

        private CharacterAnimation[] _animations;
        private int _currentAnimationIndex;

        private void Awake()
        {
            _animator ??= GetComponent<CharacterAnimator>();
            _animations = _animator.Animations;
        }
        
        [Button]
        public void PlayAnimation()
        {
            if (_animations == null || _animations.Length == 0)
            {
                Log.Error("No animations found in CharacterAnimator.");
                return;
            }
            
            var animation = _animations[_currentAnimationIndex];
            Log.Info($"Playing animation: {animation.Id}");
            _animator.PlayAnimation(animation.Id);
        }

        [Button]
        public void NextAnimation()
        {
            if (_animations == null || _animations.Length == 0)
            {
                Log.Error("No animations found in CharacterAnimator.");
                return;
            }
            
            _currentAnimationIndex = (_currentAnimationIndex + 1) % _animations.Length;
            var animation = _animations[_currentAnimationIndex];
            Log.Info($"Playing animation: {animation.Id}");
            _animator.PlayAnimation(animation.Id);
        }

        [Button]
        public void PreviousAnimation()
        {
            if (_animations == null || _animations.Length == 0)
            {
                Log.Error("No animations found in CharacterAnimator.");
                return;
            }
            
            _currentAnimationIndex = (_currentAnimationIndex - 1 + _animations.Length) % _animations.Length;
            var animation = _animations[_currentAnimationIndex];
            Log.Info($"Playing animation: {animation.Id}");
            _animator.PlayAnimation(animation.Id);
        }
    }
}