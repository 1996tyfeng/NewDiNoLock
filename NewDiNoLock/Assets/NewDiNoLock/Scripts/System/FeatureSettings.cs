using System;

namespace NewDiNoLock.System
{
    [Serializable]
    public sealed class FeatureSettings
    {
        public FeatureToggleSettings pomodoro = FeatureToggleSettings.CreateDisabled();
        public FeatureToggleSettings healthReminder = FeatureToggleSettings.CreateDisabled();
        public FeatureToggleSettings todoReminder = FeatureToggleSettings.CreateDisabled();

        public static FeatureSettings CreateDefault()
        {
            return new FeatureSettings
            {
                pomodoro = FeatureToggleSettings.CreateDisabled(),
                healthReminder = FeatureToggleSettings.CreateDisabled(),
                todoReminder = FeatureToggleSettings.CreateDisabled()
            };
        }

        public void Normalize()
        {
            pomodoro ??= FeatureToggleSettings.CreateDisabled();
            healthReminder ??= FeatureToggleSettings.CreateDisabled();
            todoReminder ??= FeatureToggleSettings.CreateDisabled();
        }
    }

    [Serializable]
    public sealed class FeatureToggleSettings
    {
        public bool enabled;

        public static FeatureToggleSettings CreateDisabled()
        {
            return new FeatureToggleSettings { enabled = false };
        }
    }
}
