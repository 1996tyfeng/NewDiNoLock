using System;
using System.Collections.Generic;
using UnityEngine;

namespace NewDiNoLock.Rendering
{
    [DisallowMultipleComponent]
    public sealed class FrameAnimationPlayer : MonoBehaviour, IPetAnimationPlayer
    {
        [Serializable]
        private struct AnimationStateMapping
        {
            public string animationName;
            public string animatorStateName;
        }

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private string _defaultSkinId = "frame_default";

        [SerializeField]
        private string _fallbackAnimation = AnimationName.Idle;

        [SerializeField]
        private List<AnimationStateMapping> _stateMappings = new List<AnimationStateMapping>
        {
            new AnimationStateMapping { animationName = AnimationName.Idle, animatorStateName = "Idle" },
            new AnimationStateMapping { animationName = AnimationName.Walk, animatorStateName = "Walk" },
            new AnimationStateMapping { animationName = AnimationName.Lift, animatorStateName = "Drag_Start" },
            new AnimationStateMapping { animationName = AnimationName.DragLoop, animatorStateName = "Drag_Loop" },
            new AnimationStateMapping { animationName = AnimationName.Drop, animatorStateName = "Drag_End" },
            new AnimationStateMapping { animationName = AnimationName.Click, animatorStateName = "Click" },
            new AnimationStateMapping { animationName = AnimationName.Notify, animatorStateName = "Notify" },
            new AnimationStateMapping { animationName = AnimationName.Sleep, animatorStateName = "Sleep" }
        };

        private readonly Dictionary<string, string> _stateLookup = new Dictionary<string, string>(StringComparer.Ordinal);
        private string _skinId;

        public string CurrentAnimation { get; private set; } = AnimationName.Idle;

        public string CurrentSkinId => string.IsNullOrWhiteSpace(_skinId) ? _defaultSkinId : _skinId;

        private void Awake()
        {
            EnsureDependencies();
            RebuildLookup();
            Play(AnimationName.Idle, true);
        }

        private void OnValidate()
        {
            EnsureDependencies();
            RebuildLookup();
        }

        public bool HasAnimation(string animationName)
        {
            if (string.IsNullOrWhiteSpace(animationName))
            {
                return false;
            }

            EnsureLookupReady();
            return TryResolveStateName(animationName, out _);
        }

        public void Play(string animationName, bool loop = false)
        {
            EnsureDependencies();
            EnsureLookupReady();

            var resolvedAnimation = ResolveAnimationName(animationName);
            if (!TryResolveStateName(resolvedAnimation, out var stateName))
            {
                return;
            }

            if (!TryGetAnimatorStateHash(stateName, out var stateHash))
            {
                return;
            }

            CurrentAnimation = resolvedAnimation;
            _animator.Play(stateHash, 0, 0f);
        }

        public void SetFlipX(bool flipX)
        {
            EnsureDependencies();
            _spriteRenderer.flipX = flipX;
        }

        public void SetSkin(string skinId)
        {
            _skinId = string.IsNullOrWhiteSpace(skinId) ? _defaultSkinId : skinId;
        }

        private void EnsureDependencies()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            }

            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>(true);
            }
        }

        private void EnsureLookupReady()
        {
            if (_stateLookup.Count == 0)
            {
                RebuildLookup();
            }
        }

        private void RebuildLookup()
        {
            _stateLookup.Clear();
            for (var index = 0; index < _stateMappings.Count; index++)
            {
                var mapping = _stateMappings[index];
                if (string.IsNullOrWhiteSpace(mapping.animationName) || string.IsNullOrWhiteSpace(mapping.animatorStateName))
                {
                    continue;
                }

                _stateLookup[mapping.animationName] = mapping.animatorStateName;
            }
        }

        private string ResolveAnimationName(string animationName)
        {
            if (HasAnimation(animationName))
            {
                return animationName;
            }

            if ((animationName == AnimationName.Happy || animationName == AnimationName.Notify || animationName == AnimationName.Drop)
                && HasAnimation(AnimationName.Click))
            {
                return AnimationName.Click;
            }

            return HasAnimation(_fallbackAnimation) ? _fallbackAnimation : AnimationName.Idle;
        }

        private bool TryResolveStateName(string animationName, out string stateName)
        {
            if (_stateLookup.TryGetValue(animationName, out stateName))
            {
                return true;
            }

            stateName = animationName;
            return _animator != null && TryGetAnimatorStateHash(stateName, out _);
        }

        private bool TryGetAnimatorStateHash(string stateName, out int stateHash)
        {
            stateHash = Animator.StringToHash(stateName);
            if (_animator != null && _animator.HasState(0, stateHash))
            {
                return true;
            }

            stateHash = Animator.StringToHash($"Base Layer.{stateName}");
            return _animator != null && _animator.HasState(0, stateHash);
        }
    }
}
