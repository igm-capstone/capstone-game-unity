using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wenzil.Console
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    sealed class ConsoleCommandAttribute : Attribute
    {
        readonly string command;

        // This is a positional argument
        public ConsoleCommandAttribute(string command)
        {
            this.command = command;
        }

        public string Command
        {
            get { return command; }
        }

        public string Description { get; set; }

        public string Usage { get; set; }
    }

}
