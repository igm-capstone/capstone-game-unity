using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathFinder.Grid2D
{
    public class Grid2D : ISearchSpace
    {
        readonly Node2D[,] nodes;

        public Node2D this[int x, int y]
        {
            get
            {
                return nodes[x, y];
            }
        }

        public IEnumerable<INode> Nodes
        {
            get
            {
                // cast will convert the 2D array into IEnumerable ;)
                return nodes.Cast<INode>();
            }
        }

        public Grid2D(int width, int height)
        {
            nodes = new Node2D[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    nodes[i, j] = new Node2D(this, i, j);
                }
            }
        }

        public IEnumerable<NodeConnection> GetNodeConnections(int x, int y)
        {
            var notLeftEdge = x > 0;
            var notRightEdge = x < nodes.GetLength(0) - 1;
            var notBottomEdge = y > 0;
            var notTopEdge = y < nodes.GetLength(1) - 1;

            var connections = new List<NodeConnection>();

            //if (notLeftEdge) AddNodeIfValid(connections, nodes[x - 1, y]);
            //if (notRightEdge) AddNodeIfValid(connections, nodes[x + 1, y]);
            //if (notBottomEdge) AddNodeIfValid(connections, nodes[x, y - 1]);
            //if (notTopEdge) AddNodeIfValid(connections, nodes[x, y + 1]);

            //if (notLeftEdge && notBottomEdge) AddNodeIfValid(connections, nodes[x - 1, y - 1]);
            //if (notLeftEdge && notTopEdge) AddNodeIfValid(connections, nodes[x - 1, y + 1]);
            //if (notRightEdge && notBottomEdge) AddNodeIfValid(connections, nodes[x + 1, y - 1]);
            //if (notRightEdge && notTopEdge) AddNodeIfValid(connections, nodes[x + 1, y + 1]);

            if (notTopEdge) CreateConnectionIfValid(connections, nodes[x, y], nodes[x, y + 1]);
            if (notRightEdge && notTopEdge) CreateConnectionIfValid(connections, nodes[x, y], nodes[x + 1, y + 1]);
            if (notRightEdge) CreateConnectionIfValid(connections, nodes[x, y], nodes[x + 1, y]);
            if (notRightEdge && notBottomEdge) CreateConnectionIfValid(connections, nodes[x, y], nodes[x + 1, y - 1]);
            if (notBottomEdge) CreateConnectionIfValid(connections, nodes[x, y], nodes[x, y - 1]);
            if (notLeftEdge && notBottomEdge) CreateConnectionIfValid(connections, nodes[x, y], nodes[x - 1, y - 1]);
            if (notLeftEdge) CreateConnectionIfValid(connections, nodes[x, y], nodes[x - 1, y]);
            if (notLeftEdge && notTopEdge) CreateConnectionIfValid(connections, nodes[x, y], nodes[x - 1, y + 1]);

            return connections;
        }

        void CreateConnectionIfValid(List<NodeConnection> list, Node2D nodeFrom, Node2D nodeTo)
        {
            if (nodeTo.Weight < Single.MaxValue)
            {
                var conn = new NodeConnection {
                    // 1.4 for diagonals and 1 for horizontal or vertical connections
                    Cost = nodeFrom.X == nodeTo.X || nodeFrom.Y == nodeTo.Y ? 1 : 1.4f,
                    From = nodeFrom,
                    To = nodeTo,
                };

                list.Add(conn);
            }
        }

        public static float HeuristicManhattan(INode from, INode to)
        {
            var nFrom = @from as Node2D;
            var nTo = to as Node2D;

            // Manhattan distance on a square grid
            return Mathf.Abs(nFrom.X - nTo.X) + Mathf.Abs(nFrom.Y - nTo.Y);
        }

        //                                  dy  
        //                               ___|___
        //                     dx       /        \
        //    _________________|_______|__________|
        //   /                         |          \
        //  x--------------------------------------.  -.
        //                1x            '.         |    \ 
        //                                 '.      |     } dy 
        //                               1.4x '.   |    /
        //                                       '.|  -'
        //                                         x
        //
        //
        public static float HeuristicManhattan2(INode from, INode to)
        {
            var nFrom = @from as Node2D;
            var nTo = to as Node2D;

            var dx = Mathf.Abs(nFrom.X - nTo.X);
            var dy = Mathf.Abs(nFrom.Y - nTo.Y);

            // diagonal
            var d = Mathf.Min(dx, dy);

            // Manhattan distance on a square grid
            return Mathf.Abs(dx - dy) + (d * 1.4f);
        }
    }
}