using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace Wenzil.Console.Commands
{ 
    /// <summary>
    /// HELP command. Display the list of available commands or details about a specific command.
    /// </summary>
    public static class HelpCommand
    {
        public static readonly string name = "HELP";
        public static readonly string description = "Display the list of available commands or details about a specific command.";
        public static readonly string usage = "HELP [command]";

        private static StringBuilder commandList = new StringBuilder();

        public static void Execute(params string[] args)
        {
            if (args.Length == 0)
            {
                DisplayAvailableCommands();
            }
            else
            {
                DisplayCommandDetails(args[0]);
            }
        }

        private static void DisplayAvailableCommands()
        {
            commandList.Length = 0; // clear the command list before rebuilding it
            commandList.Append("<b>Available Commands</b>\n");

            foreach (ConsoleCommand command in ConsoleCommandsDatabase.commands)
            {
                commandList.Append(string.Format("    <b>{0}</b> - {1}\n", command.name, command.description));
            }

            commandList.Append("To display details about a specific command, type 'HELP' followed by the command name.");
            Debug.Log(commandList);
        }

        private static void DisplayCommandDetails(string commandName)
        {
            string formatting =
@"<b>{0} Command</b>
    <b>Description:</b> {1}
    <b>Usage:</b> {2}";

            ConsoleCommand command = ConsoleCommandsDatabase.GetCommand(commandName);

            if (command == null)
            {
                Debug.LogFormat("Cannot find help information about {0}. Are you sure it is a valid command?");
                return;
            }

            Debug.LogFormat(formatting, command.name, command.description, command.usage);
        }
    }
}
