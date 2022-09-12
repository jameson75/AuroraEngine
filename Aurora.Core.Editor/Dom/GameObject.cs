using Aurora.Core.Editor.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Aurora.Core.Editor.Dom
{
    public class GameObject
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public GameObjectType GameObjectType { get; set; }
        public string ResourceFilename { get; set; }       
        public ModelEffect ModelEffect { get; set; }        
        public Light Light { get; set; }
    }
}
