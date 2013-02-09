
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Utils.Toolkit;
using CipherPark.AngelJacket.Core.Animation;

namespace CipherPark.AngelJacket.Core.UI.Animation
{   
    public abstract class UIControlAnimation : PropertyGroupAnimation
    {       
        private long? _lastGameTime = null;

        public UIControlAnimation()
        {
           
        }

        public virtual void Start()
        {
            _lastGameTime = null;
        }

        public virtual void UpdateControl(long gameTime, UIControl control)
        {
            if (_lastGameTime == null)
                _lastGameTime = gameTime;

            ulong timeT = (ulong)(gameTime - _lastGameTime.Value);

            if (TargetPropertyExists(UIControlPropertyNames.Enabled))
                control.Enabled = GetPropertyBooleanValueAtT(UIControlPropertyNames.Enabled, timeT);

            if (TargetPropertyExists(UIControlPropertyNames.Position))
                control.Position = GetPropertyDrawingPointValueAtT(UIControlPropertyNames.Position, timeT);

            if (TargetPropertyExists(UIControlPropertyNames.Size))
                control.Size = GetPropertyDrawingSizeValueAtT(UIControlPropertyNames.Size, timeT);

            if (TargetPropertyExists(UIControlPropertyNames.Visible))
                control.Visible = GetPropertyBooleanValueAtT(UIControlPropertyNames.Visible, timeT);

            if (TargetPropertyExists(UIControlPropertyNames.ZOrder))
                control.ZOrder = GetPropertyFloatValueAtT(UIControlPropertyNames.ZOrder, timeT);

            _lastGameTime = gameTime;
        }

        public static class UIControlPropertyNames
        {
            public const string Enabled = "Enabled";
            public const string Position = "Position";
            public const string Size = "Size";
            public const string Visible = "Visible";
            public const string ZOrder = "ZOrder";
        }
    }

    //public abstract class UIContentAnimation : PropertyGroupAnimation
    //{
    //    public SpriteSortMode? SpriteSortMode { get; set; }
    //    public BlendState BlendState { get; set; }
    //    public SamplerState SamplerState { get; set; }
    //    public DepthStencilState DepthStencilState { get; set; }
    //    public RasterizerState RasterizerState { get; set; }
    //    public Action CustomShaderCallback { get; set; }
    //    public Matrix? TransformationMatrix { get; set; }
    //}

    //public class ListControlItemEffect : PropertyGroupAnimation
    //{

    //}   
}