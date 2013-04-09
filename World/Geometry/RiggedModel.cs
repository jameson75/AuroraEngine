using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public class RiggedModel : BasicModel
    {
        private Bones _bones = new Bones();

        private Bones Bones { get { return _bones; } }

        public RiggedModel(IGameApp game)
            : base(game)
        { }

        public void Pose(string keyFrameName)
        {

        }
    }

    public class Bone
    {
        public string Name { get; set; }

        public Transform Transform { get; set; }
    }

    public class Bones :  ObservableCollection<Bone>
    {   
        public Bones()
        { }    
       
        public void AddRange(IEnumerable<Bone> bones)
        {
            foreach( Bone bone in bones )
                this.Add(bone);
        }

        public Bone this[string name]
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                    if (this[i].Name == name)
                        return this[i];
                return null;
            }
        }
    }  
}
