using System;

namespace CipherPark.Aurora.Core.World.Scene
{
    public class SceneRenderContext
    {
        public string Id { get; set; }
        public bool ShowAll { get; set; }
        public Func<SceneNode, bool> NodeFilter { get; set; }        
    }
}
