using System;
using UnityEngine;

namespace NewDiNoLock.System
{
    [Serializable]
    public sealed class AppSettings
    {
        public WindowSettings window = WindowSettings.CreateDefault();
        public PetSettings pet = PetSettings.CreateDefault();
        public SystemSettings system = SystemSettings.CreateDefault();
        public FeatureSettings features = FeatureSettings.CreateDefault();

        public static AppSettings CreateDefault()
        {
            return new AppSettings
            {
                window = WindowSettings.CreateDefault(),
                pet = PetSettings.CreateDefault(),
                system = SystemSettings.CreateDefault(),
                features = FeatureSettings.CreateDefault()
            };
        }

        public AppSettings Clone()
        {
            return JsonUtility.FromJson<AppSettings>(JsonUtility.ToJson(this));
        }

        public void Normalize()
        {
            window ??= WindowSettings.CreateDefault();
            pet ??= PetSettings.CreateDefault();
            system ??= SystemSettings.CreateDefault();
            features ??= FeatureSettings.CreateDefault();

            pet.Normalize();
            system.Normalize();
            features.Normalize();
        }
    }
}
