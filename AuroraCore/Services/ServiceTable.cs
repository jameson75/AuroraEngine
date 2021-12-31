using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CipherPark.KillScript.Core.Services
{
    ///////////////////////////////////////////////////////////////////////////////
    // Developer: Eugene Adams
    // Company: Cipher Park
    // Copyright © 2010-2013
    // Angel Jacket by Cipher Park is licensed under 
    // a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
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
