using System.Linq;
using SharpDX;

namespace CipherPark.Aurora.Core.Utils
{
    /// <summary>
    /// Represents and object aligned bounding box.
    /// </summary>
    public struct BoundingBoxOA
    {
        /// <summary>
        /// 
        /// </summary>
        public Vector3 BackTopLeft { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 BackTopRight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 BackBottomRight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 BackBottomLeft { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 FrontTopLeft { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 FrontTopRight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 FrontBottomRight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 FrontBottomLeft { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corners"></param>
        /// <returns></returns>
        public static BoundingBoxOA FromCorners(Vector3[] corners)
        {
            //******************************************************
            //NOTE:  
            //ASSUMING Left-Handed system, clockwise winding order.
            //If viewer is facing +z direction then...
            //back 0, 1, 2, 3 - starting from top left.
            //front 4, 5, 6, 7 - starting from top left.
            //******************************************************
            BoundingBoxOA newBox = new BoundingBoxOA();
            newBox.BackTopLeft = corners[0];
            newBox.BackTopRight = corners[1];
            newBox.BackBottomRight = corners[2];
            newBox.BackBottomLeft = corners[3];
            newBox.FrontTopLeft = corners[4];
            newBox.FrontTopRight = corners[5];
            newBox.FrontBottomRight = corners[6];
            newBox.FrontBottomLeft = corners[7];
            return newBox;
        }

        public static BoundingBoxOA FromAABoundingBox(BoundingBox box)
        {
            Vector3[] corners = box.GetCorners();
            return FromCorners(corners);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public ContainmentType Contains(BoundingBox box)
        {
            int nCornersContained = 0;

            Vector3[] _corners = GetCorners();
            for (int i = 0; i < _corners.Length; i++)
            {
                Vector3 c = _corners[i];
                ContainmentType t = box.Contains(ref c);
                if (t == ContainmentType.Intersects)
                    return ContainmentType.Intersects;
                else if (t == ContainmentType.Contains)
                    nCornersContained++;
            }

            if (nCornersContained == 0)
                return ContainmentType.Disjoint;
            else if (nCornersContained == _corners.Length)
                return ContainmentType.Contains;
            else
                return ContainmentType.Intersects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public ContainmentType Contains(Vector3 point)
        {
            return Contains(point, GetPlanes());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sphere"></param>
        /// <returns></returns>
        public ContainmentType Contains(BoundingSphere sphere)
        {                       
            var centerContainment = Contains(sphere.Center);
            if (centerContainment != ContainmentType.Disjoint)
            {
                return (GetCorners().Any(c => Vector3.Distance(c, sphere.Center) < sphere.Radius)) ?
                    ContainmentType.Contains :
                    ContainmentType.Intersects;
            }

            BoundingQuadOA[] quads = GetQuads();
            for (int i = 0; i < quads.Length; i++)
            {
                if (quads[i].Intersects(ref sphere))
                {
                    return ContainmentType.Intersects;
                }
            }

            return ContainmentType.Disjoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="planes"></param>
        /// <returns></returns>
        private ContainmentType Contains(Vector3 point, Plane[] planes)
        {
            bool intersectingPlaneFound = false;

            for (int i = 0; i < planes.Length; i++)
            {
                PlaneIntersectionType t = planes[i].Intersects(ref point);

                //If the point is in front of any plane of the bounding box
                //then it is on the outside of the box. Otherwise, it's either
                //inside the box or on the surface of the box (intersects the box)>
                if (t == PlaneIntersectionType.Front)
                    return ContainmentType.Disjoint;

                else if (t == PlaneIntersectionType.Intersecting)
                    intersectingPlaneFound = true;
            }

            //At this point we know that the point is inside or
            //the surface of the of the bounding box.
            return intersectingPlaneFound ? 
                ContainmentType.Intersects :
                ContainmentType.Contains;
        }       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sphere"></param>
        /// <returns></returns>
        public bool Intersects(BoundingSphere sphere)
        {
            return this.Contains(sphere) != ContainmentType.Disjoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool Intersects(BoundingBoxOA box)
        {            
            foreach (var v in GetCorners())
            {
                if (box.Contains(v) != ContainmentType.Disjoint)
                    return true;
            }

            foreach (var v in box.GetCorners())
            {
                if (Contains(v) != ContainmentType.Disjoint)
                    return true;
            }

            var thisPlanes = GetPlanes();
            for(int i = 0; i < thisPlanes.Length; i+=2)
            {
                Ray ray1 = thisPlanes[i].GetRay().Reverse();
                Ray ray2 = thisPlanes[i + 1].GetRay().Reverse();
                if( box.Intersects(ref ray1, out _) && box.Intersects(ref ray2, out _))
                {
                    return true;
                }
            }

            var boxPlanes = box.GetPlanes();
            for (int i = 0; i < boxPlanes.Length; i+=2)
            {
                Ray ray1 = boxPlanes[i].GetRay().Reverse();
                Ray ray2 = boxPlanes[i + 1].GetRay().Reverse();
                if (Intersects(ref ray1, out _) && Intersects(ref ray2, out _))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public PlaneIntersectionType Intersects(ref Plane plane)
        {
            bool hasPointInFront = false;
            bool hasPointInBack = false;

            Vector3[] _corners = GetCorners();
            for (int i = 0; i < _corners.Length; i++)
            {
                PlaneIntersectionType t = plane.Intersects(ref _corners[i]);
                if (t == PlaneIntersectionType.Front)
                    hasPointInFront = true;
                else if (t == PlaneIntersectionType.Back)
                    hasPointInBack = true;

                //Return 'Intersecting' if any point of this bounding box is coplanar to the specified
                //plane or we've determined that this bounding box has points on both sides of that plane.
                if (t == PlaneIntersectionType.Intersecting || (hasPointInFront && hasPointInBack))
                    return PlaneIntersectionType.Intersecting;
            }

            //At this point, it's been determined that all corners of this bounding box exist on the
            //same side of the specified plane.

            return hasPointInFront ? PlaneIntersectionType.Front : PlaneIntersectionType.Back;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public bool Intersects(ref Ray ray, out Vector3 point)
        {
            BoundingQuadOA[] quads = GetQuads();
            point = Vector3.Zero;
            for (int i = 0; i < quads.Length; i++)
            {
                Plane plane = quads[i].GetPlane();            
                //We discard any planes of this bounding box which are not facing away from the ray's origin.                
                if (Plane.DotCoordinate(plane, ray.Position) > 0)
                {
                    //We return the intersection point of the first plane intersected.
                    if (quads[i].Intersects(ray, out point))
                        return true;
                }
            }
            return false;
        }       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is BoundingBoxOA)
                return Equals((BoundingBoxOA)obj);
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool Equals(BoundingBoxOA box)
        {
            Vector3[] _corners = GetCorners();
            Vector3[] box_corners = box.GetCorners();
            for (int i = 0; i < _corners.Length; i++)
                if (_corners[i] != box_corners[i])
                    return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns planes in the order of Front, Back, Top, Bottom, Left, Right.
        /// </summary>
        /// <returns></returns>
        public Plane[] GetPlanes()
        {
            return GetQuads().Select(q => q.GetPlane()).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BoundingQuadOA[] GetQuads()
        {
            //******************************************************
            //NOTE:  
            //ASSUMING Left-Handed system, clockwise winding order.
            //If viewer is facing +z direction then...
            //back 0, 1, 2, 3 - starting from top left.
            //front 4, 5, 6, 7 - starting from top left.
            //******************************************************
            Vector3[] _corners = GetCorners();
            BoundingQuadOA front, back, top, bottom, left, right;
            front = new BoundingQuadOA(_corners[4], _corners[5], _corners[6], _corners[7]);
            back = new BoundingQuadOA(_corners[1], _corners[0], _corners[3], _corners[2]);
            top = new BoundingQuadOA(_corners[0], _corners[1], _corners[5], _corners[4]);
            bottom = new BoundingQuadOA(_corners[2], _corners[3], _corners[7], _corners[6]);
            left = new BoundingQuadOA(_corners[0], _corners[4], _corners[7], _corners[3]);
            right = new BoundingQuadOA(_corners[5], _corners[1], _corners[2], _corners[6]);
            return new BoundingQuadOA[] { front, back, top, bottom, left, right };
        }

        public Vector3[] GetCorners()
        {
            //******************************************************
            //NOTE:  
            //ASSUMING Left-Handed system, clockwise winding order.
            //If viewer is facing +z direction then...
            //back 0, 1, 2, 3 - starting from top left.
            //front 4, 5, 6, 7 - starting from top left.
            //******************************************************
            return new Vector3[]
            {
                BackTopLeft,
                BackTopRight,
                BackBottomRight,
                BackBottomLeft,
                FrontTopLeft,
                FrontTopRight,
                FrontBottomRight,
                FrontBottomLeft
            };
        }

        public BoundingBoxOA Transform(Matrix m)
        {
            return new BoundingBoxOA()
            {
                BackBottomLeft = Vector3.TransformCoordinate(this.BackBottomLeft, m),
                BackBottomRight = Vector3.TransformCoordinate(BackBottomRight, m),
                BackTopLeft = Vector3.TransformCoordinate(BackTopLeft, m),
                BackTopRight = Vector3.TransformCoordinate(BackTopRight, m),
                FrontBottomLeft = Vector3.TransformCoordinate(FrontBottomLeft, m),
                FrontBottomRight = Vector3.TransformCoordinate(FrontBottomRight, m),
                FrontTopLeft = Vector3.TransformCoordinate(FrontTopLeft, m),
                FrontTopRight = Vector3.TransformCoordinate(FrontTopRight, m),
            };
        }

        public void Inflate(float dx, float dy, float dz)
        {
            BackTopLeft += new Vector3(-dx, dy, dz);
            BackTopRight += new Vector3(dx, dy, dz);
            BackBottomRight += new Vector3(dx, -dy, dz);
            BackBottomLeft += new Vector3(-dx, -dy, dz);

            FrontTopLeft += new Vector3(-dx, dy, -dz);
            FrontTopRight += new Vector3(dx, dy, -dz);
            FrontBottomRight += new Vector3(dx, -dy, -dz);
            FrontBottomLeft += new Vector3(-dx, -dy, -dz);
        }

        public BoundingBoxOA Inflate(ref BoundingBoxOA box, float dx, float dy, float dz)
        {
            BoundingBoxOA inflatedBox = box;
            inflatedBox.Inflate(dx, dy, dz);
            return inflatedBox;
        }
    }

}
