using SharpDX;

namespace CipherPark.Aurora.Core.Extensions
{
    public static class PlaneExtension
    {
        public static Vector3? ProjectPoint(this Plane p, Vector3 point, Vector3? alongVector = null)
        {
            if (alongVector == null)
                //{Projection of a Vector to the nearest point on a plane : B = A - Dot(A - P, N) * N.                
                //(See tmpearce's explanation at http://stackoverflow.com/questions/9605556/how-to-project-a-3d-point-to-a-3d-plane#comment12185786_9605695)            
                return point - Vector3.Dot(point - p.GetOrigin(), p.Normal) * p.Normal;
            else
            {
                Ray ray = new Ray(point, Vector3.Normalize(alongVector.Value));
                Vector3 intersection;
                if (p.Intersects(ref ray, out intersection))
                    return intersection;
                else
                    return null;
            }                
        }

        public static Vector3 GetOrigin(this Plane p)
        {
            // TODO: Re-examine the correctness. I may have to use -p.D insted of p.D;
            return p.Normal * p.D;
        }

        public static Ray GetRay(this Plane p)
        {
            return new Ray(p.Normal * -p.D, p.Normal);
        }
    }


}
