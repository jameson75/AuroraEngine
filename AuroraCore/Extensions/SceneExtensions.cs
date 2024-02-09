using System;
using System.Collections.Generic;
using System.Linq;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World.Scene;

namespace CipherPark.Aurora.Core.Extensions
{
    public static class SceneExtensions
    {
        public static IEnumerable<SceneNode> SelectNodes(this SceneGraph graph, Func<SceneNode, bool> filter)
        {
            return graph.Nodes.SelectNodes(filter);
        }

        public static IEnumerable<SceneNode> SelectNodes(this IEnumerable<SceneNode> nodes, Func<SceneNode, bool> filter)
        {
            return nodes.SelectMany(n => n.SelectNodes(filter));
        }

        public static IEnumerable<SceneNode> SelectNodes(this SceneNode node, Func<SceneNode, bool> filter)
        {
            List<SceneNode> results = new List<SceneNode>();
            
            if(filter(node))            
            {
                results.Add(node);
            }
            
            foreach(var childNode in node.Children)
            {
                results.AddRange(childNode.SelectNodes(filter));
            }

            return results;
        }

        public static IEnumerable<Light> SelectLights(this SceneGraph graph)
        {
            return graph.SelectNodes(
                x => x.As<GameObjectSceneNode>()?
                      .GameObject
                      .GetContext<Light>() != null)
                      .Select(x => x.As<GameObjectSceneNode>().GameObject.GetContext<Light>())
                      .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static ICollection<SceneNode> SearchNodes(this SceneNode node, params string[] names)
        {
            List<SceneNode> results = new List<SceneNode>();
            _Search(node, names, results);
            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static ICollection<SceneNode> SearchNodes(this SceneGraph graph, params string[] names)
        {
            List<SceneNode> results = new List<SceneNode>();
            foreach (var node in graph.Nodes)
                _Search(node, names, results);
            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        internal static bool MatchesFilter(this SceneNode node, Func<SceneNode, bool> filter)
        {
            return filter == null || filter.Invoke(node);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="names"></param>
        /// <param name="results"></param>
        private static void _Search(SceneNode root, string[] names, List<SceneNode> results)
        {
            if (names.Contains(root.Name))
                results.Add(root);

            foreach (var node in root.Children)
                _Search(node, names, results);
        }
    }
}
