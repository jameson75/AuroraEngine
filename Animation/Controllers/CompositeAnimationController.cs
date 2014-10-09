using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.Kinetics;
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
    public class CompositeAnimationController : AnimationController
    {
        List<IAnimationController> _children = new List<IAnimationController>();

        public CompositeAnimationController()
        { }

        public CompositeAnimationController(IEnumerable<IAnimationController> children)
        {
            _children.AddRange(children);
        }

        public override void Reset() 
        {
            _children.ForEach(c => c.Reset());
        }

        public override void UpdateAnimation(GameTime gameTime)
        {
            //**********************************************************************************
            //NOTE: We use an auxilary controller collection to enumerate through, in 
            //the event that an updated controller alters this Simulator's Animation Controllers
            //collection.
            //**********************************************************************************
            List<IAnimationController> auxAnimationControllers = new List<IAnimationController>(_children);
            foreach (IAnimationController controller in auxAnimationControllers)
            {
                controller.UpdateAnimation(gameTime);
                if (controller.IsAnimationComplete)
                    _children.Remove(controller);
            }

            if (_children.Count == 0 && !IsAnimationComplete)
                OnAnimationComplete();
        }

        public List<IAnimationController> Children { get { return _children; } }
    }
}
