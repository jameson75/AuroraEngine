using SharpDX;
using SharpDX.Direct3D11;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Extensions
{
    public static class ResourceViewExtension
    {
        public static Texture2DDescription GetTextureDescription(this ResourceView view)
        {
            using (var resource = view.ResourceAs<Texture2D>())
                return resource.Description;
        }

        public static Size2 GetTexture2DSize(this ResourceView view)
        {
            return new Size2(view.GetTextureDescription().Width, view.GetTextureDescription().Height);
        }
    }
}
