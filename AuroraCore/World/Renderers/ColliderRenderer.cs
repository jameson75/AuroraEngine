using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Utils.Toolkit;
using CipherPark.Aurora.Core.UI.Controls;
using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.World.Collision;
using CipherPark.Aurora.Core.Sequencer;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Systems;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World
{
    public class ColliderRenderer : IRenderer
    {
        private Collider _collider = null;
        private Mesh _mesh = null;
        private IGameApp _app = null;
        private SurfaceEffect _effect = null;

        private SurfaceEffect Effect { get { return _effect; } }

        public static ColliderRenderer Create(IGameApp game)
        {
            ColliderRenderer renderer = new ColliderRenderer();
            renderer._app = game;
            renderer._effect = new FlatEffect(game, SurfaceVertexType.PositionColor);        
            return renderer;
        }

        public Collider Collider
        {
            get { return _collider; }
            set
            {
                _collider = value;
                OnColliderChanged();
            }
        }        

        public Color Color { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(ITransformable container)
        {
            if (Effect != null && _mesh != null)
            {
                var cameraNode = _app.GetActiveModuleContext()
                                 .Scene
                                 .CameraNode;            
                //NOTE: When calculating the world transform of the collider, we use the container as the collider's parent
                //space if one is specified, otherwise, we use the explicit parent space of the collider.
                Effect.World = container != null ? container.WorldTransform().ToMatrix() * Collider.Transform.ToMatrix() :
                                                   Collider.WorldTransform().ToMatrix();
                Effect.View = cameraNode.RiggedViewMatrix;
                Effect.Projection = cameraNode.ProjectionMatrix;
                Effect.Apply();
                _mesh.Draw();
                Effect.Restore();
            }
        }
        
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
        protected void OnColliderChanged()
        {
            if (_mesh != null)
            {
                _mesh.Dispose();
                _mesh = null;
            }

            if (Collider is QuadCollider)
                _mesh = ConstructMeshFromQuadCollider();

            else if (Collider is SphereCollider)
                _mesh = ConstructMeshFromSphereCollider();

            else if (Collider is BoxCollider)
                _mesh = ConstructMeshFromBoxCollider();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Mesh ConstructMeshFromBoxCollider()
        {
            const float EdgeLength = 5.0f;
            BoxCollider boxCollider = (BoxCollider)Collider;
            BoundingBoxOA renderBox = boxCollider.Box;
            const float Padding = 0.5f;
            renderBox.Inflate(Padding, Padding, Padding);

            Vector3[] corners = renderBox.GetCorners();
            Vector3[] points = new Vector3[32];
            short[] indices = new short[48];            

            for (int i = 0; i < corners.Length; i++)
            {
                int a = i;
                int b = (i <= 3) ? ((i == 0) ? 3 : i - 1) : ((i == 4) ? 7 : i - 1);
                int c = (i <= 3) ? ((i == 3) ? 0 : i + 1) : ((i == 7) ? 4 : i + 1);
                int d = (i <= 3) ? i + 4 : i - 4;
                int v = i * 4;
                points[v] = corners[a];
                points[v + 1] = corners[a] + Vector3.Normalize(corners[b] - corners[a]) * EdgeLength;
                points[v + 2] = corners[a] + Vector3.Normalize(corners[c] - corners[a]) * EdgeLength;
                points[v + 3] = corners[a] + Vector3.Normalize(corners[d] - corners[a]) * EdgeLength;
                
                int n = i * 6;                
                indices[n] = (short)v;
                indices[n + 1] = (short)( v + 1 );
                indices[n + 2] = (short)v;
                indices[n + 3] = (short)(v + 2);
                indices[n + 4] = (short)v;
                indices[n + 5] = (short)(v + 3);
            }

            VertexPositionColor[] vertices = points.Select( p => new VertexPositionColor()
                                                            {
                                                                Position = new Vector4(p, 1.0f),
                                                                Color = this.Color.ToVector4(),
                                                            }).ToArray();

            BoundingBox meshBoundingBox = BoundingBox.FromPoints(points);

            return ContentBuilder.BuildDynamicMesh(_app.GraphicsDevice,
                                                   _effect.GetVertexShaderByteCode(),
                                                   vertices,
                                                   vertices.Count(),
                                                   indices,
                                                   indices.Count(),
                                                   VertexPositionColor.InputElements,
                                                   VertexPositionColor.ElementSize,
                                                   meshBoundingBox, 
                                                   SharpDX.Direct3D.PrimitiveTopology.LineList);                                                                      
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Mesh ConstructMeshFromSphereCollider()
        {  
            const float EdgeLength = 7.0f;       
            SphereCollider sphereCollider = (SphereCollider)Collider;
            BoundingBox renderBox = BoundingBox.FromSphere(sphereCollider.Sphere);          
            Vector3[] corners = renderBox.GetCorners();
            Vector3[] points = new Vector3[32];
            short[] indices = new short[48];            

            for (int i = 0; i < corners.Length; i++)
            {
                int a = i;
                int b = (i <= 3) ? ((i == 0) ? 3 : i - 1) : ((i == 4) ? 7 : i - 1);
                int c = (i <= 3) ? ((i == 3) ? 0 : i + 1) : ((i == 7) ? 4 : i + 1);
                int d = (i <= 3) ? i + 4 : i - 4;
                int v = i * 4;
                points[v] = corners[a];
                points[v + 1] = corners[a] + Vector3.Normalize(corners[b] - corners[a]) * EdgeLength;
                points[v + 2] = corners[a] + Vector3.Normalize(corners[c] - corners[a]) * EdgeLength;
                points[v + 3] = corners[a] + Vector3.Normalize(corners[d] - corners[a]) * EdgeLength;

                int n = i * 6;
                indices[n] = (short)v;
                indices[n + 1] = (short)(v + 1);
                indices[n + 2] = (short)v;
                indices[n + 3] = (short)(v + 2);
                indices[n + 4] = (short)v;
                indices[n + 5] = (short)(v + 3);
            }

            VertexPositionColor[] vertices = points.Select(p => new VertexPositionColor()
            {
                Position = new Vector4(p, 1.0f),
                Color = this.Color.ToVector4(),
            }).ToArray();

            BoundingBox meshBoundingBox = BoundingBox.FromPoints(points);

            return ContentBuilder.BuildDynamicMesh<VertexPositionColor>(_app.GraphicsDevice,
                                                          _effect.GetVertexShaderByteCode(),
                                                          vertices,
                                                          vertices.Count(),
                                                          indices,
                                                          indices.Count(),
                                                          VertexPositionColor.InputElements,
                                                          VertexPositionColor.ElementSize,
                                                          meshBoundingBox,
                                                          SharpDX.Direct3D.PrimitiveTopology.LineList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Mesh ConstructMeshFromQuadCollider()
        {
            const float EdgeLength = 5.0f;
            QuadCollider quadCollider = (QuadCollider)Collider;
            Vector3[] corners = quadCollider.Quad.GetCorners();
            Vector3[] allCorners = corners.Concat(corners).ToArray();           
            Vector3[] points = new Vector3[32];
            short[] indices = new short[48];

            for (int i = 0; i < allCorners.Length; i++)
            {
                int a = i;
                int b = (i <= 3) ? ((i == 0) ? 3 : i - 1) : ((i == 4) ? 7 : i - 1);
                int c = (i <= 3) ? ((i == 3) ? 0 : i + 1) : ((i == 7) ? 4 : i + 1);                
                int v = i * 4;
                points[v] = allCorners[a];
                points[v + 1] = allCorners[a] + Vector3.Normalize(allCorners[b] - allCorners[a]) * EdgeLength;
                points[v + 2] = allCorners[a] + Vector3.Normalize(allCorners[c] - allCorners[a]) * EdgeLength;               

                int n = i * 6;
                indices[n] = (short)v;
                indices[n + 1] = (short)(v + 1);
                indices[n + 2] = (short)v;
                indices[n + 3] = (short)(v + 2);

                //TODO:Since I've decided to change the shape of the rendered brackets, get rid of these two last indices...
                indices[n + 4] = 0; //(short)(v + 1);
                indices[n + 5] = 0; // (short)(v + 2);
            }

            VertexPositionColor[] vertices = points.Select(p => new VertexPositionColor()
            {
                Position = new Vector4(p, 1.0f),
                Color = this.Color.ToVector4(),
            }).ToArray();

            BoundingBox meshBoundingBox = BoundingBox.FromPoints(points);

            return ContentBuilder.BuildDynamicMesh<VertexPositionColor>(_app.GraphicsDevice,
                                                          _effect.GetVertexShaderByteCode(),
                                                          vertices,
                                                          vertices.Count(),
                                                          indices,
                                                          indices.Count(),
                                                          VertexPositionColor.InputElements,
                                                          VertexPositionColor.ElementSize,
                                                          meshBoundingBox,
                                                          SharpDX.Direct3D.PrimitiveTopology.LineList);
        }

        public void Dispose()
        {
            if (_mesh != null && !_mesh.IsDisposed)
                _mesh.Dispose();

            if (_effect != null && !_effect.IsDisposed)
                _effect.Dispose();
        }
    }
}
