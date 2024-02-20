using System;
using System.ComponentModel;

namespace CipherPark.Aurora.Core.World.Scene
{
    public class SceneRenderContext
    {
        public SceneRenderContext()
        { }

        public SceneRenderContext(Func<SceneNode, bool> nodeFilter, bool showAll = false, string contextId = null)
        {
            Id = contextId;
            ShowAll = showAll;
            NodeFilter = nodeFilter;
        }

        public string Id { get; set; }
        public bool ShowAll { get; set; }
        public Func<SceneNode, bool> NodeFilter { get; set; }
        public static SceneRenderContext Default { get; } = new SceneRenderContext();
    }
}
