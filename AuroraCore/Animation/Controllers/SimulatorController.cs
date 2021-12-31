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

namespace CipherPark.Aurora.Core.Animation
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISimulatorController
    {       
        void Reset();
        void Update(GameTime gameTime);       
        bool WasSimulationAborted { get; }
        bool IsSimulationFinal { get; }
        bool IsSimulationSuspended { get; }
        void SignalComplete();
        void SuspendSimulation(bool suspend);
        event EventHandler SimulationComplete;
        event EventHandler SimulationAborted;
        event EventHandler SimulationFinal;
        event EventHandler SimulationSuspend;
        event EventHandler SimulationContinue;
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class SimulatorController : ISimulatorController
    {                 
        public bool WasSimulationAborted { get; private set; }
        public bool IsSimulationFinal { get; private set; }
        public bool IsSimulationSuspended { get; private set; }
        
        public abstract void Update(GameTime gameTime);

        public void SignalComplete()
        {
            OnSimulationComplete();
        }

        public void SignalAbort()
        {
            OnSimulationAbort();
        }

        public void SuspendSimulation(bool suspend)
        {
            if (suspend && !IsSimulationSuspended)
                OnSimulationSuspend();
            else if(!suspend && IsSimulationSuspended )
                OnSimulationContinue();
        }        
        
        public void Reset()
        {
            IsSimulationSuspended = false;
            IsSimulationFinal = false;
            WasSimulationAborted = false;
            OnSimulationReset();
        }
        
        public event EventHandler SimulationComplete;

        public event EventHandler SimulationAborted;

        public event EventHandler SimulationFinal;

        public event EventHandler SimulationSuspend;

        public event EventHandler SimulationContinue;

        protected virtual void OnSimulationReset() { }

        protected virtual void OnSimulationComplete()
        {
            IsSimulationFinal = true;
            SimulationComplete?.Invoke(this, EventArgs.Empty);
            SimulationFinal?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSimulationAbort()
        {
            IsSimulationFinal = true;           
            SimulationAborted?.Invoke(this, EventArgs.Empty);
            WasSimulationAborted = true;
            SimulationFinal?.Invoke(this, EventArgs.Empty);
        }   

        protected virtual void OnSimulationSuspend()
        {
            IsSimulationSuspended = true;
            SimulationSuspend?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSimulationContinue()
        {
            IsSimulationSuspended = false;
            SimulationContinue?.Invoke(this, EventArgs.Empty);
        }
    }   
}