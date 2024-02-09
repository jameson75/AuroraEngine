using SharpDX.Direct3D11;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Extensions
{
    public static class Texture2DExtension
    {
        public static ShaderResourceView ToShaderResourceView(this Texture2D texture, SharpDX.Direct3D11.Device device)
        {
            return new ShaderResourceView(device, texture);
        }
    }
}
