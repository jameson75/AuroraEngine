using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using DXBuffer = SharpDX.Direct3D11.Buffer;
using CipherPark.KillScript.Core.World.Geometry;
using CipherPark.KillScript.Core.Utils;
using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.Animation.Controllers;
using CipherPark.KillScript.Core.Effects;

namespace CipherPark.KillScript.Core.Systems
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
