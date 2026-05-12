using UnityEngine;

namespace NewDiNoLock.UI
{
    [DisallowMultipleComponent]
    public sealed class ContextMenuView : MonoBehaviour
    {
        [SerializeField]
        private Collider2D _interactionCollider;

        [SerializeField]
        private Camera _camera;

        private SettingsMenuController _menuController;

        public void Configure(SettingsMenuController menuController)
        {
            _menuController = menuController;
            EnsureDependencies();
        }

        private void Awake()
        {
            EnsureDependencies();
        }

        private void Update()
        {
            if (_menuController == null || !Input.GetMouseButtonDown(1))
            {
                return;
            }

            EnsureDependencies();
            if (IsPointerOverPet(Input.mousePosition))
            {
                _menuController.ShowContextMenu(Input.mousePosition);
            }
        }

        private bool IsPointerOverPet(Vector3 screenPosition)
        {
            if (_interactionCollider == null)
            {
                return false;
            }

            var camera = ResolveCamera();
            if (camera == null)
            {
                return false;
            }

            var distance = Mathf.Abs(transform.position.z - camera.transform.position.z);
            screenPosition.z = distance;
            var worldPosition = camera.ScreenToWorldPoint(screenPosition);
            return _interactionCollider.OverlapPoint(worldPosition);
        }

        private Camera ResolveCamera()
        {
            if (_camera == null)
            {
                _camera = Camera.main != null ? Camera.main : FindObjectOfType<Camera>();
            }

            return _camera;
        }

        private void EnsureDependencies()
        {
            if (_interactionCollider == null)
            {
                _interactionCollider = GetComponentInChildren<Collider2D>();
            }

            ResolveCamera();
        }
    }
}
