using System;
using NewDiNoLock.Infrastructure;

namespace NewDiNoLock.System
{
    public interface ISettingsService
    {
        AppSettings Current { get; }
        AppSettings Load();
        void Save(AppSettings settings);
        void Update(Action<AppSettings> updateSettings);
    }

    public readonly struct SettingsChanged
    {
        public SettingsChanged(AppSettings settings)
        {
            Settings = settings;
        }

        public AppSettings Settings { get; }
    }

    public sealed class SettingsService : ISettingsService
    {
        private readonly AppPaths _paths;
        private readonly ILocalStorageService _storage;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        public SettingsService(AppPaths paths, ILocalStorageService storage, IEventBus eventBus, ILogger logger)
        {
            _paths = paths ?? throw new ArgumentNullException(nameof(paths));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public AppSettings Current { get; private set; }

        public AppSettings Load()
        {
            if (!_storage.FileExists(_paths.SettingsFilePath))
            {
                Current = AppSettings.CreateDefault();
                SaveInternal(Current);
                return Current;
            }

            try
            {
                var json = _storage.ReadAllText(_paths.SettingsFilePath);
                var loadedSettings = UnityEngine.JsonUtility.FromJson<AppSettings>(json);
                if (loadedSettings == null)
                {
                    throw new InvalidOperationException("Settings file produced no data.");
                }

                loadedSettings.Normalize();
                Current = loadedSettings;
                return Current;
            }
            catch (Exception exception)
            {
                _logger.Warning($"Settings file is invalid. Falling back to defaults. Path: {_paths.SettingsFilePath}. Error: {exception.Message}");
                Current = AppSettings.CreateDefault();
                SaveInternal(Current);
                return Current;
            }
        }

        public void Save(AppSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            settings.Normalize();
            Current = settings;
            SaveInternal(Current);
            _eventBus.Publish(new SettingsChanged(Current.Clone()));
        }

        public void Update(Action<AppSettings> updateSettings)
        {
            if (updateSettings == null)
            {
                throw new ArgumentNullException(nameof(updateSettings));
            }

            var settings = Current?.Clone() ?? Load().Clone();
            updateSettings(settings);
            Save(settings);
        }

        private void SaveInternal(AppSettings settings)
        {
            _storage.EnsureDirectory(_paths.DataRoot);
            var json = UnityEngine.JsonUtility.ToJson(settings, true);
            _storage.WriteAllText(_paths.SettingsFilePath, json);
        }
    }
}
