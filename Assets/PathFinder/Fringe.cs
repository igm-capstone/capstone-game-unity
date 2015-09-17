using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;

namespace PathFinder
{
    public class Fringe : IPathFinder
    {
        public struct FringeNode {
            public INode parent;
            public float cost;

            public override string ToString()
            {
                return String.Format("{0}->{1}", cost, (object)parent ?? "Null");
            }
        }

        public LinkedList<INode> fringe = new LinkedList<INode>();
        public LinkedList<INode> pathInteractive = new LinkedList<INode>();
        public Dictionary<INode, FringeNode> cache = new Dictionary<INode, FringeNode>();

        public HeuristicFunc Heuristic { get; set; }

        public int VisitedNodes
        {
            get { return cache.Count; }
        }

        public float PathCost { get; private set; }

        public Fringe(HeuristicFunc heuristic)
        {
            Heuristic = heuristic;
        }

        public IEnumerable<INode> FindPath(INode startNode, INode endNode)
        {
            var path = new LinkedList<INode>();

            fringe.Clear();
            cache.Clear();

            fringe.AddFirst(startNode);
            cache.Add(startNode, new FringeNode());

            var fLimit = Heuristic(startNode, endNode);
            var found = false;

            while (!found && fringe.Count > 0)
            {
                var fMin = float.MaxValue;

                for (var linkedNode = fringe.First; linkedNode != null;)
                {
                    var node = linkedNode.Value;
                    var nodeInfo = cache[node];
                    var fNode = nodeInfo.cost + Heuristic(node, endNode);

                    if (fNode > fLimit)
                    {
                        fMin = Mathf.Min(fNode, fMin);
                        linkedNode = linkedNode.Next;
                        continue;
                    }

                    if (node == endNode)
                    {
                        found = true;
                        break;
                    }

                    foreach (var connection in node.Connections.Reverse())
                    {
                        var costConn = nodeInfo.cost + (connection.Cost * connection.To.Weight);
                        var connNode = connection.To;

                        FringeNode connInfo;
                        if (cache.TryGetValue(connNode, out connInfo))
                        {
                            if (costConn >= connInfo.cost)
                            {
                                continue;
                            }
                        }

                        var linkedConn = fringe.Find(connNode);
                        if (linkedConn != null)
                        {
                            fringe.Remove(linkedConn);
                            fringe.AddAfter(linkedNode, linkedConn);
                        }
                        else
                        {
                            fringe.AddAfter(linkedNode, connNode);
                        }

                        connInfo.parent = node;
                        connInfo.cost = costConn;

                        cache[connNode] = connInfo;
                    }

                    var lastNode = linkedNode;

                    linkedNode = lastNode.Next;

                    fringe.Remove(lastNode);
                }

                fLimit = fMin;
            }


            if (!found)
            {
                return null;
            }

            PathCost = cache[endNode].cost;

            var pathNode = endNode;
            while (pathNode != null)
            {
                path.AddFirst(pathNode);
                pathNode = cache[pathNode].parent;
            }

            return path;
        }


        public IEnumerator FindInteractive(INode startNode, INode endNode, System.Func<Coroutine> coroutine = null)
        {
            fringe.AddFirst(startNode);
            cache.Add(startNode, new FringeNode());

            var fLimit = Heuristic(startNode, endNode);
            var found = false;

            while (!found && fringe.Count > 0)
            {
                var fMin = Single.MaxValue;

                for (var linkedNode = fringe.First; linkedNode != null;)
                {
                    var node = linkedNode.Value;

                    var nodeInfo = cache[node];
                    var fNode = nodeInfo.cost + Heuristic(node, endNode);

                    if (fNode > fLimit)
                    {
                        fMin = Mathf.Min(fNode, fMin);
                        linkedNode = linkedNode.Next;
                        continue;
                    }

                    if (node == endNode)
                    {
                        found = true;
                        break;
                    }

                    foreach (var connection in node.Connections.Reverse())
                    {
                        var costConn = nodeInfo.cost + (connection.Cost * connection.To.Weight);
                        var connNode = connection.To;

                        if (coroutine != null)
                        {
                            yield return coroutine();
                        }

                        FringeNode connInfo;
                        if (cache.TryGetValue(connNode, out connInfo))
                        {
                            if (costConn >= connInfo.cost)
                            {
                                continue;
                            }
                        }

                        var linkedConn = fringe.Find(connNode);
                        if (linkedConn != null)
                        {
                            fringe.Remove(linkedConn);
                            fringe.AddAfter(linkedNode, linkedConn);
                        }
                        else
                        {
                            fringe.AddAfter(linkedNode, connNode);
                        }

                        connInfo.parent = node;
                        connInfo.cost = costConn;

                        cache[connNode] = connInfo;
                    }

                    var lastNode = linkedNode;

                    linkedNode = lastNode.Next;

                    fringe.Remove(lastNode);
                }

                fLimit = fMin;
            }

            PathCost = cache[endNode].cost;

            if (coroutine != null)
            {
                if (cache.ContainsKey(endNode))
                {
                    var node = endNode;
                    pathInteractive.Clear();
                    while (node != null)
                    {
                        pathInteractive.AddFirst(node);
                        node = cache[node].parent;
                    }
                }

                yield return coroutine();
            }

        }
    }
}