using System;
using NewDiNoLock.Window;
using UnityEngine;
using UnityEngine.UI;

namespace NewDiNoLock.UI
{
    [DisallowMultipleComponent]
    public sealed class RightClickMenuButtonElement : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private DesktopMenuCommand _command;

        public DesktopMenuCommand Command => _command;

        public event Action<DesktopMenuCommand> Clicked;

        private void Awake()
        {
            RegisterButton();
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(NotifyClicked);
            }
        }

        private void OnValidate()
        {
            if (_button == null)
            {
                _button = GetComponentInChildren<Button>(true);
            }
        }

        private void RegisterButton()
        {
            if (_button == null)
            {
                _button = GetComponentInChildren<Button>(true);
            }

            if (_button == null)
            {
                return;
            }

            _button.onClick.RemoveListener(NotifyClicked);
            _button.onClick.AddListener(NotifyClicked);
        }

        private void NotifyClicked()
        {
            Clicked?.Invoke(_command);
        }
    }
}
