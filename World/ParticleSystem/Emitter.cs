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

namespace CipherPark.AngelJacket.Core.World.ParticleSystem
{
    public class Emitter
    {
        public Transform Transform { get; set; } 
        public float EmissionAngle { get; set; }
        public float EmissionRange { get; set; }
        public ParticleController ParticleController { get; set; }
    }

    public class ParticleController
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
    }
}
    
