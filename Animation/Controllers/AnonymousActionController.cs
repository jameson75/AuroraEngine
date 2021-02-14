using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.Animation.Controllers
{
    public class AnonymousActionController : SimulatorController
    {
        private long? _animationStartTime = null;
        private long? _lastTriggerTime = null;
        private Random _rand = new Random();

        public ulong Frequency { get; set; }

        public bool FireImmediately { get; set; }

        public Action<SimulatorController> Action { get; set; }

        protected override void OnSimulationReset()
        {
            _animationStartTime = null;
            _lastTriggerTime = null;
        }

        public override void Update(GameTime gameTime)
        {
            if (_animationStartTime == null)
                _animationStartTime = gameTime.GetTotalSimtime();           

            ulong animationElapsedTime = (ulong)(gameTime.GetTotalSimtime() - _animationStartTime.Value);

            ulong triggerElapsedTime = (_lastTriggerTime != null ) ? (ulong)(gameTime.GetTotalSimtime() - _lastTriggerTime.Value) : animationElapsedTime;

            if (triggerElapsedTime >= Frequency || (_lastTriggerTime == null && FireImmediately))
            {
                if(Action != null)
                    Action(this);
                
                _lastTriggerTime = gameTime.GetTotalSimtime();
            }                                   
        }
    }
}
