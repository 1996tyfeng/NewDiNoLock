using System;
using NewDiNoLock.Bootstrap;
using NUnit.Framework;

namespace NewDiNoLock.Tests.EditMode
{
    public sealed class AppLifecycleTests
    {
        [Test]
        public void Shutdown_RunsActionsAndDisposesResourcesInReverseOrder()
        {
            var lifecycle = new AppLifecycle();
            var callOrder = string.Empty;

            lifecycle.RegisterShutdownAction(() => callOrder += "A");
            lifecycle.RegisterShutdownAction(() => callOrder += "B");
            lifecycle.RegisterDisposable(new TestDisposable(() => callOrder += "C"));
            lifecycle.RegisterDisposable(new TestDisposable(() => callOrder += "D"));

            lifecycle.MarkStarted();
            lifecycle.Shutdown();

            Assert.IsTrue(lifecycle.IsShutdown);
            Assert.IsFalse(lifecycle.IsRunning);
            Assert.AreEqual("DCBA", callOrder);
        }

        [Test]
        public void Shutdown_IsIdempotent()
        {
            var lifecycle = new AppLifecycle();
            var callCount = 0;

            lifecycle.RegisterShutdownAction(() => callCount++);
            lifecycle.Shutdown();
            lifecycle.Shutdown();

            Assert.AreEqual(1, callCount);
        }

        private sealed class TestDisposable : IDisposable
        {
            private readonly Action _onDispose;

            public TestDisposable(Action onDispose)
            {
                _onDispose = onDispose;
            }

            public void Dispose()
            {
                _onDispose();
            }
        }
    }
}
