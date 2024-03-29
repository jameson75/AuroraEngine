﻿namespace CipherPark.Aurora.Core.World.Scene
{
    public abstract class SceneNodeBehaviour
    {
        private SceneNode _containerNode = null;

        public SceneNode ContainerNode
        {
            get { return _containerNode; }
            set
            {
                if (value == null)
                {
                    var c = _containerNode;
                    _containerNode = null;
                    if (c.Behaviour == this)
                        c.Behaviour = null;
                }
                else
                {
                    _containerNode = value;
                    if (_containerNode.Behaviour != this)
                        _containerNode.Behaviour = this;
                }
            }
        }

        public virtual void Update(SceneNode node) { }
    }
}
