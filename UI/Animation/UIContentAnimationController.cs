using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Animation
{
    public class UIContentAnimationController<T> : PropertiesAnimationController where T : UIContent
    { 
        private long? _elapsedTime = null;      

        public T Target { get; set; }

        public UIContentAnimationController()
        {

        }

        protected long? ElapsedTime 
        {
             get { return _elapsedTime; }
        }

        public override void Start()
        {
            _elapsedTime = null;            
        }

        public override void UpdateAnimation(long gameTime)
        {
            if (_elapsedTime == null)
                _elapsedTime = gameTime;
            
            ulong timeT = (ulong)(gameTime - _elapsedTime.Value);

            OnUpdateTarget(timeT);

            _elapsedTime = gameTime;
        }

        public virtual void OnUpdateTarget(ulong timeT)
        {
            
        }
    }
}
