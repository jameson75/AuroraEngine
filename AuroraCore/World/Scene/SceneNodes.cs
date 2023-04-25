using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Scene
{
    public class SceneNodes :  ObservableCollection<SceneNode>
    {   
        public SceneNodes()
        { }    

        public SceneNodes(IEnumerable<SceneNode> nodes)
        {
            AddRange(nodes);
        }
       
        public void AddRange(IEnumerable<SceneNode> nodes)
        {
            foreach( SceneNode node in nodes )
                this.Add(node);
        }

        public SceneNode this[string name]
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                    if (this[i].Name == name)
                        return this[i];
                throw new IndexOutOfRangeException();
            }
        }
    }    
}
