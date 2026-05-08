using System;
using System.IO;
using NewDiNoLock.Infrastructure;
using NewDiNoLock.System;
using NUnit.Framework;

namespace NewDiNoLock.Tests.EditMode
{
    public sealed class SettingsServiceTests
    {
        private string _testRoot;

        [SetUp]
        public void SetUp()
        {
            _testRoot = Path.Combine(Path.GetTempPath(), "NewDiNoLockSettingsTests", Guid.NewGuid().ToString("N"));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_testRoot))
            {
                Directory.Delete(_testRoot, true);
            }
        }

        [Test]
        public void Load_WhenSettingsFileMissing_CreatesDefaultSettingsFile()
        {
            var service = CreateService(out var paths, out _);

            var settings = service.Load();

            Assert.IsTrue(File.Exists(paths.SettingsFilePath));
            Assert.IsTrue(settings.window.alwaysOnTop);
            Assert.IsFalse(settings.window.showAboveFullscreen);
            Assert.IsTrue(settings.window.autoHideInFullscreen);
            Assert.IsTrue(settings.pet.autoWalkEnabled);
            Assert.AreEqual(PetSettings.DefaultSkinId, settings.pet.currentSkinId);
            Assert.IsTrue(settings.pet.restorePositionOnStart);
            Assert.IsFalse(settings.system.keepAwakeEnabled);
            Assert.AreEqual(0.5f, settings.system.volume);
            Assert.IsTrue(settings.system.interactionBubbleEnabled);
        }

        [Test]
        public void Save_ThenLoad_PersistsChangedSettings()
        {
            var service = CreateService(out _, out _);
            var settings = service.Load();

            settings.window.alwaysOnTop = false;
            settings.window.showAboveFullscreen = true;
            settings.pet.currentSkinId = "night_dino";
            settings.system.volume = 0.25f;
            service.Save(settings);

            var secondService = CreateService(out _, out _);
            var loadedSettings = secondService.Load();

            Assert.IsFalse(loadedSettings.window.alwaysOnTop);
            Assert.IsTrue(loadedSettings.window.showAboveFullscreen);
            Assert.AreEqual("night_dino", loadedSettings.pet.currentSkinId);
            Assert.AreEqual(0.25f, loadedSettings.system.volume);
        }

        [Test]
        public void Load_WhenSettingsFileIsInvalid_FallsBackToDefaultsAndLogsWarning()
        {
            var service = CreateService(out var paths, out var logger);
            Directory.CreateDirectory(paths.DataRoot);
            File.WriteAllText(paths.SettingsFilePath, "{ invalid json");

            var settings = service.Load();

            Assert.IsTrue(settings.window.alwaysOnTop);
            Assert.AreEqual(PetSettings.DefaultSkinId, settings.pet.currentSkinId);
            Assert.AreEqual(1, logger.WarningCount);
            StringAssert.Contains("alwaysOnTop", File.ReadAllText(paths.SettingsFilePath));
        }

        [Test]
        public void Save_PublishesSettingsChangedEvent()
        {
            var service = CreateService(out _, out _, out var eventBus);
            var receivedEventCount = 0;
            AppSettings receivedSettings = null;
            eventBus.Subscribe<SettingsChanged>(eventData =>
            {
                receivedEventCount++;
                receivedSettings = eventData.Settings;
            });

            service.Load();
            service.Update(settings => settings.system.keepAwakeEnabled = true);

            Assert.AreEqual(1, receivedEventCount);
            Assert.IsNotNull(receivedSettings);
            Assert.IsTrue(receivedSettings.system.keepAwakeEnabled);
        }

        private SettingsService CreateService(out AppPaths paths, out TestLogger logger)
        {
            return CreateService(out paths, out logger, out _);
        }

        private SettingsService CreateService(out AppPaths paths, out TestLogger logger, out IEventBus eventBus)
        {
            paths = new AppPaths(_testRoot);
            eventBus = new EventBus();
            logger = new TestLogger();
            return new SettingsService(paths, new LocalStorageService(), eventBus, logger);
        }

        private sealed class TestLogger : ILogger
        {
            public int WarningCount { get; private set; }

            public void Debug(string message)
            {
            }

            public void Info(string message)
            {
            }

            public void Warning(string message)
            {
                WarningCount++;
            }

            public void Error(string message)
            {
            }

            public void Error(Exception exception, string message = null)
            {
            }
        }
    }
}
