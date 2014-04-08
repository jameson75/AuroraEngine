using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using DXBuffer = SharpDX.Direct3D11.Buffer;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Effects;

namespace CipherPark.AngelJacket.Core.Kinetics
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ParticleSolver
    {
        public abstract void Step(GameTime time, ParticleSystem system);
        public abstract void Reset();
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParticleKeyframeSolver : ParticleSolver
    { 
        protected List<KeyframeAnimationController> Controllers { get { return _controllers; } }      
        
        private List<KeyframeAnimationController> _controllers = null;

        public ParticleKeyframeSolver()
        {
            _controllers = new List<KeyframeAnimationController>();
        }

        public override void Reset()
        {
            _controllers.ForEach(c => c.Reset());
        }

        public override void Step(GameTime time, ParticleSystem system)
        {
            foreach (KeyframeAnimationController c in _controllers)
                c.UpdateAnimation(time);
        }       
    } 
}
