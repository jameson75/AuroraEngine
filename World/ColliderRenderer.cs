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

namespace CipherPark.AngelJacket.Core.World
{
    public class ColliderRenderer : IRenderer
    {
        public Collider Collider { get; set; }

        public SurfaceEffect Effect { get; set; }           

        public void Draw(GameTime gameTime)
        {
            if (Collider is QuadCollider)
                RenderQuadCollider((QuadCollider)Collider);
            
            else if(Collider is SphereCollider)
                RenderSphereCollider((SphereCollider)Collider);
            
            else if(Collider is BoundingBoxCollider)
                RenderBoundingBoxCollider((BoundingBoxCollider)Collider);
        }
        
        public void Update(GameTime gameTime)
        {
            
        }

        private void RenderBoundingBoxCollider(BoundingBoxCollider boundingBoxCollider)
        {
            throw new NotImplementedException();
        }

        private void RenderSphereCollider(SphereCollider sphereCollider)
        {
            throw new NotImplementedException();
        }

        private void RenderQuadCollider(QuadCollider quadCollider)
        {
            throw new NotImplementedException();
        }                
    }
}
