using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.AngelJacket.Core.Services
{
    public class ServiceTable
    {
        private List<object> _services = new List<object>();

        public object GetService(Type tService)
        {
            foreach (object service in _services)
                if (tService.IsInstanceOfType(service))
                    return service;
            return null;
        }
    }
}
