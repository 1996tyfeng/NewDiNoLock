using System;
using System.Collections.Generic;
using UnityEngine;

namespace NewDiNoLock.Rendering
{
    [DisallowMultipleComponent]
    public sealed class PlaceholderAnimationPlayer : MonoBehaviour, IPetAnimationPlayer
    {
        [Serializable]
        private struct AnimationEntry
        {
            public string name;
            public Color color;
            public Vector3 scale;
        }

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private List<AnimationEntry> _animations = new List<AnimationEntry>
        {
            new AnimationEntry { name = AnimationName.Idle, color = new Color(0.56f, 0.82f, 0.72f), scale = Vector3.one },
            new AnimationEntry { name = AnimationName.Walk, color = new Color(0.98f, 0.80f, 0.38f), scale = new Vector3(1.08f, 0.96f, 1f) },
            new AnimationEntry { name = AnimationName.Lift, color = new Color(0.95f, 0.65f, 0.48f), scale = new Vector3(0.92f, 1.12f, 1f) },
            new AnimationEntry { name = AnimationName.Click, color = new Color(0.97f, 0.56f, 0.62f), scale = new Vector3(1.12f, 1.12f, 1f) }
        };

        [SerializeField]
        private string _defaultSkinId = "placeholder_default";

        private readonly Dictionary<string, AnimationEntry> _animationLookup = new Dictionary<string, AnimationEntry>(StringComparer.Ordinal);
        private Vector3 _baseScale = Vector3.one;
        private string _skinId;

        public string CurrentAnimation { get; private set; } = AnimationName.Idle;

        public string CurrentSkinId => string.IsNullOrEmpty(_skinId) ? _defaultSkinId : _skinId;

        private void Awake()
        {
            EnsureSpriteRenderer();
            RebuildLookup();
            _baseScale = transform.localScale;
            ApplyAnimation(GetFallbackAnimationName(AnimationName.Idle));
        }

        public bool HasAnimation(string animationName)
        {
            if (string.IsNullOrWhiteSpace(animationName))
            {
                return false;
            }

            EnsureLookupReady();
            return _animationLookup.ContainsKey(animationName);
        }

        public void Play(string animationName, bool loop = false)
        {
            EnsureSpriteRenderer();
            EnsureLookupReady();
            ApplyAnimation(GetFallbackAnimationName(animationName));
        }

        public void SetFlipX(bool flipX)
        {
            EnsureSpriteRenderer();
            _spriteRenderer.flipX = flipX;
        }

        public void SetSkin(string skinId)
        {
            _skinId = string.IsNullOrWhiteSpace(skinId) ? _defaultSkinId : skinId;
        }

        private void OnValidate()
        {
            EnsureSpriteRenderer();
            RebuildLookup();
        }

        private void EnsureLookupReady()
        {
            if (_animationLookup.Count == 0)
            {
                RebuildLookup();
            }
        }

        private void RebuildLookup()
        {
            _animationLookup.Clear();
            for (var index = 0; index < _animations.Count; index++)
            {
                var entry = _animations[index];
                if (string.IsNullOrWhiteSpace(entry.name))
                {
                    continue;
                }

                if (entry.scale == default)
                {
                    entry.scale = Vector3.one;
                }

                _animationLookup[entry.name] = entry;
            }
        }

        private void EnsureSpriteRenderer()
        {
            if (_spriteRenderer != null)
            {
                return;
            }

            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
            {
                _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }

        private string GetFallbackAnimationName(string requestedAnimation)
        {
            if (HasAnimation(requestedAnimation))
            {
                return requestedAnimation;
            }

            if (requestedAnimation == AnimationName.Drop || requestedAnimation == AnimationName.Happy || requestedAnimation == AnimationName.Notify)
            {
                if (HasAnimation(AnimationName.Click))
                {
                    return AnimationName.Click;
                }
            }

            if (HasAnimation(AnimationName.Idle))
            {
                return AnimationName.Idle;
            }

            foreach (var entry in _animations)
            {
                if (!string.IsNullOrWhiteSpace(entry.name))
                {
                    return entry.name;
                }
            }

            return AnimationName.Idle;
        }

        private void ApplyAnimation(string animationName)
        {
            CurrentAnimation = animationName;

            if (!_animationLookup.TryGetValue(animationName, out var entry))
            {
                return;
            }

            _spriteRenderer.color = entry.color;
            transform.localScale = Vector3.Scale(_baseScale, entry.scale);
        }
    }
}
