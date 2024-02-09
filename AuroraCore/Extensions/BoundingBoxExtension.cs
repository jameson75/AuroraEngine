using SharpDX;

namespace CipherPark.Aurora.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class BoundingBoxExtension
    {
        private static BoundingBox _empty = new BoundingBox(Vector3.Zero, Vector3.Zero);

        public static BoundingBox Transform(this BoundingBox boundingBox, Matrix matrix)
        {           
            Vector4 vMin = Vector3.Transform(boundingBox.Minimum, matrix);
            Vector4 vMax = Vector3.Transform(boundingBox.Maximum, matrix);         
            return new BoundingBox(new Vector3(vMin.X, vMin.Y, vMin.Z), new Vector3(vMax.X, vMax.Y, vMax.Z));
        }

        public static float GetLengthX(this BoundingBox boundingBox)
        {
            return boundingBox.Maximum.X - boundingBox.Minimum.X;
        }

        public static float GetLengthY(this BoundingBox boundingBox)
        {
            return boundingBox.Maximum.Y - boundingBox.Minimum.Y;
        }

        public static float GetLengthZ(this BoundingBox boundingBox)
        {
            return boundingBox.Maximum.Z - boundingBox.Minimum.Z;
        }

        public static BoundingBox Empty { get { return _empty; } }    
    }


}
