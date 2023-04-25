using System;

namespace CipherPark.Aurora.Core.World.Scene
{
    public class SceneRenderContext
    {
        public int CorrelationId { get; set; }
        public Func<SceneNode, bool> NodeFilter { get; set; }
        public bool ShowAll { get; set; }
    }
}
