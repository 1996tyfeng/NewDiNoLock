using System.IO;
using UnityEngine;

namespace NewDiNoLock.System
{
    public sealed class AppPaths
    {
        public const string SettingsFileName = "settings.json";

        public AppPaths()
            : this(Application.persistentDataPath)
        {
        }

        public AppPaths(string dataRoot)
        {
            DataRoot = dataRoot;
            SettingsFilePath = Path.Combine(DataRoot, SettingsFileName);
        }

        public string DataRoot { get; }
        public string SettingsFilePath { get; }
    }
}
