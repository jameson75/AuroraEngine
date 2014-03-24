using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
    public class ModelSceneNode : SceneNode
    {
        Transform _transform = Transform.Identity;       
        Model _model = null;
        
        public ModelSceneNode(IGameApp game)
            : base(game)
        {            
            
        }

        public ModelSceneNode(Model model, string name = null)
            : base(model.Game, name)
        {
            Model = model;            
        }

        public Model Model 
        { 
            get 
            { 
                return _model; 
            } 
            set 
            {
                if (_model != null)
                {
                    ((ITransformable)_model).TransformableParent = null;
                    if (value == null)
                        _transform = _model.Transform;
                }

                _model = value;
                
                if (_model != null)               
                    ((ITransformable)_model).TransformableParent = this;                                  
            }
        }     

        public override Transform Transform 
        { 
            get 
            {
                //if (Model != null)
                //    return Model.Transform;
                //else
                    return _transform;
            }
            set 
            {
                //if (Model != null)
                //    Model.Transform = value;
                //else
                    _transform = value;
            }
        }

        //public override BoundingBox Bounds
        //{
        //    get
        //    {
        //        return Model.Mesh.BoundingBox;
        //    }
        //}

        public override void Draw(long gameTime)
        {
            if (Model != null)
            {
                //TODO: Change design so that Model.Draw() implementations aquire the Camera from a IViewportService
                //and set the view and projecton matrices themselves.
                
                Model.Effect.View = Camera.TransformToViewMatrix(Scene.CameraNode.ParentToWorld(Scene.CameraNode.Transform)); //ViewMatrix;
                Model.Effect.Projection = Scene.CameraNode.Camera.ProjectionMatrix;                
                Model.Draw(gameTime);               
            }
            base.Draw(gameTime);
        }
    }
}
