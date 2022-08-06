using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World;

namespace CipherPark.Aurora.Core.Utils
{
    public static class GameObjectExtensions
    {
        public static bool SupportsCameraTraversing(this GameObject gameObject)
        {
            return gameObject.GetContext<EditorObjectContext>()?.IsTraversingPlane ?? false;            
        }

        public static bool IsEditorObject(this GameObject gameObject)
        {
            return gameObject.GetContext<EditorObjectContext>() != null;
        }

        public static bool IsReferenceGridObject(this GameObject gameObject)
        {
            return gameObject.GetContext<EditorObjectContext>()?.IsReferenceGrid ?? false;          
        }
    }
}
