using System;
using UnityEngine;

namespace NewDiNoLock.Infrastructure
{
    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Error(Exception exception, string message = null);
    }

    public sealed class Logger : ILogger
    {
        private const string Prefix = "[NewDiNoLock]";

        public void Debug(string message)
        {
            UnityEngine.Debug.Log(Format(message));
        }

        public void Info(string message)
        {
            UnityEngine.Debug.Log(Format(message));
        }

        public void Warning(string message)
        {
            UnityEngine.Debug.LogWarning(Format(message));
        }

        public void Error(string message)
        {
            UnityEngine.Debug.LogError(Format(message));
        }

        public void Error(Exception exception, string message = null)
        {
            if (exception == null)
            {
                Error(message);
                return;
            }

            var formattedMessage = string.IsNullOrEmpty(message)
                ? Format(exception.Message)
                : Format(message);

            UnityEngine.Debug.LogError($"{formattedMessage}\n{exception}");
        }

        private static string Format(string message)
        {
            return string.IsNullOrEmpty(message) ? Prefix : $"{Prefix} {message}";
        }
    }
}
