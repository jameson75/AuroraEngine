using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.World;

namespace CipherPark.Aurora.Core.Services
{
    public class MouseTrackingService
    {
        private Point? mouseLocation;

        public MouseTrackingService(IGameApp gameApp)
        {
            GameApp = gameApp;
        }

        private IGameApp GameApp { get; }

        public void UpdateMouse(Point location)
        {
            mouseLocation = location;
        }

        public Point? GetMouseLocation()
        {
            return mouseLocation;
        }
    }

    public class ScenePicker
    {        
        public static IEnumerable<PickInfo> PickNodes(IGameApp gameApp, int x, int y, Func<SceneNode, bool> filter)
        {
            List<PickInfo> results = new List<PickInfo>();

            var scene = gameApp.GetActiveScene();

            var camera = scene.CameraNode;

            var platformNodes = scene.Select(filter)
                                  .Distinct()
                                  .Cast<GameObjectSceneNode>()
                                  .ToArray();

            ViewportF vp = gameApp.GraphicsDeviceContext.Rasterizer.GetViewports<ViewportF>()[0];
            Vector3 near = Vector3.Unproject(new Vector3(x, y, vp.MinDepth), vp.X, vp.X, vp.Width, vp.Height, vp.MinDepth, vp.MaxDepth, camera.RiggedViewMatrix * camera.ProjectionMatrix);
            Vector3 far = Vector3.Unproject(new Vector3(x, y, vp.MaxDepth), vp.X, vp.X, vp.Width, vp.Height, vp.MinDepth, vp.MaxDepth, camera.RiggedViewMatrix * camera.ProjectionMatrix);
            Vector3 dir = Vector3.Normalize(far - near);
            Ray ray = new Ray(near, dir);
            foreach (var platformNode in platformNodes)
            {
                var platformPlane = platformNode.GameObject
                                        .GetContext<PlatformPlane>()
                                        .Plane;
                Vector3 intersectionPoint;
                bool isHit = ray.Intersects(ref platformPlane, out intersectionPoint);
                if (isHit)
                {
                    results.Add(new PickInfo()
                    {
                        IntersectionPoint = intersectionPoint,
                        Node = platformNode,
                    });
                }
            }

            return results;
        }       
    } 

    public class PickInfo
    {
        public Vector3 IntersectionPoint { get; set; }
        public GameObjectSceneNode Node { get; set; } 
    }

    public class PlatformPlane
    {
        public Plane Plane { get; set; }
    }

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

    public static class SharpDXExtensions
    {
        static public BoundingBox ExpandedAlongY(this BoundingBox box)
        {
            return new BoundingBox(new Vector3(box.Minimum.X, float.MinValue, box.Minimum.Z),
                                   new Vector3(box.Maximum.X, float.MaxValue, box.Maximum.Z));
        }

        static public Vector2 XY(this Vector3 v) { return new Vector2(v.X, v.Y); }

        static public Vector2 XZ(this Vector3 v) { return new Vector2(v.X, v.Z); }

        static public Vector2 YZ(this Vector3 v) { return new Vector2(v.Y, v.Z); }

        static public Vector3 MapToXZ(this Vector2 v, float y) { return new Vector3(v.X, y, v.Y); }
    }

    public static class CastExtensions
    {
        public static T As<T>(this SceneNodeBehaviour behaviour) where T : SceneNodeBehaviour
        {
            return behaviour as T;
        }

        public static T As<T>(this IRenderer renderer) where T : class, IRenderer
        {
            return renderer as T;
        }

        public static T As<T>(this SceneNode sceneNode) where T : SceneNode
        {
            return sceneNode as T;
        }
    }

    public static class PickExtensions
    {
        public static PickInfo GetClosest(this IEnumerable<PickInfo> pickList, Vector3 from)
        {
            PickInfo closestPick = null;
            float closestPickDistanceSq = 0;

            foreach (var pickInfo in pickList)
            {
                var distanceSq = Vector3.DistanceSquared(closestPick.IntersectionPoint, pickInfo.IntersectionPoint);
                if (closestPick == null || distanceSq < closestPickDistanceSq)
                {
                    closestPick = pickInfo;
                    closestPickDistanceSq = distanceSq;
                }
            }
                
            return closestPick;
        }
    }
}
