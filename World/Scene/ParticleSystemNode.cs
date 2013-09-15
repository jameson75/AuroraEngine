using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.World.Renderers;
using CipherPark.AngelJacket.Core.Kinetics;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Scene
{
    public class ParticleSystemNode : SceneNode
    {
        private Transform _transform = Transform.Identity;
        private Emitter _emitter = null;

        public ParticleSystemNode(IGameApp game)
            : base(game)
        {
            
        }

        public Emitter Emitter 
        {
            get
            {
                return _emitter;
            }

            set
            {
                if (_emitter != null)
                {
                    ((ITransformable)_emitter).TransformableParent = null;
                    if (value == null)
                        base.Transform = _emitter.Transform;
                }

                _emitter = value;

                if (_emitter != null)
                    ((ITransformable)_emitter).TransformableParent = this;
            }
        }

        public ParticleRenderer Renderer { get; set; } 


        public override Transform Transform
        {
            get
            {
                return base.Transform;
            }
            set
            {
                base.Transform = value;
            }
        }

        public override void Draw(long gameTime)
        {           
            base.Draw();
        }
    }
}
