using System;

[Flags]
public enum Rig3DExports
{
    Position = (1 << 0),
    Rotation = (1 << 1),
    Scale    = (1 << 2),
    Layer    = (1 << 3),
    Tag      = (1 << 4),
    Mesh     = (1 << 5),
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class Rig3DAssetAttribute : Attribute
{

    // This is a positional argument
    public Rig3DAssetAttribute(string name, Rig3DExports exports = 0)
    {
        Name = name;
        Exports = exports;

        // TODO: Implement code here
        //throw new NotImplementedException();
    }

    public string Name { get; private set; }
    public Rig3DExports Exports { get; private set; }


    //public bool ExportPosition { get; set; }
    //public bool ExportRotation { get; set; }
    //public bool ExportScale { get; set; }
}
