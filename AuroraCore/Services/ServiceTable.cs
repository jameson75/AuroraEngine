using System;
using System.Collections.Generic;

namespace CipherPark.Aurora.Core.Services
{
    ///////////////////////////////////////////////////////////////////////////////
    // Developer: Eugene Adams
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
            return FindService(tService);
        }

        public void RegisterService(object service)
        {
            var tService = service.GetType();
            if( FindService(tService) == null )
                _services.Add(service);
            else
                throw new InvalidOperationException($"Service {tService.Name} already registered.");
        }

        public void UnregisterService<T>()
        {
            UnregisterService(typeof(T));
        }

        public void UnregisterService(Type tService)
        {
            var service = FindService(tService);
            if (service != null)
                _services.Remove(service);
            else
                throw new InvalidOperationException($"Service {tService.Name} was not registered.");
        }

        private object FindService(Type tService)
        {
            foreach (object service in _services)
                if (tService.IsInstanceOfType(service))
                    return service;
            return null;
        }
    }
}
