using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.ParticleSystem
{
    public class Emitter
    {
        private List<Particle> _particles = new List<Particle>();

        public Transform Transform { get; set; } 
        
        public Vector3 EmissionDirection { get; set; }
        
        public float EmissionRangePitch { get; set; }
        
        public float EmissionRangeYaw { get; set; }
        
        public ParticleDescription ParticleDescription { get; set; }        
       
        public void Emit()
        {
            Particle p = Spawn(ParticleDescription);
            _particles.Add(p);
        }

        public void Update(long gameTime)
        {
            if (ParticleDescription.BirthRate != 0)
            {

            }
        }

        private static Particle Spawn(ParticleDescription description)
        {
            Particle p = new Particle();
            return p;
        }
    }

    public class ParticleDescription
    {
        public int BirthRate { get; set; }
        public int BirthRateRandomness { get; set; }
        public int Life { get; set; }
        public int LifeRandomness { get; set; }
        public float Speed { get; set; }
        public int SpeedRandomness { get; set; }
        public Color? Color { get; set; }
        public Dictionary<float, Color> ColorOverTime { get; set; }
        public Dictionary<float, float> OpacityOverLife { get; set; }
        public float Scale { get; set; }
        public float ScaleRandomness { get; set; }
        public float PointSize { get; set; }
        public int RandomSeed { get; set; }
        public Texture2D Texture { get; set; }
    }

    public class Particle
    {

    }
}
    
