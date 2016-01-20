using System;

[AttributeUsage(AttributeTargets.Class)]
public sealed class Rig3DAssetAttribute : Attribute
{
    // This is a positional argument
    public Rig3DAssetAttribute(string name)
    {
        Name = name;

        // TODO: Implement code here
        //throw new NotImplementedException();
    }

    public string Name { get; private set; }

    public bool ExportPosition { get; set; }
    public bool ExportRotation { get; set; }
    public bool ExportScale { get; set; }
}
