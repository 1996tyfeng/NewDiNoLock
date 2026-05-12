using System;
using UnityEngine;
using UnityEngine.UI;

namespace NewDiNoLock.UI
{
    [DisallowMultipleComponent]
    public sealed class CommonToggleElement : MonoBehaviour
    {
        [SerializeField]
        private bool _value;

        [SerializeField]
        private bool _leftIsTrue = true;

        [SerializeField]
        private float _slideSpeed = 12f;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private RectTransform _node;

        [SerializeField]
        private GameObject _enableBackground;

        [SerializeField]
        private GameObject _disableBackground;

        private Coroutine _slideRoutine;

        public bool Value
        {
            get => _value;
            set => SetValue(value, false, false);
        }

        public bool LeftIsTrue => _leftIsTrue;
        public float SlideSpeed => _slideSpeed;

        public event Action<bool> ValueChanged;

        private void Awake()
        {
            EnsureReferences();
            RegisterButton();
            ApplyState(false);
        }

        private void OnEnable()
        {
            ApplyState(false);
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(Toggle);
            }
        }

        private void OnValidate()
        {
            EnsureReferences();
        }

        public void SetDefaultValue(bool value)
        {
            SetValue(value, false, false);
        }

        public void SetValue(bool value)
        {
            SetValue(value, true, true);
        }

        public void SetValue(bool value, bool animated)
        {
            SetValue(value, animated, true);
        }

        public void Toggle()
        {
            SetValue(!_value, true, true);
        }

        private void SetValue(bool value, bool animated, bool notify)
        {
            if (_value == value)
            {
                if (!animated && _slideRoutine == null)
                {
                    ApplyState(false);
                }

                return;
            }

            _value = value;
            ApplyState(animated);

            if (notify)
            {
                ValueChanged?.Invoke(_value);
            }
        }

        private void ApplyState(bool animated)
        {
            EnsureReferences();
            ApplyBackground();
            MoveNode(animated);
        }

        private void ApplyBackground()
        {
            if (_enableBackground != null)
            {
                _enableBackground.SetActive(_value);
            }

            if (_disableBackground != null)
            {
                _disableBackground.SetActive(!_value);
            }
        }

        private void MoveNode(bool animated)
        {
            if (_node == null)
            {
                return;
            }

            if (_slideRoutine != null)
            {
                StopCoroutine(_slideRoutine);
                _slideRoutine = null;
            }

            var targetAnchorX = ResolveTargetAnchorX();
            if (!animated || !isActiveAndEnabled)
            {
                ApplyNodeAnchor(targetAnchorX);
                return;
            }

            _slideRoutine = StartCoroutine(AnimateNode(targetAnchorX));
        }

        private global::System.Collections.IEnumerator AnimateNode(float targetAnchorX)
        {
            var startAnchorMin = _node.anchorMin;
            var startAnchorMax = _node.anchorMax;
            var startPivot = _node.pivot;
            var target = new Vector2(targetAnchorX, 0.5f);
            var duration = 1f / Mathf.Max(0.01f, _slideSpeed);
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                t = Mathf.SmoothStep(0f, 1f, t);
                _node.anchorMin = Vector2.Lerp(startAnchorMin, target, t);
                _node.anchorMax = Vector2.Lerp(startAnchorMax, target, t);
                _node.pivot = Vector2.Lerp(startPivot, target, t);
                _node.anchoredPosition = Vector2.zero;
                yield return null;
            }

            ApplyNodeAnchor(targetAnchorX);
            _slideRoutine = null;
        }

        private float ResolveTargetAnchorX()
        {
            var shouldBeLeft = _leftIsTrue ? _value : !_value;
            return shouldBeLeft ? 0f : 1f;
        }

        private void ApplyNodeAnchor(float anchorX)
        {
            var anchor = new Vector2(anchorX, 0.5f);
            _node.anchorMin = anchor;
            _node.anchorMax = anchor;
            _node.pivot = anchor;
            _node.anchoredPosition = Vector2.zero;
        }

        private void RegisterButton()
        {
            if (_button == null)
            {
                return;
            }

            _button.onClick.RemoveListener(Toggle);
            _button.onClick.AddListener(Toggle);
        }

        private void EnsureReferences()
        {
            if (_button == null)
            {
                _button = GetComponentInChildren<Button>(true);
            }

            if (_node == null)
            {
                var nodeTransform = transform.Find("img_ToggleNode") as RectTransform;
                _node = nodeTransform;
            }

            if (_enableBackground == null)
            {
                var enableTransform = transform.Find("img_ToggleBgEnable");
                _enableBackground = enableTransform != null ? enableTransform.gameObject : null;
            }

            if (_disableBackground == null)
            {
                var disableTransform = transform.Find("img_ToggleBgDisable");
                _disableBackground = disableTransform != null ? disableTransform.gameObject : null;
            }
        }
    }
}
