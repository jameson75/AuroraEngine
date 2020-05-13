using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.Systems;
using CipherPark.AngelJacket.Core.World.Geometry;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Animation.Controllers
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

        public override void Reset() 
        {
            _children.ForEach(c => c.Reset());
        }

        public override void Update(GameTime gameTime)
        {
            //**********************************************************************************
            //NOTE: We use an auxilary controller collection to enumerate through, in 
            //the event that an updated controller alters this Simulator's Animation Controllers
            //collection.
            //**********************************************************************************
            List<ISimulatorController> auxAnimationControllers = new List<ISimulatorController>(_children);
            foreach (ISimulatorController controller in auxAnimationControllers)
            {
                controller.Update(gameTime);
                if (controller.IsSimulationComplete)
                    _children.Remove(controller);
            }

            if (_children.Count == 0 && !IsSimulationComplete)
                OnSimulationComplete();
        }

        public List<ISimulatorController> Children { get { return _children; } }
    }
}
