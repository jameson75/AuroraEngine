using CipherPark.Aurora.Core.World.Scene;

namespace CipherPark.Aurora.Core.Services
{
    public class EditorObjectContext
    { 
        public GameObjectSceneNode TargetNode { get; set; }       
        public bool IsAdornmentRoot { get; set; }
        public bool IsTraversingPlane { get; set; }        
        public bool IsReferenceGrid { get; set; }
        public bool IsSelectionAdornment { get; set; }
        public bool IsShadowAdronment { get; set; }
        public bool IsReferenceObjectRoot { get; set; }
        public bool IsInteractive { get; set; }
    }
}
