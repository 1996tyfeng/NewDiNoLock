using NewDiNoLock.Rendering;
using NUnit.Framework;
using UnityEngine;

namespace NewDiNoLock.Tests.EditMode
{
    public sealed class PlaceholderAnimationPlayerTests
    {
        private GameObject _gameObject;
        private PlaceholderAnimationPlayer _player;
        private SpriteRenderer _spriteRenderer;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject("PlaceholderAnimationPlayerTests");
            _player = _gameObject.AddComponent<PlaceholderAnimationPlayer>();
            _player.SendMessage("Awake");
            _spriteRenderer = _gameObject.GetComponent<SpriteRenderer>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void Play_WhenAnimationExists_UsesRequestedAnimation()
        {
            _player.Play(AnimationName.Walk, true);

            Assert.AreEqual(AnimationName.Walk, _player.CurrentAnimation);
        }

        [Test]
        public void Play_WhenOptionalAnimationMissing_FallsBackToClickBeforeIdle()
        {
            _player.Play(AnimationName.Notify);

            Assert.AreEqual(AnimationName.Click, _player.CurrentAnimation);
        }

        [Test]
        public void Play_WhenAnimationMissing_FallsBackToIdle()
        {
            _player.Play("UnknownAnimation");

            Assert.AreEqual(AnimationName.Idle, _player.CurrentAnimation);
        }

        [Test]
        public void HasAnimation_ReturnsTrueForPlaceholderSupportedAnimations()
        {
            Assert.IsTrue(_player.HasAnimation(AnimationName.Idle));
            Assert.IsTrue(_player.HasAnimation(AnimationName.Walk));
            Assert.IsTrue(_player.HasAnimation(AnimationName.Lift));
            Assert.IsTrue(_player.HasAnimation(AnimationName.Click));
            Assert.IsFalse(_player.HasAnimation(AnimationName.Sleep));
        }

        [Test]
        public void SetFlipX_UpdatesSpriteRendererOrientation()
        {
            _player.SetFlipX(true);

            Assert.IsTrue(_spriteRenderer.flipX);
        }

        [Test]
        public void SetSkin_WhenSkinIdEmpty_UsesDefaultSkinId()
        {
            _player.SetSkin(string.Empty);

            Assert.AreEqual("placeholder_default", _player.CurrentSkinId);
        }
    }
}
