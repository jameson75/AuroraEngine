using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Aurora.Core.Editor.Dom
{
    public class SceneNode
    {
        public string Name { get; set; }
        public float[] Matrix { get; set; }
        public ulong Flags { get; set; }
        public bool Visible { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public NodeType NodeType { get; set; }
        public GameObject GameObject { get; set; }
        public List<SceneNode> Children { get; } = new List<SceneNode>();
    }
}
