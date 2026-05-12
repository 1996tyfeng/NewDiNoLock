using NewDiNoLock.Rendering;
using NUnit.Framework;
using UnityEngine;

namespace NewDiNoLock.Tests.EditMode
{
    public sealed class FrameAnimationPlayerTests
    {
        private GameObject _gameObject;
        private FrameAnimationPlayer _player;
        private SpriteRenderer _spriteRenderer;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject("FrameAnimationPlayerTests");
            _spriteRenderer = _gameObject.AddComponent<SpriteRenderer>();
            _gameObject.AddComponent<Animator>();
            _player = _gameObject.AddComponent<FrameAnimationPlayer>();
            _player.SendMessage("Awake");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void HasAnimation_ReturnsTrueForMappedAnimations()
        {
            Assert.IsTrue(_player.HasAnimation(AnimationName.Idle));
            Assert.IsTrue(_player.HasAnimation(AnimationName.Walk));
            Assert.IsTrue(_player.HasAnimation(AnimationName.Lift));
            Assert.IsTrue(_player.HasAnimation(AnimationName.Drop));
        }

        [Test]
        public void SetFlipX_UpdatesSpriteRendererOrientation()
        {
            _player.SetFlipX(true);

            Assert.IsTrue(_spriteRenderer.flipX);
        }

        [Test]
        public void SetSkin_WhenEmpty_UsesDefaultSkin()
        {
            _player.SetSkin(string.Empty);

            Assert.AreEqual("frame_default", _player.CurrentSkinId);
        }
    }
}
