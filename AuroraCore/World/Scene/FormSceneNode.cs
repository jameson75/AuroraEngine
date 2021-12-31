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

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Scene
{
    public class FormSceneNode : SceneNode
    {
        Transform _transform = Transform.Identity;
        Form _form = null;

        public FormSceneNode(IGameApp game)
            : base(game)
        {
            
        }

        public FormSceneNode(Form form, string name = null)
            : base(form.Game, name)
        {
            Form = form;            
        }

        public Form Form
        {
            get
            {
                return _form;
            }
            set
            {
                if (_form != null)
                {
                    ((ITransformable)_form).TransformableParent = null;
                    if (value == null)
                        _transform = _form.Transform;
                }

                _form = value;

                if (_form != null)
                    ((ITransformable)_form).TransformableParent = this;
            }
        }       

        public override Transform Transform
        {
            get
            {             
                return _transform;
            }
            set
            {              
                _transform = value;
            }
        }        

        public override void Draw(GameTime gameTime)
        {
            if (Form != null)
            {                
                Form.ElementEffect.View = Camera.TransformToViewMatrix(Scene.CameraNode.ParentToWorld(Scene.CameraNode.Transform)); //ViewMatrix;
                Form.ElementEffect.Projection = Scene.CameraNode.Camera.ProjectionMatrix;
                Form.Draw(gameTime);
            }
            base.Draw(gameTime);
        }
    }
}
