using System.Collections.Generic;
using CipherPark.Aurora.Core.Utils;
using SharpDX;

namespace CipherPark.Aurora.Core.Extensions
{
    public static class PickExtensions
    {
        public static PickInfo GetClosest(this IEnumerable<PickInfo> pickList, Vector3 from)
        {
            PickInfo closestPick = null;
            float closestPickDistanceSq = 0;

            foreach (var pickInfo in pickList)
            {
                var distanceSq = Vector3.DistanceSquared(from, pickInfo.IntersectionPoint);
                if (closestPick == null || distanceSq < closestPickDistanceSq)
                {
                    closestPick = pickInfo;
                    closestPickDistanceSq = distanceSq;
                }
            }
                
            return closestPick;
        }
    }
}
