using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;

namespace CipherPark.AngelJacket.Core.Kinetics
{
    public class Force
    {
        public Vector3 LinearVelocity { get; set; }
        public Quaternion AngularVelocity { get; set; }
        public bool IsImpulse { get; set; }
    }

    public class Motion
    {
        
    }
}
