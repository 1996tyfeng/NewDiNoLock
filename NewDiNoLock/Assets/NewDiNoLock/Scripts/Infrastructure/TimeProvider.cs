using System;
using UnityEngine;

namespace NewDiNoLock.Infrastructure
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        float DeltaTime { get; }
        float UnscaledDeltaTime { get; }
    }

    public sealed class TimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public float DeltaTime => Time.deltaTime;
        public float UnscaledDeltaTime => Time.unscaledDeltaTime;
    }

    public sealed class ManualTimeProvider : ITimeProvider
    {
        public ManualTimeProvider(DateTime startTime)
        {
            Now = startTime;
            UtcNow = startTime.Kind == DateTimeKind.Utc ? startTime : startTime.ToUniversalTime();
        }

        public DateTime Now { get; private set; }
        public DateTime UtcNow { get; private set; }
        public float DeltaTime { get; private set; }
        public float UnscaledDeltaTime { get; private set; }

        public void Advance(TimeSpan timeSpan)
        {
            Now = Now.Add(timeSpan);
            UtcNow = UtcNow.Add(timeSpan);
            DeltaTime = (float)timeSpan.TotalSeconds;
            UnscaledDeltaTime = DeltaTime;
        }

        public void SetDeltaTime(float deltaTime)
        {
            DeltaTime = deltaTime;
            UnscaledDeltaTime = deltaTime;
        }
    }
}
