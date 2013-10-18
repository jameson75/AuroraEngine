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
using CipherPark.AngelJacket.Core.Content;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Effects
{
    public static class CommonBlendStates 
    {
        public static BlendState Create(Device graphicsDevice, BlendStatePresets preset)
        {
            BlendStateDescription desc = BlendStateDescription.Default();
            switch (preset)
            {
                case BlendStatePresets.Premultiplied:
                    desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].SourceBlend = BlendOption.SourceColor;
                    desc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
                    desc.RenderTarget[0].DestinationBlend = BlendOption.DestinationColor;
                    desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
                    break;
                case BlendStatePresets.NonPremultiplied:
                    desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].SourceBlend = BlendOption.SourceColor;
                    desc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
                    desc.RenderTarget[0].DestinationBlend = BlendOption.DestinationColor;
                    desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
                    break;
                case BlendStatePresets.Additive:
                    desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].SourceBlend = BlendOption.SourceColor;
                    desc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
                    desc.RenderTarget[0].DestinationBlend = BlendOption.DestinationColor;
                    desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;
                    break;
                case BlendStatePresets.Multiplicative:
                    desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
                    desc.RenderTarget[0].SourceBlend = BlendOption.SourceColor;
                    desc.RenderTarget[0].SourceAlphaBlend = BlendOption.Zero;
                    desc.RenderTarget[0].DestinationBlend = BlendOption.DestinationColor;
                    desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.SourceColor;
                    break;
            }
            return new BlendState(graphicsDevice, desc);            
        }
    }

    public enum BlendStatePresets
    {
        Premultiplied,
        NonPremultiplied,
        Additive,
        Multiplicative
    }
}
