using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;


namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public struct Edge
    {
        public Vector3 P1;
        public Vector3 P2;

        public Edge(Vector3 p1, Vector3 p2)
        {
            P1 = p1;
            P2 = p2;
        }
    }

    public class Edges : List<Edge> { }
}
