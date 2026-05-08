using System;
using NewDiNoLock.Infrastructure;
using NUnit.Framework;

namespace NewDiNoLock.Tests.EditMode
{
    public sealed class DisposableGroupTests
    {
        [Test]
        public void Dispose_DisposesAllItems()
        {
            var group = new DisposableGroup();
            var first = new TestDisposable();
            var second = new TestDisposable();

            group.Add(first);
            group.Add(second);
            group.Dispose();

            Assert.IsTrue(first.IsDisposed);
            Assert.IsTrue(second.IsDisposed);
            Assert.AreEqual(0, group.Count);
        }

        [Test]
        public void AddAfterDispose_DisposesItemImmediately()
        {
            var group = new DisposableGroup();
            var disposable = new TestDisposable();

            group.Dispose();
            group.Add(disposable);

            Assert.IsTrue(disposable.IsDisposed);
        }

        private sealed class TestDisposable : IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}
