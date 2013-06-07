using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;

namespace CipherPark.AngelJacket.Core.Kinetics
{
    public interface IRigidBody : ITransformable
    {
        Vector3 CenterOfMass { get; set; }
    }
}
