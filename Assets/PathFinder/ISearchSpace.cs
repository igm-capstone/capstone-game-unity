using System.Collections.Generic;

namespace PathFinder
{
    public interface ISearchSpace
    {
        IEnumerable<INode> Nodes { get; }
    }
}