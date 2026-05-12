using System;
using NewDiNoLock.Bootstrap;
using NewDiNoLock.System;
using NewDiNoLock.Window;
using NUnit.Framework;
using UnityEngine;

namespace NewDiNoLock.Tests.EditMode
{
    public sealed class DesktopWindowServiceTests
    {
        [Test]
        public void Apply_WhenHandleExists_AppliesTransparentBorderlessStyle()
        {
            var adapter = new FakeWindowPlatformAdapter();
            var service = new DesktopWindowService(adapter, new TestLogger());

            var result = service.Apply(WindowSettings.CreateDefault());

            Assert.IsTrue(result);
            Assert.AreEqual(1, adapter.ApplyTransparentBorderlessCount);
            Assert.AreEqual(adapter.Handle, adapter.LastStyleHandle);
        }

        [Test]
        public void Apply_WhenAlwaysOnTopDisabled_UsesNormalZOrder()
        {
            var adapter = new FakeWindowPlatformAdapter();
            var service = new DesktopWindowService(adapter, new TestLogger());

            service.Apply(new WindowSettings
            {
                alwaysOnTop = false,
                showAboveFullscreen = true
            });

            Assert.AreEqual(WindowZOrderMode.Normal, adapter.LastZOrderMode);
        }

        [Test]
        public void Apply_WhenAlwaysOnTopEnabled_UsesTopmostZOrder()
        {
            var adapter = new FakeWindowPlatformAdapter();
            var service = new DesktopWindowService(adapter, new TestLogger());

            service.Apply(new WindowSettings
            {
                alwaysOnTop = true,
                showAboveFullscreen = false
            });

            Assert.AreEqual(WindowZOrderMode.Topmost, adapter.LastZOrderMode);
        }

        [Test]
        public void Apply_WhenShowAboveFullscreenEnabled_UsesTopmostZOrder()
        {
            var adapter = new FakeWindowPlatformAdapter();
            var service = new DesktopWindowService(adapter, new TestLogger());

            service.Apply(new WindowSettings
            {
                alwaysOnTop = true,
                showAboveFullscreen = true
            });

            Assert.AreEqual(WindowZOrderMode.Topmost, adapter.LastZOrderMode);
        }

        [Test]
        public void Apply_WhenHandleIsMissing_ReturnsFalseAndSkipsWindowCalls()
        {
            var adapter = new FakeWindowPlatformAdapter
            {
                GetHandleResult = false
            };
            var service = new DesktopWindowService(adapter, new TestLogger());

            var result = service.Apply(WindowSettings.CreateDefault());

            Assert.IsFalse(result);
            Assert.AreEqual(0, adapter.ApplyTransparentBorderlessCount);
            Assert.AreEqual(0, adapter.SetZOrderCount);
        }

        [Test]
        public void SetClickThrough_DelegatesToPlatformAdapter()
        {
            var adapter = new FakeWindowPlatformAdapter();
            var service = new DesktopWindowService(adapter, new TestLogger());

            var result = service.SetClickThrough(true);

            Assert.IsTrue(result);
            Assert.AreEqual(1, adapter.SetClickThroughCount);
            Assert.IsTrue(adapter.LastClickThroughValue);
        }

        [Test]
        public void SetClickThrough_AfterApply_RestoresAppliedZOrder()
        {
            var adapter = new FakeWindowPlatformAdapter();
            var service = new DesktopWindowService(adapter, new TestLogger());
            service.Apply(new WindowSettings
            {
                alwaysOnTop = true,
                showAboveFullscreen = false
            });
            adapter.ResetCallCounts();

            var result = service.SetClickThrough(true);

            Assert.IsTrue(result);
            Assert.AreEqual(1, adapter.SetClickThroughCount);
            Assert.AreEqual(1, adapter.SetZOrderCount);
            Assert.AreEqual(WindowZOrderMode.Topmost, adapter.LastZOrderMode);
        }

