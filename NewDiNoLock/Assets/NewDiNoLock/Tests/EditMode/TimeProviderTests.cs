using System;
using NewDiNoLock.Infrastructure;
using NUnit.Framework;

namespace NewDiNoLock.Tests.EditMode
{
    public sealed class TimeProviderTests
    {
        [Test]
        public void ManualTimeProvider_Advance_UpdatesTimeAndDelta()
        {
            var startTime = new DateTime(2026, 5, 8, 12, 0, 0, DateTimeKind.Utc);
            var timeProvider = new ManualTimeProvider(startTime);

            timeProvider.Advance(TimeSpan.FromSeconds(2.5));

            Assert.AreEqual(startTime.AddSeconds(2.5), timeProvider.UtcNow);
            Assert.AreEqual(2.5f, timeProvider.DeltaTime);
            Assert.AreEqual(2.5f, timeProvider.UnscaledDeltaTime);
        }
    }
}
