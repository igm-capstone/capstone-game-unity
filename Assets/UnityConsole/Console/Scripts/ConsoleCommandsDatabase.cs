using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wenzil.Console
{
    /// <summary>
    /// Use RegisterCommand() to register your own commands. Registered commands persist between scenes but don't persist between multiple application executions.
    /// </summary>
    public static class ConsoleCommandsDatabase 
    {
        private static Dictionary<string, ConsoleCommand> database = new Dictionary<string, ConsoleCommand>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Return all the commands in alphabetical order.
        /// </summary>
        public static IEnumerable<ConsoleCommand> commands { get { return database.OrderBy(kv => kv.Key).Select(kv => kv.Value); } }

        private static bool initialized;

        static void Initialize()
        {
            initialized = true;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        var attr = Attribute.GetCustomAttribute(method, typeof(ConsoleCommandAttribute)) as ConsoleCommandAttribute;
                        if (attr == null) continue;

                        var callback = Delegate.CreateDelegate(typeof(ConsoleCommandCallback), method) as ConsoleCommandCallback;


                        UnityEngine.Debug.Log("registering command " + attr.Command + " " + callback);
                        RegisterCommand(attr.Command, callback, attr.Description, attr.Usage);
                    }
                }
            }
        }

        public static void RegisterCommand(string command, ConsoleCommandCallback callback, string description = "", string usage = "") 
        {
            RegisterCommand(command, description, usage, callback);
        }

        public static void RegisterCommand(string command, string description, string usage, ConsoleCommandCallback callback)
        {
            database[command] = new ConsoleCommand(command, description, usage, callback);
        }

        public static void ExecuteCommand(string command, params string[] args)
        {
            if (!initialized) 
                Initialize();

            ConsoleCommand retrievedCommand = GetCommand(command);

            if (retrievedCommand == null)
            {
                UnityEngine.Debug.LogWarning("Command " + command.ToUpper() + " not found.");
                return;
            }

            retrievedCommand.callback(args);
        }

        public static bool TryGetCommand(string command, out ConsoleCommand result)
        {
            if (!initialized)
                Initialize();

            result = GetCommand(command);
            return result != null;
        }

        public static ConsoleCommand GetCommand(string command)
        {
            if (!initialized)
                Initialize();

            if (HasCommand(command))
            {
                return database[command];
            }
            else
            {
                return null;
            }
        }

        public static bool HasCommand(string command)
        {
            if (!initialized)
                Initialize();

            return database.ContainsKey(command);
        }
    }
}