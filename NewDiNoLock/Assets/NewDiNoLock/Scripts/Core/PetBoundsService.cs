using UnityEngine;

namespace NewDiNoLock.Core
{
    public sealed class PetBoundsService
    {
        public Vector3 ClampToCamera(Camera camera, Transform target, Vector3 worldPosition, float padding = 0f)
        {
            if (camera == null || target == null)
            {
                return worldPosition;
            }

            var bounds = ResolveTargetBounds(target);
            var cameraBounds = ResolveCameraBounds(camera, worldPosition.z);
            return ClampPosition(cameraBounds, target.position, worldPosition, bounds, padding);
        }

        public Vector3 PickRandomPointInCamera(Camera camera, Transform target, float padding = 0f)
        {
            if (camera == null || target == null)
            {
                return target != null ? target.position : Vector3.zero;
            }

            var bounds = ResolveTargetBounds(target);
            var cameraBounds = ResolveCameraBounds(camera, target.position.z);
            var centerOffset = bounds.center - target.position;
            var minX = cameraBounds.xMin + bounds.extents.x + padding - centerOffset.x;
            var maxX = cameraBounds.xMax - bounds.extents.x - padding - centerOffset.x;
            var minY = cameraBounds.yMin + bounds.extents.y + padding - centerOffset.y;
            var maxY = cameraBounds.yMax - bounds.extents.y - padding - centerOffset.y;

            if (minX > maxX)
            {
                var center = cameraBounds.center.x;
                minX = center;
                maxX = center;
            }

            if (minY > maxY)
            {
                var center = cameraBounds.center.y;
                minY = center;
                maxY = center;
            }

            return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), target.position.z);
        }

        public static Vector2 ClampPoint(Rect bounds, Vector2 point, Vector2 extents, float padding = 0f)
        {
            var minX = bounds.xMin + Mathf.Max(0f, extents.x) + padding;
            var maxX = bounds.xMax - Mathf.Max(0f, extents.x) - padding;
            var minY = bounds.yMin + Mathf.Max(0f, extents.y) + padding;
            var maxY = bounds.yMax - Mathf.Max(0f, extents.y) - padding;

            if (minX > maxX)
            {
                var center = bounds.center.x;
                minX = center;
                maxX = center;
            }

            if (minY > maxY)
            {
                var center = bounds.center.y;
                minY = center;
                maxY = center;
            }

            return new Vector2(Mathf.Clamp(point.x, minX, maxX), Mathf.Clamp(point.y, minY, maxY));
        }

        public static Vector3 ClampPosition(Rect bounds, Vector3 currentPosition, Vector3 targetPosition, Bounds targetBounds, float padding = 0f)
        {
            var centerOffset = targetBounds.center - currentPosition;
            var targetCenter = targetPosition + centerOffset;
            var clampedCenter = ClampPoint(bounds, targetCenter, targetBounds.extents, padding);
            return new Vector3(clampedCenter.x - centerOffset.x, clampedCenter.y - centerOffset.y, targetPosition.z);
        }

        private static Bounds ResolveTargetBounds(Transform target)
        {
            var collider = target.GetComponentInChildren<Collider2D>();
            if (collider != null)
            {
                return collider.bounds;
            }

            var renderer = target.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                return renderer.bounds;
            }

            return new Bounds(target.position, Vector3.one);
        }

        private static Rect ResolveCameraBounds(Camera camera, float targetZ)
        {
            if (camera.orthographic)
            {
                var halfHeight = camera.orthographicSize;
                var halfWidth = halfHeight * camera.aspect;
                var center = camera.transform.position;
                return new Rect(center.x - halfWidth, center.y - halfHeight, halfWidth * 2f, halfHeight * 2f);
            }

            var distance = Mathf.Abs(targetZ - camera.transform.position.z);
            var bottomLeft = camera.ScreenToWorldPoint(new Vector3(0f, 0f, distance));
            var topRight = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, distance));
            return Rect.MinMaxRect(bottomLeft.x, bottomLeft.y, topRight.x, topRight.y);
        }
    }
}
