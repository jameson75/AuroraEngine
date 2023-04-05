using Aurora.Core.Editor.Environment;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Utils;
using System;
using System.Linq;
using SharpDX;
using Aurora.Sample.Editor.Scene;
using System.Collections.Generic;

namespace Aurora.Core.Editor.Util
{
    public static class SurfaceEffectExtension
    {
        public static T As<T>(this SurfaceEffect effect) where T : SurfaceEffect
            => (T)effect;
    }

    public static class GameObjectExtension
    {
        public static bool IsGameModelObject(this GameObject gameObject)
            => gameObject.GetContext<Model>() != null;

        public static bool IsEditorObject(this GameObject gameObject)
            => gameObject.GetContext<EditorObjectContext>() != null;

        public static bool IsReferenceGridObject(this GameObject gameObject)
            => gameObject.GetContext<EditorObjectContext>()?.IsReferenceGrid ?? false;

        public static bool IsPickableObject(this GameObject gameObject)
            => gameObject.GetContext<EditorObjectContext>() == null ||
               gameObject.GetContext<EditorObjectContext>().IsPickable;

        public static bool IsPathObject(this GameObject gameObject)
            => gameObject.GetContext<EditorObjectContext>()?.IsPathObject ?? false;

        public static bool IsActionObject(this GameObject gameObject)
            => gameObject.GetContext<EditorObjectContext>()?.IsActionObject ?? false;

        public static bool IsPathRootObject(this GameObject gameObject)
            => gameObject.GetContext<EditorObjectContext>()?.IsPathRootObject ?? false;

        public static bool IsSatelliteObject(this GameObject gameObject)
            => gameObject.GetContext<EditorObjectContext>()?.IsSatelliteObject ?? false;

        public static bool SupportsCameraTraversing(this GameObject gameObject)
            => gameObject.GetContext<EditorObjectContext>()?.SupportsCameraTraversing ?? false;

        public static Model GetGameModel(this GameObject gameObject)
            => gameObject.GetContext<Model>();

        public static string GetResourceFilename(this GameObject gameObject)
            => gameObject.GetContext<GameObjectMeta>()?.ResourceFilename;

        public static Light GetLighting(this GameObject gameObject)
            => gameObject.GetContext<Light>();           

        public static ReferenceGrid GetReferenceGrid(this GameObject gameObject)
            => gameObject.GetContext<ReferenceGrid>();      

        public static NavigationPath GetNavigationPath(this GameObject gameObject)
            => gameObject.GetContext<NavigationPath>();       
    }

    public static class SceneNodeExtension
    {
        public static bool IsEditorNode(this SceneNode sceneNode)
            => sceneNode.GetGameObject()?.IsEditorObject() ?? false;

        public static bool IsPathNode(this SceneNode sceneNode)
            => sceneNode.GetGameObject()?.IsPathObject() ?? false;

        public static bool IsActionNode(this SceneNode sceneNode)
            => sceneNode.GetGameObject()?.IsActionObject() ?? false;

        public static bool IsPathRootNode(this SceneNode sceneNode)
            => sceneNode.GetGameObject()?.IsPathRootObject() ?? false;

        public static bool IsSatelliteNode(this SceneNode sceneNode)
            => sceneNode.GetGameObject()?.IsSatelliteObject() ?? false;

        public static GameObject GetGameObject(this SceneNode sceneNode)
            => sceneNode.As<GameObjectSceneNode>()?.GameObject;

        public static BoundingBoxOA GetWorldBoundingBox(this GameObjectSceneNode sceneNode)
          => sceneNode.ParentToWorldBoundingBox(sceneNode.GameObject.GetBoundingBox().GetValueOrDefault());

        public static void Orphan(this SceneNode sceneNode)
            => sceneNode.Parent?.Children.Remove(sceneNode);
    }

    public static class CastExtensions
    {
        public static T As<T>(this Light light) where T : Light
            => light as T;
    }

    public static class ModelExtensions
    {
        public static Mesh GetMesh(this Model model)
        {
            if (model is StaticMeshModel)
            {
                return ((StaticMeshModel)model).Mesh;
            }

            if (model is SkinnedMeshModel)
            {
                return ((SkinnedMeshModel)model).Mesh;
            }

            return null;
        }
    }

    public static class SceneExtensions
    {
        public static void Clear(this SceneNodes nodes, Func<SceneNode, bool> filter)
        {
            foreach (var node in nodes.ToList())
            {
                if (filter(node))
                {
                    nodes.Remove(node);
                }
            }
        }

        public static GameObjectSceneNode SelectReferenceObjectRoot(this SceneGraph scene)
        {
            return scene.SelectNodes(n => n.Visible &&
                                     n.GetGameObject()
                                      .GetContext<EditorObjectContext>()
                                      ?.IsReferenceObjectRoot == true)
                        .First()
                        .As<GameObjectSceneNode>();
        }

        public static IEnumerable<GameObjectSceneNode> GameObjectChildren(this SceneNode node)
            => node.Children.Where(n => n is GameObjectSceneNode)
                            .Select(n => n.As<GameObjectSceneNode>());
    }

    public static class Vector3Extensions
    {
        public static Vector3 AddX(this Vector3 v, float x)
            => new Vector3(v.X + x, v.Y, v.Z);

        public static Vector3 AddY(this Vector3 v, float y)
            => new Vector3(v.X, v.Y + y, v.Z);

        public static Vector3 AddZ(this Vector3 v, float z)
            => new Vector3(v.X, v.Y, v.Z + z);

        public static Vector3 Add(this Vector3 v, float x, float y, float z)
            => new Vector3(v.X + x, v.Y + y, v.Z + z);
    }
}
