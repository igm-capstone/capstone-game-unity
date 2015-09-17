using System;
using System.Collections.Generic;

namespace PathFinder.Grid2D
{
    public class Node2D : INode
    {
        public readonly Grid2D Grid;

        public readonly int X;
        public readonly int Y;

        public float Weight { get; set; }

        public IEnumerable<NodeConnection> Connections
        {
            get
            {
                return Grid.GetNodeConnections(X, Y);
            }
        }

        public Node2D(Grid2D grid, int x, int y, float weight = 1)
        {
            Grid = grid;
            X = x;
            Y = y;
            Weight = weight;
        }

        public override string ToString()
        {
            return String.Format("[({0}, {1}); {2}]", X, Y, Weight);
        }
    }
}