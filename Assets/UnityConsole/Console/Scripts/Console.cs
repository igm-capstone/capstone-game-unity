using System;
using System.Collections;
using UnityEngine;

public delegate void OnConsoleLog(object line);

namespace Wenzil.Console
{
    /// <summary>
    /// Use Console.Log() anywhere in your code. The Console prefab will display the output.
    /// </summary>
    public static class Console
    {
        public static OnConsoleLog OnConsoleLog;
        private static class Tags
        {
            public static string InvalidCommand;
            public static string CommandNotFound;

            public static string Error;
            public static string Warning;
            public static string Log;
            public static string Exception;
            public static string Assert;
        }

        public static bool OutputStackTrace { get; set; }

        static Console()
        {
            Application.logMessageReceived += HandleUnityLog;

            Tags.InvalidCommand = ColorTag("Invalid Command!", "#FF0000");
            Tags.CommandNotFound = ColorTag("Unrecognized command: ", "#FF0000");
            
            Tags.Error = ColorTag("Error: ", "#EEAA00");
            Tags.Warning = ColorTag("Warning: ", "#CCAA00");
            Tags.Log = ColorTag("Log: ", "#AAAAAA");
            Tags.Exception = ColorTag("Exception: ", "#FF0000");
            Tags.Assert = ColorTag("Assert: ", "#0000FF");
        }

        public static string ColorTag(string text, string color) {
            return string.Format("<color={0}>{1}</color>", color, text);
        }

        private static void HandleUnityLog(string logString, string trace, LogType logType)
        {
            string output = String.Empty;

            switch (logType)
            {
                case LogType.Error:
                    output += Tags.Error;
                    break;
                case LogType.Assert:
                    output += Tags.Assert;
                    break;
                case LogType.Warning:
                    output += Tags.Warning;
                    break;
                case LogType.Log:
                    output += Tags.Log;
                    break;
                case LogType.Exception:
                    output += Tags.Exception;
                    break;
                default:
                    return;
            }
            output += logString + (OutputStackTrace ? "\n" + ColorTag(trace, "#AAAAAA") : String.Empty);
            if (OnConsoleLog != null)
                OnConsoleLog(output);
        }

        public static void ExecuteCommand(string command, params string[] args)
        {
            ConsoleCommandsDatabase.ExecuteCommand(command, args);
        }
    }

    public static class MBExtensions {
        #region Typesafe Invoke
        public static void Invoke(this MonoBehaviour mb, Action action, float delay) {
            if(delay == 0f)
                action();
            else
                mb.StartCoroutine(DelayedInvoke(action, delay));
        }

        public static void Invoke<T>(this MonoBehaviour mb, Action<T> action, T arg, float delay) {
            if(delay == 0f)
                action(arg);
            else
                mb.StartCoroutine(DelayedInvoke(action, arg, delay));
        }

        public static void Invoke<T1, T2>(this MonoBehaviour mb, Action<T1, T2> action, T1 arg1, T2 arg2, float delay) {
            if(delay == 0f)
                action(arg1, arg2);
            else
                mb.StartCoroutine(DelayedInvoke(action, arg1, arg2, delay));
        }

        public static void Invoke<T1, T2, T3>(this MonoBehaviour mb, Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, float delay) {
            if(delay == 0f)
                action(arg1, arg2, arg3);
            else
                mb.StartCoroutine(DelayedInvoke(action, arg1, arg2, arg3, delay));
        }

        public static void Invoke<T1, T2, T3, T4>(this MonoBehaviour mb, Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, float delay) {
            if(delay == 0f)
                action(arg1, arg2, arg3, arg4);
            else
                mb.StartCoroutine(DelayedInvoke(action, arg1, arg2, arg3, arg4, delay));
        }

        private static IEnumerator DelayedInvoke(Action action, float delay) {
            yield return new WaitForSeconds(delay);
            action();
        }

        private static IEnumerator DelayedInvoke<T>(Action<T> action, T arg, float delay) {
            yield return new WaitForSeconds(delay);
            action(arg);
        }

        private static IEnumerator DelayedInvoke<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, float delay) {
            yield return new WaitForSeconds(delay);
            action(arg1, arg2);
        }

        private static IEnumerator DelayedInvoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, float delay) {
            yield return new WaitForSeconds(delay);
            action(arg1, arg2, arg3);
        }

        private static IEnumerator DelayedInvoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, float delay) {
            yield return new WaitForSeconds(delay);
            action(arg1, arg2, arg3, arg4);
        }
        #endregion
    }
}