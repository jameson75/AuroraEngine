using CipherPark.Aurora.Core.World.Scene;

namespace CipherPark.Aurora.Core.Services
{
    public class EditorObjectContext
    { 
        public GameObjectSceneNode ModifierTargetNode { get; set; }       
        public bool IsModifierRoot { get; set; }
        public bool IsTraversingPlane { get; set; }        
        public bool IsReferenceGrid { get; set; }
        public bool IsSelectionModifier { get; set; }
        public bool IsShadowModifier { get; set; }
        public bool IsReferenceObjectRoot { get; set; }
    }
}
