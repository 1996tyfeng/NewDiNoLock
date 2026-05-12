using NewDiNoLock.Core;
using NewDiNoLock.Rendering;
using NUnit.Framework;
using UnityEngine;

namespace NewDiNoLock.Tests.EditMode
{
    public sealed class PetMovementControllerTests
    {
        private GameObject _gameObject;
        private PetBehaviorController _behaviorController;
        private PetMovementController _movementController;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject("PetMovementControllerTests");
            _gameObject.AddComponent<TestAnimationPlayer>();
            _behaviorController = _gameObject.AddComponent<PetBehaviorController>();
            _movementController = _gameObject.AddComponent<PetMovementController>();
            _behaviorController.SendMessage("Awake");
            _movementController.SendMessage("Awake");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void SetAutoWalkEnabled_WhenDisabledDuringWalk_ReturnsToIdle()
        {
            _behaviorController.RequestWalk("auto walk");

            _movementController.SetAutoWalkEnabled(false);

            Assert.IsFalse(_movementController.AutoWalkEnabled);
            Assert.AreEqual(PetState.Idle, _behaviorController.CurrentState);
        }

        private sealed class TestAnimationPlayer : MonoBehaviour, IPetAnimationPlayer
        {
            public string CurrentAnimation { get; private set; }

            public bool HasAnimation(string animationName)
            {
                return true;
            }

            public void Play(string animationName, bool loop = false)
            {
                CurrentAnimation = animationName;
            }

            public void SetFlipX(bool flipX)
            {
            }

            public void SetSkin(string skinId)
            {
            }
        }
    }
}
