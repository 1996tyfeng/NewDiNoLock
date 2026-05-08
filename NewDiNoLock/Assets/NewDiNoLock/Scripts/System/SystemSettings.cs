using System;
using UnityEngine;

namespace NewDiNoLock.System
{
    [Serializable]
    public sealed class SystemSettings
    {
        public bool keepAwakeEnabled;
        public float volume = 0.5f;
        public bool interactionBubbleEnabled = true;

        public static SystemSettings CreateDefault()
        {
            return new SystemSettings
            {
                keepAwakeEnabled = false,
                volume = 0.5f,
                interactionBubbleEnabled = true
            };
        }

        public void Normalize()
        {
            volume = Mathf.Clamp01(volume);
        }
    }
}
