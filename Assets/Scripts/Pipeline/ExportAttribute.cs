using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ExportAttribute : Attribute
{
    // This is a positional argument
    public ExportAttribute(string name = "")
    {
        Name = name;

        // TODO: Implement code here
        //throw new NotImplementedException();
    }

    public string Name { get; private set; }
}
