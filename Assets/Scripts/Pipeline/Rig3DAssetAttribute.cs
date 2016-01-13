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

    public bool IgnorePosition { get; set; }
    public bool IgnoreRotation { get; set; }
    public bool IgnoreScale { get; set; }
}
