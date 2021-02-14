using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.KillScript.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class NullableFloatExtension
    {
        public static bool IsNullOrEqualTo(this Nullable<float> f, float value)
        {
            return (f == null || f.Value == value);
        }

        public static bool IsNullOrLessThan(this Nullable<float> f, float value)
        {
            return (f == null || f.Value < value);
        }

        public static bool IsNullOrGreaterThan(this Nullable<float> f, float value)
        {
            return (f == null || f.Value > value);
        }

        public static bool IsNullOrNotEqualTo(this Nullable<float> f, float value)
        {
            return (f == null || f.Value != value);
        }

        public static bool IsNullOrLessThanOrEqualTo(this Nullable<float> f, float value)
        {
            return (f == null || f.Value <= value);
        }

        public static bool IsNullOrGreaterThanOrEqualTo(this Nullable<float> f, float value)
        {
            return (f == null || f.Value >= value);
        }
    }
}
