using CipherPark.Aurora.Core.World.Scene;

namespace CipherPark.Aurora.Core.Services
{
    public class EditorObjectContext
    {        
        public bool IsModifier { get; set; }
        public bool IsTraversingPlane { get; set; }
        public GameObjectModifierMode ModifierMode { get; set; }
        public GameObjectSceneNode ModifierTargetNode { get; set; }      
    }
}
