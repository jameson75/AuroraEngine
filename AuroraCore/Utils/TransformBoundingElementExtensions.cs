using System;
using System.Linq;
using SharpDX;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core;
using CipherPark.Aurora.Core.World;

namespace CipherPark.Aurora.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class TransformableExtensionForBoundingElements
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transformable"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static BoundingBoxOA LocalToWorldBoundingBox(this ITransformable transformable, BoundingBox box)
        {            
            var boundingBoxOA = BoundingBoxOA.FromAABoundingBox(box);
            return LocalToWorldBoundingBox(transformable, boundingBoxOA);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transformable"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static BoundingBoxOA LocalToWorldBoundingBox(this ITransformable transformable, BoundingBoxOA box)
        {                  
            return box.Transform(transformable.WorldTransform().ToMatrix());
        }
    }      
    
    public static class CollisionDebugWriter
    {
        public static bool WriteEnabled = false;

        private static System.Text.StringBuilder bufferedOut = new System.Text.StringBuilder();

        internal static void IntersectsBoxInfo(int i, BoundingQuadOA quad, Ray ray)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("*** Intersects Box Info ***");
            string quadName = null;
            switch (i)
            {
                case 0:
                    quadName = "Front";
                    break;
                case 1:
                    quadName = "Back";
                    break;
                case 2:
                    quadName = "Top";
                    break;
                case 3:
                    quadName = "Bottom";
                    break;
                case 4:
                    quadName = "Left";
                    break;
                case 5:
                    quadName = "Right";
                    break;               
            }

            sb.AppendLineFormat("Quad Index: {0}", i);
            sb.AppendLineFormat("Quad Name: {0}", quadName);
            sb.AppendLineFormat("Quad Details: {0}", quad);
            sb.AppendLineFormat("Quad Plane: {0}", quad.GetPlane());
            sb.AppendLineFormat("Ray: {0}", ray);
            float d = Plane.DotCoordinate(quad.GetPlane(), ray.Position);
            sb.AppendLineFormat("Does Quad face ray origin? {1}, (Dot product is {0})", d, d >= 0 ? "yes" : "no");
            WriteInfo(sb);
        }

        internal static void QuadContainsCoplanarPointInfo(Vector3 crossProduct, Plane quadPlane, Vector3 intersectionPoint)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("*** Quad Contains Coplanar Point Info *** ");
            sb.AppendLineFormat("Test Cross Product: {0}; Quad Plane: {1}", crossProduct, quadPlane);
            string result = crossProduct != Vector3.Normalize(quadPlane.Normal) ? "false" : string.Format("looks like it so far {0}- still testing", crossProduct == Vector3.Zero ? "(co-linear) " : null);           
            sb.AppendLineFormat("Plane Intersection Point: {0}", intersectionPoint);
            sb.AppendLineFormat("Is intersection point within the quad? {0}", result); 
            WriteInfo(sb);
        }

        internal static void QuadContainsCoplanarSuccessInfo()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("*** Contains Coplanar Info ***");
            sb.AppendLine("TRUE!!");
            WriteInfo(sb);
        }

        internal static void BoxCornerInfo(int i, Vector3 position, Vector3 normalArB)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("*** Box Corner Info ***");
            string cornerName = null;
            switch (i)
            {
                case 0:
                    cornerName = "Back_Top_Left";
                    break;
                case 1:
                    cornerName = "Back_Top_Right";
                    break;
                case 2:
                    cornerName = "Back_Bottom_Right";
                    break;
                case 3:
                    cornerName = "Back_Bottom_Left";
                    break;
                case 4:
                    cornerName = "Front_Top_Left";
                    break;
                case 5:
                    cornerName = "Front_Top_Right";
                    break;
                case 6:
                    cornerName = "Front_Bottom_Right";
                    break;
                case 7:
                    cornerName = "Front_Bottom_Left";
                    break;
            }
           
            sb.AppendLineFormat("Corner Index: {0}; ", i);
            sb.AppendLineFormat("Corner Name: \"{0}\"; ", cornerName);
            sb.AppendLineFormat("Corner World Position: {0}; ", position);
            sb.AppendLineFormat("Corner Relative World Normal {0}; ", normalArB);
            WriteInfo(sb);
        }

        public static void WriteInfo(System.Text.StringBuilder sb)
        {
            if (WriteEnabled)
                Console.Write(sb);
            bufferedOut.Append(sb);
        }

        public static void AppendLineFormat(this System.Text.StringBuilder sb, string format, params object[] args)
        {
            sb.AppendLine(string.Format(format, args));
        }

        public static void ClearBufferedOut()
        {
            bufferedOut.Clear();
        }

        public static void WriteBufferedOut()
        {
            Console.Write(bufferedOut);
        }

        internal static void IntersectsQuadInfo(Ray ray, Plane plane, float dotProduct)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("*** Intersects Quad Info ***");
            sb.AppendLineFormat("Does ray intersect quad's plane? {0}", ray.Intersects(ref plane) || dotProduct == 0 ? "yes... checking if quad contains the point" : "no");
            WriteInfo(sb);
        }
    }
}
