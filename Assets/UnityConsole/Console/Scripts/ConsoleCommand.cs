namespace Wenzil.Console 
{
    public delegate void ConsoleCommandCallback(params string[] args);

    public class ConsoleCommand 
    {
        public string name { get; private set; }
        public string description { get; private set; }
        public string usage { get; private set; }
        public ConsoleCommandCallback callback { get; private set; }

        public ConsoleCommand(string name, string description, string usage, ConsoleCommandCallback callback) 
        {
            this.name = name;
            this.description = (string.IsNullOrEmpty(description) ? "No description provided" : description);
            this.usage = (string.IsNullOrEmpty(usage) ? "No usage information provided" : usage);
            this.callback = callback;
        }
    }
}