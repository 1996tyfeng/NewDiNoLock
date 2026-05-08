using System;

namespace NewDiNoLock.System
{
    [Serializable]
    public sealed class PetSettings
    {
        public const string DefaultSkinId = "dino_default";

        public bool autoWalkEnabled = true;
        public string currentSkinId = DefaultSkinId;
        public bool restorePositionOnStart = true;
        public PetPosition position;

        public static PetSettings CreateDefault()
        {
            return new PetSettings
            {
                autoWalkEnabled = true,
                currentSkinId = DefaultSkinId,
                restorePositionOnStart = true,
                position = null
            };
        }

        public void Normalize()
        {
            if (string.IsNullOrWhiteSpace(currentSkinId))
            {
                currentSkinId = DefaultSkinId;
            }
        }
    }

    [Serializable]
    public sealed class PetPosition
    {
        public int displayIndex;
        public float x;
        public float y;
    }
}
