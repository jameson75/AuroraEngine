using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Services;

namespace CipherPark.Aurora.Core.Utils
{
    public class ScenePicker
    {        
        public static IEnumerable<PickInfo> PickNodes(IGameApp gameApp, int x, int y, Func<GameObjectSceneNode, bool> customFilter = null)
        {
            List<PickInfo> results = new List<PickInfo>();

            var scene = gameApp.GetActiveScene();

            var camera = scene.CameraNode;

            Func<SceneNode, bool> filter = (node) => node is GameObjectSceneNode && (customFilter == null || customFilter(node.As<GameObjectSceneNode>()));

            var nodes = scene.Select(filter)
                                     .Distinct()
                                     .Cast<GameObjectSceneNode>()
                                     .ToArray();

            ViewportF vp = gameApp.GraphicsDeviceContext.Rasterizer.GetViewports<ViewportF>()[0];
            Vector3 near = Vector3.Unproject(new Vector3(x, y, vp.MinDepth), vp.X, vp.X, vp.Width, vp.Height, vp.MinDepth, vp.MaxDepth, camera.RiggedViewMatrix * camera.ProjectionMatrix);
            Vector3 far = Vector3.Unproject(new Vector3(x, y, vp.MaxDepth), vp.X, vp.X, vp.Width, vp.Height, vp.MinDepth, vp.MaxDepth, camera.RiggedViewMatrix * camera.ProjectionMatrix);
            Vector3 dir = Vector3.Normalize(far - near);
            Ray ray = new Ray(near, dir);

            foreach (var node in nodes)
            {
                var pickInfo = GetPickInfo(ray, node);
                if (pickInfo != null)
                    results.Add(pickInfo);
            }

            return results;
        }

        public static PickInfo GetPickInfo(Ray ray, GameObjectSceneNode node)
        {
            var gameObjectBounds = node.GameObject
                                       .GetBoundingBox()
                                       .GetValueOrDefault();
            Vector3 intersectionPoint;
            var localRay = node.WorldToLocalRay(ray);
            bool isHit = localRay.Intersects(ref gameObjectBounds, out intersectionPoint);
            if (isHit)
            {
                return new PickInfo()
                {
                    IntersectionPoint = node.LocalToWorldCoordinate(intersectionPoint),
                    Node = node,
                };
            }            
            return null;
        }      
    } 
}
