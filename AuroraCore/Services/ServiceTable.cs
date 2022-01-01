using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.Aurora.Core.Services
{
    ///////////////////////////////////////////////////////////////////////////////
    // Developer: Eugene Adams
    // 
    // Copyright © 2010-2013
    // Aurora Engine is licensed under 
    // MIT License.
    ///////////////////////////////////////////////////////////////////////////////

    public class ServiceTable
    {
        private List<object> _services = new List<object>();

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public object GetService(Type tService)
        {
            foreach (object service in _services)
                if (tService.IsInstanceOfType(service))
                    return service;
            return null;
        }

        public void RegisterService(object inputService)
        {
            if( GetService(inputService.GetType()) == null )
                _services.Add(inputService);
            else
                throw new InvalidOperationException("Service already registered.");
        }
    }
}
