using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.XAudio2;

namespace CipherPark.AngelJacket.Core.Utils
{
    public class DXToolKit
    {
        public DeviceContext DeviceContext { get; set; }

        public Device Device { get; set; }

        public XAudio2 Audio2Device { get; set; }        
    }
}
