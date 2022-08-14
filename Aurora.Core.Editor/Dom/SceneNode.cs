using System.Collections.Generic;

namespace Aurora.Core.Editor.Dom
{
    public class SceneNode
    {
        public string Name { get; internal set; }
        public float[] Matrix { get; internal set; }
        public ulong Flags { get; internal set; }
        public bool Visible { get; internal set; }        
        public GameObjectType GameObjectType { get; set; }
        public GameObjectDescription GameObjectDescription { get; set; }
        public List<SceneNode> Children { get; } = new List<SceneNode>();
    }
}
