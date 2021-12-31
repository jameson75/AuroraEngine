using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.World.Geometry;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World
{
    public class InstanceRenderer : IRenderer
    {
        private IGameApp _game = null;       

        public InstanceRenderer(IGameApp game)
        {
            _game = game;
        }
       
        /// <summary>
        /// 
        /// </summary>
        public IGameApp Game { get { return _game; } }      

        /// <summary>
        /// 
        /// </summary>
        public SurfaceEffect Effect { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Mesh Mesh { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ITransformable> Instances { get; set; } = new List<ITransformable>();

        /// <summary>
        /// 
        /// </summary>
        public bool EnableFustrumClipping { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public void Draw(ITransformable container)
        {
            DrawInstances(container, Mesh, Effect, Instances);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="instanceMesh"></param>
        /// <param name="instanceEffect"></param>
        /// <param name="instances"></param>
        private void DrawInstances(ITransformable container, Mesh instanceMesh, SurfaceEffect instanceEffect, IEnumerable<ITransformable> instances)
        {
            if (instanceMesh.IsInstanced == false || instanceMesh.IsDynamic == false)
                throw new InvalidOperationException("Mesh is not both dynamic and instanced.");

            if (instances.Count() > 0)
            {
                //*************************************************************************************
                //NOTE: It is expected that each particle being rendered is an immediate child of this 
                //particle system.                                                                                 
                //**************************************************************************************    

                //TODO: Ensure that the correct buffer size of instance data is being determined.                
                IEnumerable<InstanceVertexData> instanceData = instances.Where(p => !IsInstanceClipped(p, instanceMesh))
                                                                                .Select(p => new InstanceVertexData()
                                                                                {
                                                                                    Matrix = Matrix.Transpose(p.Transform.ToMatrix())
                                                                                });
                instanceMesh.UpdateVertexStream<InstanceVertexData>(instanceData.ToArray());
                var cameraNode = _game.GetActiveModuleContext()
                                  .Scene
                                  .CameraNode;
                instanceEffect.View = cameraNode.RiggedViewMatrix;
                instanceEffect.Projection = cameraNode.ProjectionMatrix;
                instanceEffect.World = container == null ? Matrix.Identity : container.WorldTransform().ToMatrix();
                instanceEffect.Apply();
                instanceMesh.Draw();
                instanceEffect.Restore();
            }
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool IsInstanceClipped(ITransformable p, Mesh instanceMesh)
        {
            return EnableFustrumClipping && IsInstanceInFustrum(p, instanceMesh);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        private bool IsInstanceInFustrum(ITransformable instance, Mesh instanceMesh)
        {
            SceneGraph scene = Game.GetActiveModuleContext().Scene;
            Matrix viewMatrix = Camera.TransformToViewMatrix(scene.CameraNode.WorldTransform());
            Matrix projMatrix = scene.CameraNode.Camera.ProjectionMatrix;
            BoundingFrustum fustrum = new BoundingFrustum(viewMatrix * projMatrix);
            BoundingBox box = instanceMesh.BoundingBox.Transform(instance.WorldTransform().ToMatrix());
            return fustrum.Intersects(ref box);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Mesh?.Dispose();
            Effect?.Dispose();
        }
    }
}
