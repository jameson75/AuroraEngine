using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Kinetics;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Effects;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.ParticleSystem
{
    public class ParticleRenderer
    {        
        private IGameApp _game = null;
        private Mesh _particleMesh = null;
        private Mesh _linkMesh = null;
        BillboardEffect particleEffect = null;
        BasicEffectEx linkEffect = null;

        public ParticleRenderer(IGameApp game)
        {
            _game = game;
            particleEffect = new BillboardEffect(_game.GraphicsDevice);
            _particleMesh = ContentBuilder.BuildBillboardQuad(game, particleEffect.SelectShaderByteCode(), new DrawingSizeF(3, 3));           
        
            linkEffect = new BasicEffectEx(_game.GraphicsDevice);
            linkEffect.EnableVertexColor = true;
            //BasicVertexPositionColor[] emptyVerts = new BasicVertexPositionColor[4];
            //emptyVerts.Initialize();
            _linkMesh = ContentBuilder.BuildDynamicMesh<BasicVertexPositionColor>(game, linkEffect.SelectShaderByteCode(), null, 8, null, 0, BasicVertexPositionColor.InputElements, BasicVertexPositionColor.ElementSize, BoundingBoxExtension.Empty, true);
        }

        public void Render(long gameTime, Matrix emitterTransform, Matrix view, Matrix projection, IEnumerable<Particle> pList, IEnumerable<ParticleLink> pLinks)
        {              
            //pne.BackgroundColor = Color.Red;
            //pne.ForegroundColor = Color.Blue;
            particleEffect.View = view;
            particleEffect.Projection = projection;         
            foreach (Particle p in pList)
            {
                particleEffect.World = emitterTransform * p.Transform.ToMatrix();
                particleEffect.Apply();
                _particleMesh.Draw(gameTime);
                //_linkMesh.Update<BasicVertexPositionColor>(linkVertices);
            }

            List<BasicVertexPositionColor> linkVertices = new List<BasicVertexPositionColor>();
            foreach (ParticleLink link in pLinks)
            {
                BasicVertexPositionColor v1 = new BasicVertexPositionColor();
                v1.Color = Color.White.ToVector4();
                v1.Position = new Vector4((emitterTransform * link.P1.Transform.ToMatrix()).TranslationVector, 1.0f);
                BasicVertexPositionColor v2 = new BasicVertexPositionColor();
                v2.Color = Color.White.ToVector4();
                v2.Position = new Vector4((emitterTransform * link.P2.Transform.ToMatrix()).TranslationVector, 1.0f);
                linkVertices.Add(v1);
                linkVertices.Add(v2);
            }

            if (linkVertices.Count > 0)
            {
                linkEffect.World = Matrix.Identity;
                linkEffect.View = view;
                linkEffect.Projection = projection;
                linkEffect.Apply();
                _linkMesh.Update<BasicVertexPositionColor>(linkVertices.ToArray());
                _linkMesh.Draw(gameTime);
            }
        }
    }
}
