using System;
using NewDiNoLock.Window;
using UnityEngine;

namespace NewDiNoLock.UI
{
    [DisallowMultipleComponent]
    public sealed class RightClickMenuToggleElement : MonoBehaviour
    {
        [SerializeField]
        private CommonToggleElement _toggle;

        [SerializeField]
        private DesktopMenuCommand _command;

        public DesktopMenuCommand Command => _command;

        public bool Value
        {
            get => _toggle != null && _toggle.Value;
            set
            {
                EnsureReference();
                _toggle?.SetDefaultValue(value);
            }
        }

        public event Action<DesktopMenuCommand, bool> ValueChanged;

        private void Awake()
        {
            RegisterToggle();
        }

        private void OnDestroy()
        {
            if (_toggle != null)
            {
                _toggle.ValueChanged -= NotifyValueChanged;
            }
        }

        private void OnValidate()
        {
            EnsureReference();
        }

        public void SetDefaultValue(bool value)
        {
            Value = value;
        }

        private void RegisterToggle()
        {
            EnsureReference();
            if (_toggle == null)
            {
                return;
            }

            _toggle.ValueChanged -= NotifyValueChanged;
            _toggle.ValueChanged += NotifyValueChanged;
        }

        private void NotifyValueChanged(bool value)
        {
            ValueChanged?.Invoke(_command, value);
        }

        private void EnsureReference()
        {
            if (_toggle == null)
            {
                _toggle = GetComponentInChildren<CommonToggleElement>(true);
            }
        }
    }
}