        [Test]
        public void SetVisible_DelegatesToPlatformAdapter()
        {
            var adapter = new FakeWindowPlatformAdapter();
            var service = new DesktopWindowService(adapter, new TestLogger());

            var result = service.SetVisible(false);

            Assert.IsTrue(result);
            Assert.AreEqual(1, adapter.SetVisibleCount);
            Assert.IsFalse(adapter.LastVisibleValue);
        }

        [Test]
        public void SetVisible_WhenShowingAfterApply_RestoresAppliedZOrder()
        {
            var adapter = new FakeWindowPlatformAdapter();
            var service = new DesktopWindowService(adapter, new TestLogger());
            service.Apply(new WindowSettings
            {
                alwaysOnTop = true,
                showAboveFullscreen = false
            });
            adapter.ResetCallCounts();

            var result = service.SetVisible(true);

            Assert.IsTrue(result);
            Assert.AreEqual(1, adapter.SetVisibleCount);
            Assert.AreEqual(1, adapter.SetZOrderCount);
            Assert.AreEqual(WindowZOrderMode.Topmost, adapter.LastZOrderMode);
        }

        [Test]
        public void CreateDefaultServices_RegistersDesktopWindowService()
        {
            var lifecycle = new AppLifecycle();
            using var services = AppBootstrapper.CreateDefaultServices(lifecycle);

            Assert.IsTrue(services.TryResolve<IDesktopWindowService>(out var service));
            Assert.IsNotNull(service);
            Assert.IsTrue(services.TryResolve<IDesktopMenuService>(out var menuService));
            Assert.IsNotNull(menuService);
        }

        private sealed class FakeWindowPlatformAdapter : IWindowPlatformAdapter
        {
            public readonly PlatformWindowHandle Handle = new PlatformWindowHandle(new IntPtr(42));

            public bool GetHandleResult = true;
            public int ApplyTransparentBorderlessCount { get; private set; }
            public int SetZOrderCount { get; private set; }
            public int SetClickThroughCount { get; private set; }
            public int SetVisibleCount { get; private set; }
            public PlatformWindowHandle LastStyleHandle { get; private set; }
            public WindowZOrderMode LastZOrderMode { get; private set; }
            public bool LastClickThroughValue { get; private set; }
            public bool LastVisibleValue { get; private set; } = true;

            public bool TryGetMainWindowHandle(out PlatformWindowHandle handle)
            {
                handle = GetHandleResult ? Handle : PlatformWindowHandle.None;
                return GetHandleResult;
            }

            public bool TryApplyTransparentBorderless(PlatformWindowHandle handle)
            {
                ApplyTransparentBorderlessCount++;
                LastStyleHandle = handle;
                return true;
            }

            public bool TrySetZOrder(PlatformWindowHandle handle, WindowZOrderMode mode)
            {
                SetZOrderCount++;
                LastZOrderMode = mode;
                return true;
            }

            public bool TrySetClickThrough(PlatformWindowHandle handle, bool enabled)
            {
                SetClickThroughCount++;
                LastClickThroughValue = enabled;
                return true;
            }

            public bool TrySetVisible(PlatformWindowHandle handle, bool visible)
            {
                SetVisibleCount++;
                LastVisibleValue = visible;
                return true;
            }

            public bool TryGetCursorClientPosition(PlatformWindowHandle handle, out Vector2 position)
            {
                position = Vector2.zero;
                return true;
            }

            public void ResetCallCounts()
            {
                ApplyTransparentBorderlessCount = 0;
                SetZOrderCount = 0;
                SetClickThroughCount = 0;
                SetVisibleCount = 0;
            }
        }

        private sealed class TestLogger : NewDiNoLock.Infrastructure.ILogger
        {
            public void Debug(string message)
            {
            }

            public void Info(string message)
            {
            }

            public void Warning(string message)
            {
            }

            public void Error(string message)
            {
            }

            public void Error(Exception exception, string message = null)
            {
            }
        }
    }
}
