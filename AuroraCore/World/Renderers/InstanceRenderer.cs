using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using CipherPark.Aurora.Core.Extensions;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.World.Geometry;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
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
        public List<ITransformable> Instances { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Func<List<ITransformable>> GetInstances { get; set; }

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
            IEnumerable<ITransformable> instances = Instances ?? GetInstances?.Invoke();
            if (instances != null)
            {
                DrawInstances(container, Mesh, Effect, instances);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="mesh"></param>
        /// <param name="effect"></param>
        /// <param name="instances"></param>
        private void DrawInstances(ITransformable container, Mesh mesh, SurfaceEffect effect, IEnumerable<ITransformable> instances)
        {
            if (mesh.IsInstanced == false)
                throw new InvalidOperationException("Mesh is not instanced.");

            if (instances.Count() > 0)
            {
                //*************************************************************************************
                //NOTE: It is expected that each particle being rendered is an immediate child of this 
                //particle system.                                                                                 
                //**************************************************************************************    

                //TODO: Ensure that the correct buffer size of instance data is being determined.
                var cameraNode = _game.GetRenderingCamera();                
                IEnumerable<InstanceVertexData> instanceData = instances.Where(p => !IsInstanceClipped(p, mesh, cameraNode))
                                                                                .Select(p => new InstanceVertexData()
                                                                                {
                                                                                    Matrix = Matrix.Transpose(p.Transform.ToMatrix())
                                                                                });
                mesh.UpdateInstanceStream(instanceData.ToArray());                
                effect.View = cameraNode.RiggedViewMatrix;
                effect.Projection = cameraNode.ProjectionMatrix;
                effect.World = container == null ? Matrix.Identity : container.WorldTransform().ToMatrix();
                effect.Apply();
                mesh.Draw();
                effect.Restore();
            }
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool IsInstanceClipped(ITransformable p, Mesh mesh, CameraSceneNode cameraNode)
        {
            return EnableFustrumClipping && IsInstanceInFustrum(p, mesh, cameraNode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        private bool IsInstanceInFustrum(ITransformable instance, Mesh mesh, CameraSceneNode cameraNode)
        {           
            Matrix viewMatrix = Camera.TransformToViewMatrix(cameraNode.WorldTransform());
            Matrix projMatrix = cameraNode.Camera.ProjectionMatrix;
            BoundingFrustum fustrum = new BoundingFrustum(viewMatrix * projMatrix);
            BoundingBox box = mesh.BoundingBox.Transform(instance.WorldTransform().ToMatrix());
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
