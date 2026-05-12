using NewDiNoLock.Core;
using NUnit.Framework;
using UnityEngine;

namespace NewDiNoLock.Tests.EditMode
{
    public sealed class PetBoundsServiceTests
    {
        [Test]
        public void ClampPoint_KeepsWholePetInsideBounds()
        {
            var bounds = Rect.MinMaxRect(-5f, -3f, 5f, 3f);
            var clamped = PetBoundsService.ClampPoint(bounds, new Vector2(10f, -10f), new Vector2(1f, 0.5f));

            Assert.AreEqual(4f, clamped.x);
            Assert.AreEqual(-2.5f, clamped.y);
        }

        [Test]
        public void ClampPoint_WhenPetIsWiderThanBounds_UsesCenter()
        {
            var bounds = Rect.MinMaxRect(-1f, -1f, 1f, 1f);
            var clamped = PetBoundsService.ClampPoint(bounds, new Vector2(10f, 0.5f), new Vector2(5f, 0.25f));

            Assert.AreEqual(0f, clamped.x);
            Assert.AreEqual(0.5f, clamped.y);
        }

        [Test]
        public void ClampPosition_UsesTargetBoundsCenterOffset()
        {
            var bounds = Rect.MinMaxRect(-5f, -3f, 5f, 3f);
            var currentPosition = new Vector3(0f, 0f, 0f);
            var targetPosition = new Vector3(10f, -10f, 0f);
            var petBounds = new Bounds(new Vector3(0.5f, -0.25f, 0f), new Vector3(2f, 1f, 0f));

            var clamped = PetBoundsService.ClampPosition(bounds, currentPosition, targetPosition, petBounds);

            Assert.AreEqual(3.5f, clamped.x);
            Assert.AreEqual(-2.25f, clamped.y);
        }
    }
}
