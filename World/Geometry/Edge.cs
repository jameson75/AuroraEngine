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
    public class VectorPath
    {       
        private List<Vector3> _data = new List<Vector3>();        
        public List<Vector3> Data { get { return _data; } }        
    }   
}
