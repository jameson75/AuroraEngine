using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.World.Scene;

namespace Aurora.Core.Editor.Util
{
    public static class SurfaceEffectExtension
    {
        public static T As<T>(this SurfaceEffect effect) where T : SurfaceEffect
            => (T)effect;

    }

    public static class GameObjectExtension
    {
        public static Model GetGameModel(this GameObject gameObject)
            => gameObject.Renderer.As<ModelRenderer>()?.Model;
    }

    public static class SceneNodeExtension
    {
        public static bool IsDesignerNode(this SceneNode sceneNode)
            => sceneNode.As<GameObjectSceneNode>()?.GameObject.GetContext<EditorObjectContext>() != null;
    }
}
