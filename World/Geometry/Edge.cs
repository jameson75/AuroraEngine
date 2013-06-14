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
    }

    public class Edges
    {       
        private List<Vector3> _data = new List<Vector3>();
        private EdgeDataLayout _layout;
        public Edges(EdgeDataLayout layout)
        {
            _layout = layout;
        }
        public List<Vector3> Data { get { return _data; } }        
    }

    public enum EdgeDataLayout
    {
        List,
        Strip
    }
}
