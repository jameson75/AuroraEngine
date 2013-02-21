using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public class SkySphere
    {
        public Mesh InnerSphere { get; set; }
        public Mesh OuterSphere { get; set; }
        public Mesh Plane { get; set; }
    }
}
