using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World;

namespace CipherPark.Aurora.Core.Utils
{
    public static class GameObjectExtensions
    {
        public static bool SupportsCameraTraversing(this GameObject gameObject)
        {
            var context = gameObject.GetContext<EditorObjectContext>();
            return context != null && context.IsTraversingPlane;
        }

        public static bool IsEditorObject(this GameObject gameObject)
        {
            return gameObject.GetContext<EditorObjectContext>() != null;
        }
    }
}
