using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public class OutlineEffect : SurfaceEffect
    {
        private FlatEffect basicEffect = null;
        private bool _isRestoreRequired = false;
        private RasterizerState oldRasterizerState = null;

        public OutlineEffect(IGameApp game)
            : base(game)
        {
            basicEffect = new FlatEffect(game, SurfaceVertexType.PositionColor);                     
        }

        public override byte[] GetVertexShaderByteCode()
        {
            return basicEffect.GetVertexShaderByteCode();
        }

        public override void Apply()
        {
            basicEffect.World = Matrix.Scaling(1.05f) * this.World;
            basicEffect.View = this.View;
            basicEffect.Projection = this.Projection;
            oldRasterizerState = GraphicsDevice.ImmediateContext.Rasterizer.State;
            RasterizerStateDescription newRasterizerStateDesc = (oldRasterizerState != null) ? oldRasterizerState.Description : RasterizerStateDescription.Default();
            newRasterizerStateDesc.IsFrontCounterClockwise = true;
            GraphicsDevice.ImmediateContext.Rasterizer.State = new RasterizerState(GraphicsDevice, newRasterizerStateDesc);
            basicEffect.Apply();
            _isRestoreRequired = true;
            base.Apply();
        }

        public override void Restore()
        {
            if (_isRestoreRequired)
            {
                GraphicsDevice.ImmediateContext.Rasterizer.State = oldRasterizerState;
            }
            base.Restore();
        }
    }
}
