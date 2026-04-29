using System;
using System.Collections;
using Rimus.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace Rimus.Scripts.Tools
{
    public class SpriteAnimator : MonoBehaviour
    {
        public Sprite[] sprites = Array.Empty<Sprite>();
        public float duration = 1f;
        public bool loop = true;
        public float delay;
        public bool randomizeStart;
        public bool randomSpriteStart;
        public float maxRandomDelay = 0.5f;
        public float waitBetweenLoops;
        public bool playOnAwake = true;
        public Action OnAnimationEnd;
        public Action OnCompleteAction;

        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Image _image;
        private Coroutine _animationCoroutine;
        private Coroutine _specialAnimationCoroutine;
        private int _randomSpriteIndex;

        private bool IsUi => _image != null;
        private bool HasTargetRenderer => _spriteRenderer != null || _image != null;
        private float FrameDuration => sprites.Length > 0 ? duration / sprites.Length : 0f;

        private void Awake()
        {
            _spriteRenderer ??= GetComponent<SpriteRenderer>();
            _image ??= GetComponent<Image>();
        }

        private void Start()
        {
            if (playOnAwake)
            {
                Play();
            }
        }

        public void Play(CharacterAnimation animation)
        {
            loop = animation.Loop;
            Play(animation.Sprites, animation.Duration);
        }

        private void ChangeSprites(Sprite[] animationSprites)
        {
            sprites = animationSprites ?? Array.Empty<Sprite>();
        }

        private void Play()
        {
            Stop();

            if (!CanPlay(sprites))
            {
                return;
            }

            if (sprites.Length == 1)
            {
                SetSprite(sprites[0]);
                return;
            }

            if (gameObject.activeInHierarchy && enabled)
            {
                _animationCoroutine = StartCoroutine(Animate());
            }
        }

        private SpriteAnimator Play(Sprite[] animation, float duration = default)
        {
            ChangeSprites(animation);

            if (duration > 0f)
            {
                this.duration = duration;
            }

            Play();
            return this;
        }

        public void OnComplete(Action action)
        {
            OnAnimationEnd = action;
        }

        public void Stop()
        {
            StopTrackedCoroutine(ref _animationCoroutine);
            StopTrackedCoroutine(ref _specialAnimationCoroutine);
        }

        public void PlaySpecial(Sprite[] specialSprites, float duration)
        {
            Stop();

            if (!CanPlay(specialSprites))
            {
                return;
            }

            _specialAnimationCoroutine = StartCoroutine(SpecialAnimation(specialSprites, duration, Play));
        }

        private void OnEnable()
        {
            Play();
        }

        private void OnDisable()
        {
            Stop();
        }

        private bool CanPlay(Sprite[] animationSprites)
        {
            return animationSprites != null && animationSprites.Length > 0 && HasTargetRenderer;
        }

        private void SetSprite(Sprite sprite)
        {
            if (IsUi)
            {
                _image.sprite = sprite;
                return;
            }

            _spriteRenderer.sprite = sprite;
        }

        private IEnumerator Animate()
        {
            yield return WaitForInitialDelay();

            if (randomSpriteStart)
            {
                _randomSpriteIndex = UnityEngine.Random.Range(0, sprites.Length);
            }

            while (true)
            {
                yield return PlayCurrentSequence();

                OnAnimationEnd?.Invoke();

                if (OnCompleteAction != null)
                {
                    OnCompleteAction.Invoke();
                    OnCompleteAction = null;
                }

                if (!loop)
                {
                    _animationCoroutine = null;
                    yield break;
                }

                if (waitBetweenLoops > 0f)
                {
                    yield return new WaitForSeconds(waitBetweenLoops);
                }
            }
        }

        private IEnumerator PlayCurrentSequence()
        {
            if (randomSpriteStart)
            {
                for (var index = _randomSpriteIndex; index < sprites.Length; index++)
                {
                    SetSprite(sprites[index]);
                    yield return new WaitForSeconds(FrameDuration);
                }

                _randomSpriteIndex = 0;
                yield break;
            }

            foreach (var sprite in sprites)
            {
                SetSprite(sprite);
                yield return new WaitForSeconds(FrameDuration);
            }
        }

        private IEnumerator SpecialAnimation(Sprite[] specialSprites, float specialDuration, Action callback = null)
        {
            var frameDuration = specialDuration / specialSprites.Length;

            foreach (var sprite in specialSprites)
            {
                SetSprite(sprite);
                yield return new WaitForSeconds(frameDuration);
            }

            _specialAnimationCoroutine = null;
            callback?.Invoke();
        }

        private IEnumerator WaitForInitialDelay()
        {
            if (randomizeStart)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(0f, maxRandomDelay));
                yield break;
            }

            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }
        }

        private void StopTrackedCoroutine(ref Coroutine coroutine)
        {
            if (coroutine == null)
            {
                return;
            }

            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}
