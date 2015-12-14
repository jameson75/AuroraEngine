using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CipherPark.AngelJacket.Core.World.Collision;

namespace CipherPark.AngelJacket.Core.World
{
    public interface IWorldCollector
    {
        void CollectObject(WorldObject worldObject);
    }

    public class WorldObjectContext
    {
        public WorldObject WorldObject { get; set; }
        public IWorldCollector Collector { get; set; }
        public bool MarkedForCollection { get; set; }       
    }

    public class LifetimeManager
    {
        List<WorldObjectContext> _contexts = new List<WorldObjectContext>();

        public void RegisterCollector(WorldObject worldObject, IWorldCollector collector)
        {
            if (_contexts.Any(c => c.WorldObject == worldObject))
                throw new InvalidOperationException("Context already registered");

            _contexts.Add(new WorldObjectContext()
            {
                WorldObject = worldObject,
                Collector = collector
            });
        }

        public bool IsRegistered(WorldObject worldObject)
        {
            return _contexts.Any(c => c.WorldObject == worldObject);
        }

        public void MarkForCollection(WorldObject worldObject)
        {
            _contexts.First(c => c.WorldObject == worldObject).MarkedForCollection = true;
        }       

        public void Collect()
        {
            List<WorldObjectContext> aux = new List<WorldObjectContext>(_contexts);
            foreach(WorldObjectContext entity in aux)
                if(entity.MarkedForCollection)
                {
                    entity.Collector.CollectObject(entity.WorldObject);
                    _contexts.Remove(entity);                  
                }
        }
    }
}
