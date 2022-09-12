using SharpDX;
using System;

namespace Aurora.Sample.Editor.Scene
{
    public class ReferenceGrid
    {
        public ReferenceGrid(float width, int wSectors, int wSegments, float height, int hSectors, int hSegments, ReferenceGridOrientation orientation = ReferenceGridOrientation.Y)
        {
            Width = width;
            WidthSectors = wSectors;
            WidthSegments = wSegments;
            Height = height;
            HeightSectors = hSectors;
            HeightSegments = hSegments;
        }   

        public float Width { get; }

        public int WidthSectors { get; }

        public int WidthSegments { get; }

        public float Height { get; }

        public int HeightSegments { get; }

        public int HeightSectors { get; }             

        public Vector3 Normal
        {
            get
            {
                switch (Orientation)
                {
                    case ReferenceGridOrientation.Y:
                        return Vector3.Up;
                    case ReferenceGridOrientation.X:
                        return Vector3.Right;
                    case ReferenceGridOrientation.Z:
                        return Vector3.ForwardLH;
                    default:
                        throw new InvalidOperationException("Unsupported orientation");
                }           
            }
        }

        public ReferenceGridOrientation Orientation { get; } = ReferenceGridOrientation.Y;
    }
}
