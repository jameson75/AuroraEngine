using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.World;

namespace CipherPark.Aurora.Core.Services
{
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
}
