using System;
using System.Collections.Generic;
using Rimus.Scripts.Tools;
using UnityEngine;

namespace Rimus.Scripts.Characters
{
    [Serializable]
    public struct CharacterAnimation
    {
        public string Id;
        public float Duration;
        public bool Loop;
        public Sprite[] Sprites;
    }
    
    public class CharacterAnimator : MonoBehaviour
    {
        public CharacterAnimation[] Animations => _animations;
        
        [SerializeField] private SpriteAnimator _spriteAnimator;
        [SerializeField] private CharacterAnimation[] _animations;
        
        private Dictionary<string, CharacterAnimation> _animationDict;

        private void Awake()
        {
            _spriteAnimator ??= GetComponent<SpriteAnimator>();
            _animationDict = new Dictionary<string, CharacterAnimation>();
            foreach (var anim in _animations)
            {
                _animationDict.TryAdd(anim.Id, anim);
            }
        }
        
        public void PlayAnimation(string animationId)
        {
            if (animationId == null)
            {
                Log.Error($"Animation ID cannot be null.");
                return;
            }
            if (!_animationDict.TryGetValue(animationId, out var animation))
            {
                Log.Error($"Animation with ID '{animationId}' not found.");
                return;
            }
            _spriteAnimator.Play(animation);
        }

        public bool HasAnimation(string animationId)
        {
            return animationId != null && _animationDict != null && _animationDict.ContainsKey(animationId);
        }

        public bool TryPlayAnimation(string animationId)
        {
            if (!HasAnimation(animationId))
            {
                return false;
            }

            PlayAnimation(animationId);
            return true;
        }
    }
}
