using System;
using NewDiNoLock.Bootstrap;
using NUnit.Framework;

namespace NewDiNoLock.Tests.EditMode
{
    public sealed class ServiceRegistryTests
    {
        [Test]
        public void Resolve_ReturnsRegisteredService()
        {
            var registry = new ServiceRegistry();
            var service = new TestService();

            registry.Register<ITestService>(service);

            Assert.AreSame(service, registry.Resolve<ITestService>());
        }

        [Test]
        public void Resolve_WhenMissing_Throws()
        {
            var registry = new ServiceRegistry();

            Assert.Throws<InvalidOperationException>(() => registry.Resolve<ITestService>());
        }

        [Test]
        public void Dispose_DisposesOwnedServices()
        {
            var registry = new ServiceRegistry();
            var service = new TestService();

            registry.Register<ITestService>(service);
            registry.Dispose();

            Assert.IsTrue(service.IsDisposed);
        }

        private interface ITestService
        {
        }

        private sealed class TestService : ITestService, IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}
