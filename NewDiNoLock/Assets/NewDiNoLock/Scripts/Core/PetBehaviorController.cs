using System;
using NewDiNoLock.Infrastructure;
using NewDiNoLock.Rendering;
using UnityEngine;
using ILogger = NewDiNoLock.Infrastructure.ILogger;

namespace NewDiNoLock.Core
{
    [DisallowMultipleComponent]
    public sealed class PetBehaviorController : MonoBehaviour
    {
        [SerializeField]
        private MonoBehaviour _animationPlayerSource;

        private IEventBus _eventBus;
        private ILogger _logger;
        private IPetAnimationPlayer _animationPlayer;
        private PetStateMachine _stateMachine;
        private IDisposable _stateChangedSubscription;

        public PetState CurrentState => _stateMachine?.CurrentState ?? PetState.Idle;

        public void Configure(IEventBus eventBus, ILogger logger, IPetAnimationPlayer animationPlayer)
        {
            if (_stateMachine != null)
            {
                throw new InvalidOperationException("PetBehaviorController is already initialized. Configure must run before Awake.");
            }

            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _animationPlayer = animationPlayer ?? throw new ArgumentNullException(nameof(animationPlayer));
        }

        private void Awake()
        {
            EnsureDependencies();
            InitializeStateMachineIfNeeded();
        }

        private void OnDestroy()
        {
            _stateChangedSubscription?.Dispose();
            _stateChangedSubscription = null;
        }

        public bool RequestWalk(string reason = null)
        {
            return _stateMachine.TryRequestState(PetState.Walk, reason);
        }

        public bool RequestInteract(string reason = null)
        {
            return _stateMachine.TryRequestState(PetState.Interact, reason);
        }

        public bool RequestSleep(string reason = null)
        {
            return _stateMachine.TryRequestState(PetState.Sleep, reason);
        }

        public bool RequestHide(string reason = null)
        {
            return _stateMachine.TryRequestState(PetState.Hidden, reason);
        }

        public bool RequestNotify(bool isHighPriority, string reason = null)
        {
            return _stateMachine.TryRequestNotify(isHighPriority, reason);
        }

        public bool BeginDrag(string reason = null)
        {
            return _stateMachine.TryRequestState(PetState.Dragged, reason);
        }

        public bool EndDrag(string reason = null)
        {
            return _stateMachine.TryExitState(PetState.Dragged, PetState.Idle, false, reason);
        }

        public bool EndInteraction(string reason = null)
        {
            return _stateMachine.TryExitState(PetState.Interact, PetState.Idle, false, reason);
        }

        public bool DismissNotify(string reason = null)
        {
            return _stateMachine.TryExitState(PetState.Notify, PetState.Idle, false, reason);
        }

        public bool Show(string reason = null)
        {
            return _stateMachine.TryExitState(PetState.Hidden, PetState.Idle, false, reason);
        }

        private void EnsureDependencies()
        {
            if (_eventBus == null)
            {
                _eventBus = new EventBus();
            }

            if (_logger == null)
            {
                _logger = new UnityLoggerAdapter();
            }

            if (_animationPlayer == null)
            {
                _animationPlayer = ResolveAnimationPlayer();
            }
        }

        private void InitializeStateMachineIfNeeded()
        {
            if (_stateMachine != null)
            {
                return;
            }

            _stateMachine = new PetStateMachine(_eventBus, _logger, PetState.Idle);
            _stateChangedSubscription = _eventBus.Subscribe<PetStateChangedEvent>(OnPetStateChanged);
            PlayAnimationForState(_stateMachine.CurrentState);
        }

        private IPetAnimationPlayer ResolveAnimationPlayer()
        {
            if (_animationPlayerSource is IPetAnimationPlayer serializedAnimationPlayer)
            {
                return serializedAnimationPlayer;
            }

            var behaviours = GetComponents<MonoBehaviour>();
            for (var index = 0; index < behaviours.Length; index++)
            {
                if (behaviours[index] is IPetAnimationPlayer animationPlayer)
                {
                    return animationPlayer;
                }
            }

            throw new InvalidOperationException("PetBehaviorController requires a component that implements IPetAnimationPlayer.");
        }

        private void OnPetStateChanged(PetStateChangedEvent eventData)
        {
            PlayAnimationForState(eventData.CurrentState);
        }

        private void PlayAnimationForState(PetState state)
        {
            var animationName = ResolveAnimationName(state);
            var loop = state == PetState.Idle || state == PetState.Walk || state == PetState.Sleep;
            _animationPlayer.Play(animationName, loop);
        }

        private static string ResolveAnimationName(PetState state)
        {
            switch (state)
            {
                case PetState.Idle:
                    return AnimationName.Idle;
                case PetState.Walk:
                    return AnimationName.Walk;
                case PetState.Dragged:
                    return AnimationName.Lift;
                case PetState.Interact:
                    return AnimationName.Click;
                case PetState.Notify:
                    return AnimationName.Notify;
                case PetState.Sleep:
                    return AnimationName.Sleep;
                case PetState.Hidden:
                    return AnimationName.Idle;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, "Unsupported pet state.");
            }
        }

        private sealed class UnityLoggerAdapter : ILogger
        {
            public void Debug(string message)
            {
                UnityEngine.Debug.Log(message);
            }

            public void Info(string message)
            {
                UnityEngine.Debug.Log(message);
            }

            public void Warning(string message)
            {
                UnityEngine.Debug.LogWarning(message);
            }

            public void Error(string message)
            {
                UnityEngine.Debug.LogError(message);
            }

            public void Error(Exception exception, string message = null)
            {
                if (exception == null)
                {
                    Error(message);
                    return;
                }

                if (string.IsNullOrEmpty(message))
                {
                    UnityEngine.Debug.LogError(exception);
                    return;
                }

                UnityEngine.Debug.LogError($"{message}\n{exception}");
            }
        }
    }
}
