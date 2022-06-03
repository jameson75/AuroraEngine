///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Geometry
{
    public static class BufferResizeCalculator
    {
        public static int Calculate(int sourceLength, int destinationActualLength, int destinationLength, int incrementBlockSize)
        {
            var shortFall = sourceLength - destinationActualLength + destinationLength;
            var increment = ((shortFall / incrementBlockSize) + (shortFall % incrementBlockSize == 0 ? 0 : 1)) * incrementBlockSize;
            var newSize = destinationActualLength + increment;
            return newSize;
        }
    }
}
