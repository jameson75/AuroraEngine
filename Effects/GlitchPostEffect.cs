using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;

namespace CipherPark.AngelJacket.Core.Effects
{
    public class GlitchPostEffect : PostEffect
    {
        public float RedChannelOffset { get; set; }
        public float BlueChannelOffset { get; set; }
        public float GreenChannelOffset { get; set; }
        public float RedChannelAlpha { get; set; }
        public float BlueChannelAlpha { get; set; }
        public float GreenChannelAlpha { get; set; }        
    }
}
