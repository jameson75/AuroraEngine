﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using DXBuffer = SharpDX.Direct3D11.Buffer;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Animation.Controllers;
using CipherPark.Aurora.Core.Effects;

namespace CipherPark.Aurora.Core.Systems
{
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
                c.Update(time);

            IsComplete = _controllers.All(c => c.IsSimulationFinal);
        }
    } 
}
