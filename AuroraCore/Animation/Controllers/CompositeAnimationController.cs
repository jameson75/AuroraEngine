using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.Module;
using CipherPark.Aurora.Core.Systems;
using CipherPark.Aurora.Core.World.Geometry;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Animation.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CompositeAnimationController : SimulatorController
    {
        List<ISimulatorController> _children = new List<ISimulatorController>();       

        public CompositeAnimationController()
        { }

        public CompositeAnimationController(IEnumerable<ISimulatorController> children)
        {            
            _children.AddRange(children);
        }

        protected override void OnSimulationReset() 
        {           
            _children.ForEach(c => c.Reset());
        }

        public override void Update(GameTime gameTime)
        {
            foreach (ISimulatorController controller in _children)
            {
                if(!controller.IsSimulationFinal && !controller.IsSimulationSuspended)
                    controller.Update(gameTime);               
            }

            if (_children.All(c => c.IsSimulationFinal) && !IsSimulationFinal)
            {
                if (_children.Any(c => c.WasSimulationAborted))
                    OnSimulationAbort();
                else
                    OnSimulationComplete();
            }
        }

        public List<ISimulatorController> Children { get { return _children; } }
    }
}
