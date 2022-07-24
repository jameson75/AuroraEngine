using SharpDX;
using CipherPark.Aurora.Core.World.Scene;

namespace CipherPark.Aurora.Core.Utils
{
    public class PickInfo
    {
        public Vector3 IntersectionPoint { get; set; }
        public GameObjectSceneNode Node { get; set; }
        public Ray Ray { get; set; }
    }
}
