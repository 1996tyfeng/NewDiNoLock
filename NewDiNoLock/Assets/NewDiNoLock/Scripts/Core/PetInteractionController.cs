using System;
using System.Collections;
using UnityEngine;

namespace NewDiNoLock.Core
{
    [DisallowMultipleComponent]
    public sealed class PetInteractionController : MonoBehaviour
    {
        [SerializeField]
        private PetBehaviorController _behaviorController;

        [SerializeField]
        private Collider2D _interactionCollider;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private float _dragThresholdPixels = 6f;

        [SerializeField]
        private float _interactionDuration = 1.2f;

        [SerializeField]
        private float _boundsPadding = 0.05f;

        private readonly PetBoundsService _boundsService = new PetBoundsService();
        private Vector3 _pointerDownScreenPosition;
        private Vector3 _dragOffset;
        private bool _isPointerDown;
        private bool _isDragging;
        private Coroutine _interactionRoutine;

        public bool IsDragging => _isDragging;

        public event Action<Vector3> Clicked;

        private void Awake()
        {
            EnsureDependencies();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryBeginPointerDown();
            }

            if (_isPointerDown && Input.GetMouseButton(0))
            {
                UpdatePointerDrag();
            }

            if (_isPointerDown && Input.GetMouseButtonUp(0))
            {
                EndPointer();
            }
        }

        private void TryBeginPointerDown()
        {
            EnsureDependencies();
            if (!IsPointerOverPet(Input.mousePosition))
            {
                return;
            }

            _isPointerDown = true;
            _isDragging = false;
            _pointerDownScreenPosition = Input.mousePosition;
            _dragOffset = transform.position - ScreenToWorld(_pointerDownScreenPosition);
        }

        private void UpdatePointerDrag()
        {
            var dragDistance = Vector2.Distance(Input.mousePosition, _pointerDownScreenPosition);
            if (!_isDragging && dragDistance >= _dragThresholdPixels)
            {
                StopInteractionRoutine();
                _isDragging = _behaviorController.BeginDrag("mouse drag");
            }

            if (_isDragging)
            {
                MoveToPointer(Input.mousePosition);
            }
        }

        private void EndPointer()
        {
            _isPointerDown = false;

            if (_isDragging)
            {
                MoveToPointer(Input.mousePosition);
                transform.position = _boundsService.ClampToCamera(ResolveCamera(), transform, transform.position, _boundsPadding);
                _behaviorController.EndDrag("mouse release");
                _isDragging = false;
                return;
            }

            StartInteraction();
        }

        private bool IsPointerOverPet(Vector3 screenPosition)
        {
            if (_interactionCollider == null)
            {
                return false;
            }

            var worldPosition = ScreenToWorld(screenPosition);
            return _interactionCollider.OverlapPoint(worldPosition);
        }

        private void MoveToPointer(Vector3 screenPosition)
        {
            var pointerWorldPosition = ScreenToWorld(screenPosition);
            var requestedPosition = pointerWorldPosition + _dragOffset;
            var clampedPosition = _boundsService.ClampToCamera(ResolveCamera(), transform, requestedPosition, _boundsPadding);
            transform.position = clampedPosition;

            if ((clampedPosition - requestedPosition).sqrMagnitude > 0.000001f)
            {
                _dragOffset = clampedPosition - pointerWorldPosition;
            }
        }

        private void StartInteraction()
        {
            if (!_behaviorController.RequestInteract("mouse click"))
            {
                return;
            }

            Clicked?.Invoke(transform.position);

            if (_interactionRoutine != null)
            {
                StopInteractionRoutine();
            }

            _interactionRoutine = StartCoroutine(EndInteractionAfterDelay());
        }

        private IEnumerator EndInteractionAfterDelay()
        {
            yield return new WaitForSeconds(_interactionDuration);
            _behaviorController.EndInteraction("interaction timeout");
            _interactionRoutine = null;
        }

        private void StopInteractionRoutine()
        {
            if (_interactionRoutine == null)
            {
                return;
            }

            StopCoroutine(_interactionRoutine);
            _interactionRoutine = null;
        }

        private Vector3 ScreenToWorld(Vector3 screenPosition)
        {
            var camera = ResolveCamera();
            if (camera == null)
            {
                return transform.position;
            }

            var distance = Mathf.Abs(transform.position.z - camera.transform.position.z);
            screenPosition.z = distance;
            var worldPosition = camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = transform.position.z;
            return worldPosition;
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
            if (_behaviorController == null)
            {
                _behaviorController = GetComponent<PetBehaviorController>();
            }

            if (_interactionCollider == null)
            {
                _interactionCollider = GetComponentInChildren<Collider2D>();
            }

            ResolveCamera();
        }
    }
}
