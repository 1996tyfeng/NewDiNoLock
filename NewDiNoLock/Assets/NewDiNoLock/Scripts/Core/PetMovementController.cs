using NewDiNoLock.Rendering;
using UnityEngine;

namespace NewDiNoLock.Core
{
    [DisallowMultipleComponent]
    public sealed class PetMovementController : MonoBehaviour
    {
        [SerializeField]
        private PetBehaviorController _behaviorController;

        [SerializeField]
        private MonoBehaviour _animationPlayerSource;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private bool _autoWalkEnabled = true;

        [SerializeField]
        private float _minWalkInterval = 8f;

        [SerializeField]
        private float _maxWalkInterval = 20f;

        [SerializeField]
        private float _walkSpeed = 1.2f;

        [SerializeField]
        private float _arriveDistance = 0.03f;

        [SerializeField]
        private float _boundsPadding = 0.05f;

        private readonly PetBoundsService _boundsService = new PetBoundsService();
        private IPetAnimationPlayer _animationPlayer;
        private Vector3 _targetPosition;
        private float _nextWalkAt;
        private bool _hasTarget;
        private bool _autoWalkPaused;
        private bool _wasDraggedLastFrame;

        public bool AutoWalkEnabled => _autoWalkEnabled;
        public bool AutoWalkPaused => _autoWalkPaused;
        public bool IsWalking => _hasTarget;
        public Vector3 TargetPosition => _targetPosition;

        private void Awake()
        {
            EnsureDependencies();
            ScheduleNextWalk();
        }

        private void Update()
        {
            EnsureDependencies();
            if (_behaviorController == null)
            {
                return;
            }

            if (_behaviorController.CurrentState == PetState.Dragged)
            {
                if (!_wasDraggedLastFrame)
                {
                    CancelWalkForDrag();
                }

                _wasDraggedLastFrame = true;
                return;
            }

            if (_wasDraggedLastFrame)
            {
                _wasDraggedLastFrame = false;
                ScheduleNextWalk();
            }

            transform.position = _boundsService.ClampToCamera(ResolveCamera(), transform, transform.position, _boundsPadding);

            if (!_autoWalkEnabled || _autoWalkPaused)
            {
                StopWalkingIfNeeded();
                return;
            }

            if (_behaviorController.CurrentState == PetState.Walk && _hasTarget)
            {
                MoveTowardTarget();
                return;
            }

            _hasTarget = false;

            if (_behaviorController.CurrentState != PetState.Idle || Time.time < _nextWalkAt)
            {
                return;
            }

            StartRandomWalk();
        }

        public void SetAutoWalkEnabled(bool enabled)
        {
            _autoWalkEnabled = enabled;
            if (!enabled)
            {
                StopWalkingIfNeeded();
                return;
            }

            ScheduleNextWalk();
        }

        public void SetAutoWalkPaused(bool paused)
        {
            if (_autoWalkPaused == paused)
            {
                return;
            }

            _autoWalkPaused = paused;
            if (paused)
            {
                StopWalkingIfNeeded();
                return;
            }

            if (_autoWalkEnabled)
            {
                ScheduleNextWalk();
            }
        }

        public void ForceWalkTarget(Vector3 targetPosition)
        {
            EnsureDependencies();
            if (_behaviorController == null || _autoWalkPaused)
            {
                return;
            }

            _targetPosition = _boundsService.ClampToCamera(ResolveCamera(), transform, targetPosition, _boundsPadding);
            _hasTarget = _behaviorController.RequestWalk("forced walk target");
        }

        private void StartRandomWalk()
        {
            if (_behaviorController == null)
            {
                return;
            }

            _targetPosition = _boundsService.PickRandomPointInCamera(ResolveCamera(), transform, _boundsPadding);
            _hasTarget = _behaviorController.RequestWalk("auto walk");
        }

        private void MoveTowardTarget()
        {
            var current = transform.position;
            var next = Vector3.MoveTowards(current, _targetPosition, _walkSpeed * Time.deltaTime);
            transform.position = _boundsService.ClampToCamera(ResolveCamera(), transform, next, _boundsPadding);

            var delta = _targetPosition - current;
            if (Mathf.Abs(delta.x) > 0.001f)
            {
                _animationPlayer?.SetFlipX(delta.x < 0f);
            }

            if (Vector3.Distance(transform.position, _targetPosition) <= _arriveDistance)
            {
                _hasTarget = false;
                _behaviorController.EndWalk("arrived");
                ScheduleNextWalk();
            }
        }

        private void StopWalkingIfNeeded()
        {
            _hasTarget = false;
            if (_behaviorController != null && _behaviorController.CurrentState == PetState.Walk)
            {
                _behaviorController.EndWalk("auto walk disabled");
            }
        }

        private void CancelWalkForDrag()
        {
            _hasTarget = false;
        }

        private void ScheduleNextWalk()
        {
            var minInterval = Mathf.Max(0f, _minWalkInterval);
            var maxInterval = Mathf.Max(minInterval, _maxWalkInterval);
            _nextWalkAt = Time.time + Random.Range(minInterval, maxInterval);
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

            if (_animationPlayer == null)
            {
                _animationPlayer = ResolveAnimationPlayer();
            }

            ResolveCamera();
        }

        private IPetAnimationPlayer ResolveAnimationPlayer()
        {
            if (_animationPlayerSource is IPetAnimationPlayer serializedAnimationPlayer)
            {
                return serializedAnimationPlayer;
            }

            var behaviours = GetComponentsInChildren<MonoBehaviour>(true);
            for (var index = 0; index < behaviours.Length; index++)
            {
                if (behaviours[index] is IPetAnimationPlayer animationPlayer)
                {
                    return animationPlayer;
                }
            }

            return null;
        }
    }
}
