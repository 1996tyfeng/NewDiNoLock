using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace UnityMCP.Editor
{
    /// <summary>
    /// Reports Unity MCP connection status to a file and the console for analysis.
    /// Menu: UnityMCP → Check Connection Status (Write Report)
    /// </summary>
    public static class UnityMCPStatusReporter
    {
        private const string ReportFileName = "mcp_connection_report.txt";
        private const string ReportFolder = "Assets/Plugins/UnityMCPPlugin";

        [MenuItem("UnityMCP/Check Connection Status (Write Report)", false, 10)]
        public static void WriteConnectionReport()
        {
            var report = GetConnectionReport();
            string path = Path.Combine(Application.dataPath, "..", ReportFolder, ReportFileName).Replace("\\", "/");
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(path, report);
            Debug.Log($"[UnityMCP] Connection report written to: {path}\n{report}");
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Returns a formatted report of the current MCP connection status.
        /// </summary>
        public static string GetConnectionReport()
        {
            bool connected = UnityMCPConnection.IsConnected;
            var uri = UnityMCPConnection.ServerUri;
            var lastError = UnityMCPConnection.LastErrorMessage ?? "(none)";
            var logs = UnityMCPConnection.GetRecentLogs(null, 20);

            var lines = new System.Collections.Generic.List<string>
            {
                "========== Unity MCP Connection Report ==========",
                $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                "",
                "--- Status ---",
                $"Connected: {connected}",
                $"Server URI: {uri}",
                $"Last Error: {lastError}",
                "",
                "--- Analysis ---"
            };

            if (connected)
            {
                lines.Add("OK: MCP client is connected. Unity can communicate with the MCP server.");
            }
            else
            {
                lines.Add("NOT CONNECTED: No active connection to the MCP server.");
                if (lastError.IndexOf("refused", StringComparison.OrdinalIgnoreCase) >= 0)
                    lines.Add("  → Cause: Connection refused. Ensure an MCP server is running at the Server URI (e.g. in Cursor or as a separate process).");
                else if (lastError.IndexOf("timed out", StringComparison.OrdinalIgnoreCase) >= 0)
                    lines.Add("  → Cause: Connection timed out. Check firewall or that the server is listening.");
                else if (!string.IsNullOrEmpty(lastError) && lastError != "(none)")
                    lines.Add($"  → Last error: {lastError}");
            }

            lines.Add("");
            lines.Add("--- Recent UnityMCP Logs (last 20) ---");
            if (logs != null && logs.Length > 0)
            {
                foreach (var log in logs)
                    lines.Add(log);
            }
            else
            {
                lines.Add("(no recent logs)");
            }

            lines.Add("");
            lines.Add("==============================================");
            return string.Join(Environment.NewLine, lines);
        }
    }
}
