using System.Collections.Generic;

namespace PathFinder
{
    public interface INode
    {
        float Weight { get; }

        IEnumerable<NodeConnection> Connections { get; }
    }
}