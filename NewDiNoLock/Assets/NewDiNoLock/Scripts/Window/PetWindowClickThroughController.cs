using UnityEngine;
using UnityEngine.EventSystems;

namespace NewDiNoLock.Window
{
    [DisallowMultipleComponent]
    public sealed class PetWindowClickThroughController : MonoBehaviour
    {
        [SerializeField]
        private Collider2D _interactionCollider;

        [SerializeField]
        private Camera _camera;

        private IDesktopWindowService _windowService;
        private bool _hasAppliedState;
        private bool _lastClickThrough;

        public void Configure(IDesktopWindowService windowService)
        {
            _windowService = windowService;
            ApplyCurrentPointerState(true);
        }

        private void Awake()
        {
            EnsureDependencies();
        }

        private void Update()
        {
            ApplyCurrentPointerState(false);
        }

        private void OnDisable()
        {
            if (_windowService != null)
            {
                _windowService.SetClickThrough(false);
            }

            _hasAppliedState = false;
        }

        private void ApplyCurrentPointerState(bool force)
        {
            if (_windowService == null)
            {
                return;
            }

            EnsureDependencies();
            var pointerOverInteractiveSurface = IsPointerOverPet() || IsPointerOverUi();
            var clickThrough = !pointerOverInteractiveSurface;
            if (!force && _hasAppliedState && clickThrough == _lastClickThrough)
            {
                return;
            }

            if (_windowService.SetClickThrough(clickThrough))
            {
                _lastClickThrough = clickThrough;
                _hasAppliedState = true;
            }
        }

        private bool IsPointerOverPet()
        {
            if (!_lastClickThrough && Input.GetMouseButton(0))
            {
                return true;
            }

            if (_interactionCollider == null || _camera == null)
            {
                return false;
            }

            var screenPosition = GetPointerClientPosition();
            var distance = Mathf.Abs(transform.position.z - _camera.transform.position.z);
            screenPosition.z = distance;
            var worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            return _interactionCollider.OverlapPoint(worldPosition);
        }

        private static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        private Vector3 GetPointerClientPosition()
        {
            if (_windowService != null && _windowService.TryGetPointerClientPosition(out var position))
            {
                return position;
            }

            return Input.mousePosition;
        }

        private void EnsureDependencies()
        {
            if (_interactionCollider == null)
            {
                _interactionCollider = GetComponentInChildren<Collider2D>();
            }

            if (_camera == null)
            {
                _camera = Camera.main != null ? Camera.main : FindObjectOfType<Camera>();
            }
        }
    }
}
