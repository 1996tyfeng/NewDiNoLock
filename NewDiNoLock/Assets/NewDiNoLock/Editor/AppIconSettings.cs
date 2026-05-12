using UnityEditor;

namespace NewDiNoLock.Editor
{
    [InitializeOnLoad]
    public static class AppIconSettings
    {
        static AppIconSettings()
        {
            EditorApplication.delayCall += BuildAutomation.ApplyAppIcon;
        }
    }
}
