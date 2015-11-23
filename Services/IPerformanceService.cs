using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace CipherPark.AngelJacket.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceService
    {
        void Update(GameTime gameTime);
        ReadOnlyCollection<Metric> GetAllMetrics();
        Metric GetMetric(string name, string group = null);
        void UpdateMetric(string name, string group, double value);
        void UpdateMetric(string name, double value);
    }

    public class Metric
    {
        private Queue<double> _samples = new Queue<double>();
        public string Group { get; set; }
        public string Name { get; set; }
        public double Last { get; set; }
        public double Average { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public Queue<double> Samples { get { return _samples; } }
    }
}