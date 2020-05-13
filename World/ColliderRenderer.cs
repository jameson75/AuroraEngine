using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using SharpDX.XInput;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.Services;
using CipherPark.AngelJacket.Core.Content;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.World.Collision;
using CipherPark.AngelJacket.Core.Sequencer;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Kinetics;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World
{
    public class ColliderRenderer : IRenderer
    {
        private Collider _collider = null;
        private Mesh _mesh = null;
        private IGameApp _app = null;
        private SurfaceEffect _effect = null;

        public static ColliderRenderer Create(IGameApp game)
        {
            ColliderRenderer renderer = new ColliderRenderer();
            renderer._app = game;
            renderer._effect = new BasicEffectEx(game)
            {
                EnableVertexColor = true,
            };
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

        public SurfaceEffect Effect { get { return _effect; } }

        public Color Color { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            if (Effect != null && _mesh != null)
            {
                Effect.World = Collider.WorldTransform().ToMatrix();
                _mesh.Draw(gameTime);
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

            return ContentBuilder.BuildDynamicMesh<VertexPositionColor>(_app.GraphicsDevice,
                                                          _effect.SelectShaderByteCode(),
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
                                                          _effect.SelectShaderByteCode(),
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
                                                          _effect.SelectShaderByteCode(),
                                                          vertices,
                                                          vertices.Count(),
                                                          indices,
                                                          indices.Count(),
                                                          VertexPositionColor.InputElements,
                                                          VertexPositionColor.ElementSize,
                                                          meshBoundingBox,
                                                          SharpDX.Direct3D.PrimitiveTopology.LineList);
        }                
    }
}
