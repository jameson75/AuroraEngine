using CipherPark.Aurora.Core.World.Scene;

namespace CipherPark.Aurora.Core.Services
{
    public class EditorObjectContext
    { 
        public GameObjectSceneNode TargetNode { get; set; }       
        public bool IsAdornmentRoot { get; set; }
        public bool SupportsCameraTraversing { get; set; }        
        public bool IsReferenceGrid { get; set; }
        public bool IsSelectionAdornment { get; set; }
        public bool IsShadowAdronment { get; set; }
        public bool IsReferenceObjectRoot { get; set; }
        public bool IsPickable { get; set; }
        public bool IsPathObject { get; set; }
        public bool IsActionObject { get; set; }
        public bool IsPathRootObject { get; set; }
        public bool IsSatelliteObject { get; set; }
    }
}
