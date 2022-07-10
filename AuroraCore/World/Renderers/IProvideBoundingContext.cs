using SharpDX;

namespace CipherPark.Aurora.Core.World
{
    public interface IProvideBoundingContext
    {
        BoundingBox? GetBoundingBox();
    }
}