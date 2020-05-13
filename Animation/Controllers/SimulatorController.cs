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

namespace CipherPark.AngelJacket.Core.Animation
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISimulatorController
    {       
        void Reset();
        void Update(GameTime gameTime);
        bool IsSimulationComplete { get; }
        void SignalComplete();
        event EventHandler SimulationComplete;
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class SimulatorController : ISimulatorController
    {
        private bool _isSimulationComplete = false;
        
        public bool IsSimulationComplete
        {
            get { return _isSimulationComplete; }
        }

        public void SignalComplete()
        {
            OnSimulationComplete();
        }

        public abstract void Reset();
        
        public abstract void Update(GameTime gameTime);
        
        public event EventHandler SimulationComplete;

        protected virtual void OnSimulationComplete()
        {
            _isSimulationComplete = true;
            EventHandler handler = SimulationComplete;
            if (handler != null)
                SimulationComplete(this, EventArgs.Empty);
        }        
    }   
}