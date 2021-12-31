using System;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.Utils.Toolkit;

namespace CipherPark.Aurora.Core.World
{
    public interface ISprite
    {
        RectangleF Bounds { get; }
        int Layer { get; }
        Vector2 Origin { get; }
        Vector2 Position { get; }
        float Rotation { get; }
        Vector2 Scale { get; }
        Size2 Size { get; }
        Rectangle? SourceRectangle { get; }
        SpriteEffects SpriteEffects { get; }
        ShaderResourceView Texture { get; }
        Color Tint { get; }
    }
}
