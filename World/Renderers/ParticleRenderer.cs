using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Kinetics;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Content;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Renderers
{
    public class ParticleRenderer
    {
        private IGameApp _game = null;
        private Mesh _particleMesh = null;
        private Effect _particleEffect = null;

       // private Mesh _linkMesh = null;
        //private BillboardEffect particleEffect = null;
        //private BasicEffectEx linkEffect = null;    

        public ParticleRenderer(IGameApp game, Mesh particleMesh, Effect particleEffect)
        {
            _game = game;
            _particleMesh = particleMesh;
            _particleEffect = particleEffect;
            //_game = game;
            //particleEffect = new BillboardEffect(_game.GraphicsDevice);
            //particleEffect.UseInstancing = enableInstancing;
            //3));

            //linkEffect = new BasicEffectEx(_game.GraphicsDevice);
            //linkEffect.EnableVertexColor = true;
            ////BasicVertexPositionColor[] emptyVerts = new BasicVertexPositionColor[4];
            ////emptyVerts.Initialize();
            //_linkMesh = ContentBuilder.BuildDynamicMesh<BasicVertexPositionColor>(game, linkEffect.SelectShaderByteCode(), null, 8, null, 0, BasicVertexPositionColor.InputElements, BasicVertexPositionColor.ElementSize, BoundingBoxExtension.Empty, true);
        }

        public Mesh ParticleMesh { get { return _particleMesh; } }

        public Effect ParticleEffect { get { return _particleEffect; } }

        public void DrawInstanced(long gameTime, IEnumerable<Matrix> matrices)
        {
            if (_particleMesh.IsInstanced)
            {
                //Flawed design: The data type of the instanced data is being
                //assumed here... for now it's just a world transformation matrix.
                //TODO: Figure out a way to infer the data type of the instance data.   
                if (matrices.Count() > 0)
                {
                    _particleMesh.Update<Matrix>(matrices.ToArray());
                    _particleEffect.Apply();
                    _particleMesh.Draw(gameTime);
                }
            }
            else
                throw new InvalidOperationException("Particle mesh is not instanced. Use Draw()");
        }

        public void Draw(long gameTime, IEnumerable<Particle> particles)
        {         
            if (!_particleMesh.IsInstanced)
            {
                foreach (Particle p in particles)
                {
                    _particleEffect.World = p.WorldTransform().ToMatrix();
                    _particleEffect.Apply();
                    _particleMesh.Draw(gameTime);
                }
            }
            else
                throw new InvalidOperationException("Particle mesh is instanced. Use DrawInstanced()");

            //List<BasicVertexPositionColor> linkVertices = new List<BasicVertexPositionColor>();
            //foreach (ParticleLink link in pLinks)
            //{
            //    BasicVertexPositionColor v1 = new BasicVertexPositionColor();
            //    v1.Color = Color.White.ToVector4();
            //    v1.Position = new Vector4((emitterTransform * link.P1.Transform.ToMatrix()).TranslationVector, 1.0f);
            //    BasicVertexPositionColor v2 = new BasicVertexPositionColor();
            //    v2.Color = Color.White.ToVector4();
            //    v2.Position = new Vector4((emitterTransform * link.P2.Transform.ToMatrix()).TranslationVector, 1.0f);
            //    linkVertices.Add(v1);
            //    linkVertices.Add(v2);
            //}

            //if (linkVertices.Count > 0)
            //{
            //    linkEffect.World = Matrix.Identity;
            //    linkEffect.View = view;
            //    linkEffect.Projection = projection;
            //    linkEffect.Apply();
            //    _linkMesh.Update<BasicVertexPositionColor>(linkVertices.ToArray());
            //    _linkMesh.Draw(gameTime);
            //}
        }
    }
}
