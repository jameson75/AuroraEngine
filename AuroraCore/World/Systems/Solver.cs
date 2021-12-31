using System;
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
    public abstract class ParticleSolver
    {
        public abstract void Step(GameTime time, ParticleSystem system);
        public abstract void Reset();
        public bool IsComplete { get; protected set; }
    }
}
