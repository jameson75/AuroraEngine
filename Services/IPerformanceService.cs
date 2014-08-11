using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.AngelJacket.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceService
    {
        /// <summary>
        /// 
        /// </summary>
        long MaxFrameRate { get; }

        /// <summary>
        /// 
        /// </summary>
        long ActualFrameRate { get; }
    }
}
