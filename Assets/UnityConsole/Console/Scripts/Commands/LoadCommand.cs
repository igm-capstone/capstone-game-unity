using UnityEngine;
using System.Collections;

/// <summary>
/// LOAD command. Load the specified scene by name.
/// </summary>

namespace Wenzil.Console.Commands
{
    public static class LoadCommand
    {
        public static readonly string name = "LOAD";
        public static readonly string description = "Load the specified scene by name. Before you can load a scene you have to add it to the list of levels used in the game. Use File->Build Settings... in Unity and add the levels you need to the level list there.";
        public static readonly string usage = "LOAD scene";

        public static void Execute(params string[] args)
        {
            if (args.Length == 0)
            {
                HelpCommand.Execute(LoadCommand.name);
            }
            else
            {
                LoadLevel(args[0]);
            }
        }

        private static void LoadLevel(string scene)
        {
            try
            {
                Application.LoadLevel(scene);
            }
            catch
            {
                Debug.LogFormat("Failed to load {0}.", scene);
            }

            if (Application.loadedLevelName == scene) // Assume success if we had to load the scene we were already in
                Debug.LogFormat("Success loading {0}.", scene);
            else
                Debug.LogFormat("Failed to load {0}. Are you sure it's in the list of levels in Build Settings?", scene);
        }
    }
}