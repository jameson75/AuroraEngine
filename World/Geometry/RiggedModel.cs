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
    /// <summary>
    /// 
    /// </summary>
    public class RiggedModel : BasicModel
    {
        private Bones _bones = new Bones();
        private List<TransformAnimationController> _animationControllers = new List<TransformAnimationController>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        public RiggedModel(IGameApp game)
            : base(game)
        { }    
        
        /// <summary>
        /// 
        /// </summary>
        public Bones Bones { get { return _bones; } }
       
        /// <summary>
        /// 
        /// </summary>
        public List<TransformAnimationController> Animation
        { get { return _animationControllers; } } 
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startKeyFrame"></param>
        /// <returns></returns>
        public List<TransformAnimationController> GetAnimationClip(int startTime, int? endTime = null)
        {
            List<TransformAnimationController> clip = new List<TransformAnimationController>();
            return clip;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Bone : ITransformable
    {
        private Bones _children = new Bones();

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        public Transform Transform { get; set; }
        public Bone Parent { get; set; }
        public Bones Children { get { return _children; } }
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
