using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Aurora.Core.Editor.Dom
{
    public class GameObject
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public GameObjectType GameObjectType { get; set; }
        public string ModelFileName { get; set; }
        public string EffectName { get; set; }
    }
}
