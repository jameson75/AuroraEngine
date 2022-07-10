using System;
using System.Collections.Generic;
using System.Linq;
using CipherPark.Aurora.Core.World.Scene;

namespace CipherPark.Aurora.Core.Utils
{
    public static class SceneExtensions
    {
        public static IEnumerable<SceneNode> Select(this SceneGraph graph, Func<SceneNode, bool> filter)
        {
            return graph.Nodes.SelectMany(n => n.Select(filter));
        }

        public static IEnumerable<SceneNode> Select(this SceneNode node, Func<SceneNode, bool> filter)
        {
            List<SceneNode> results = new List<SceneNode>();
            
            if(filter(node))            
            {
                results.Add(node);
            }
            
            foreach(var childNode in node.Children)
            {
                results.AddRange(childNode.Select(filter));
            }

            return results;
        }
    }
}
