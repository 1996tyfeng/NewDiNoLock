using NewDiNoLock.Core;
using NewDiNoLock.Rendering;
using NUnit.Framework;
using UnityEngine;

namespace NewDiNoLock.Tests.EditMode
{
    public sealed class PetBehaviorControllerTests
    {
        private GameObject _gameObject;
        private PetBehaviorController _controller;
        private TestAnimationPlayer _animationPlayer;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject("PetBehaviorControllerTests");
            _animationPlayer = _gameObject.AddComponent<TestAnimationPlayer>();
            _controller = _gameObject.AddComponent<PetBehaviorController>();
            _controller.SendMessage("Awake");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void Awake_InitializesIdleStateAndAnimation()
        {
            Assert.AreEqual(PetState.Idle, _controller.CurrentState);
            Assert.AreEqual(AnimationName.Idle, _animationPlayer.CurrentAnimation);
            Assert.IsTrue(_animationPlayer.LastLoopValue);
        }

        [Test]
        public void RequestWalk_PlaysWalkAnimation()
        {
            var changed = _controller.RequestWalk("auto walk");

            Assert.IsTrue(changed);
            Assert.AreEqual(PetState.Walk, _controller.CurrentState);
            Assert.AreEqual(AnimationName.Walk, _animationPlayer.CurrentAnimation);
            Assert.IsTrue(_animationPlayer.LastLoopValue);
        }

        [Test]
        public void EndWalk_WhenWalking_ReturnsToIdle()
        {
            _controller.RequestWalk("auto walk");

            var changed = _controller.EndWalk("arrived");

            Assert.IsTrue(changed);
            Assert.AreEqual(PetState.Idle, _controller.CurrentState);
            Assert.AreEqual(AnimationName.Idle, _animationPlayer.CurrentAnimation);
        }

        [Test]
        public void BeginDrag_PlaysLiftAnimation_AndEndDragPlaysDropAnimation()
        {
            _controller.BeginDrag("drag start");

            Assert.AreEqual(PetState.Dragged, _controller.CurrentState);
            Assert.AreEqual(AnimationName.Lift, _animationPlayer.CurrentAnimation);
            Assert.IsFalse(_animationPlayer.LastLoopValue);

            var changed = _controller.EndDrag("drag end");

            Assert.IsTrue(changed);
            Assert.AreEqual(PetState.Idle, _controller.CurrentState);
            Assert.AreEqual(AnimationName.Drop, _animationPlayer.CurrentAnimation);
            Assert.IsFalse(_animationPlayer.LastLoopValue);
        }

        [Test]
        public void RequestInteract_PlaysClickAnimation()
        {
            _controller.RequestInteract("click");

            Assert.AreEqual(PetState.Interact, _controller.CurrentState);
            Assert.AreEqual(AnimationName.Click, _animationPlayer.CurrentAnimation);
        }

        [Test]
        public void RequestNotify_PlaysNotifyAnimation()
        {
            _controller.RequestNotify(false, "reminder");

            Assert.AreEqual(PetState.Notify, _controller.CurrentState);
            Assert.AreEqual(AnimationName.Notify, _animationPlayer.CurrentAnimation);
        }

        [Test]
        public void RequestSleep_PlaysSleepAnimationLoop()
        {
            _controller.RequestSleep("inactive");

            Assert.AreEqual(PetState.Sleep, _controller.CurrentState);
            Assert.AreEqual(AnimationName.Sleep, _animationPlayer.CurrentAnimation);
            Assert.IsTrue(_animationPlayer.LastLoopValue);
        }

        private sealed class TestAnimationPlayer : MonoBehaviour, IPetAnimationPlayer
        {
            public string CurrentAnimation { get; private set; }

            public bool LastLoopValue { get; private set; }

            public bool HasAnimation(string animationName)
            {
                return true;
            }

            public void Play(string animationName, bool loop = false)
            {
                CurrentAnimation = animationName;
                LastLoopValue = loop;
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
