using System.Collections;
using System.Collections.Generic;
using PathFinder.Grid2D;
using UnityEngine;

namespace PathFinder
{
    public class AStar : IPathFinder
    {
        public PriorityQueue<INode> frontier = new PriorityQueue<INode>(1000);
        public Dictionary<INode, INode> mappedNodes = new Dictionary<INode, INode>();
        Dictionary<INode, float> mappedNodeCosts = new Dictionary<INode, float>();

        public HeuristicFunc Heuristic { get; set; }

        public int VisitedNodes
        {
            get { return mappedNodes.Count; }
        }

        public float PathCost { get; private set; }

        public AStar(HeuristicFunc heuristic)
        {
            Heuristic = heuristic;
        }

        public IEnumerable<INode> FindPath(INode startNode, INode endNode)
        {
            frontier.Clear();
            mappedNodeCosts.Clear();
            mappedNodes.Clear();

            var path = new LinkedList<INode>();
            var currentNode = endNode;

            AdvanceFrontier(startNode, endNode);

            PathCost = mappedNodeCosts[endNode];

            var pathNode = endNode;
            while (pathNode != null)
            {
                path.AddFirst(pathNode);
                pathNode = mappedNodes[pathNode];
            }

            return path;
        }

        public void AdvanceFrontier(INode startNode, INode endNode)
        {
            frontier.Enqueue(startNode, 0);
            mappedNodes.Add(startNode, null);
            mappedNodeCosts.Add(startNode, 0);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                var @break = false;
                foreach (var nc in current.Value.Connections)
                {
                    var connection = nc.To;

                    var newCost = mappedNodeCosts[current.Value] + (nc.Cost * connection.Weight);
                    if (!mappedNodes.ContainsKey(connection) || newCost < mappedNodeCosts[connection])
                    {
                        // connection came from current
                        if (mappedNodes.ContainsKey(connection))
                        {
                            mappedNodes[connection] = current.Value;
                        }
                        else
                        {
                            mappedNodes.Add(connection, current.Value);
                        }

                        if (mappedNodeCosts.ContainsKey(connection))
                        {
                            mappedNodeCosts[connection] = newCost;
                        }
                        else
                        {
                            mappedNodeCosts.Add(connection, newCost);
                        }

                        var priority = newCost + Heuristic(endNode, connection);
                        frontier.Enqueue(new PQNode<INode>(connection), priority);

                        if (connection == endNode)
                        {
                            @break = true;
                            break;
                        }

                    }
                }
                
                if (@break) break;
            }
        }

    }
}