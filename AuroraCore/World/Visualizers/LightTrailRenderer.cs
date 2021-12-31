using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Float4 = SharpDX.Vector4;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Effects;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World
{
    public class LightTrailRenderer : IRenderer
    {
        private StreakRenderer _streakRenderer = null;
        private PathTracker _tracker = null;

        private LightTrailRenderer()
        { }

        public static LightTrailRenderer Create(IGameApp game)
        {
            LightTrailRenderer trail = new LightTrailRenderer();
            trail._streakRenderer = StreakRenderer.Create(game);
            trail._streakRenderer.PreserveGeometry = true;
            trail._streakRenderer.StepSize = -1;
            trail.Color = Color.Transparent;
            return trail;
        }

        public void Update(GameTime gameTime)
        {
            if (_tracker != null)
            {
                _tracker.PathNodeMinDistance = NodeLength;
                _tracker.Update(gameTime);

                if (_tracker.Path.NodeCount > MaxNodeCount)
                    _tracker.Path.RemoveNodes(1);

                if (_tracker.Path.NodeCount > 1)
                {
                    _streakRenderer.Path = _tracker.Path;
                    _streakRenderer.Update(gameTime);
                }
            }
        }

        public ITransformable Anchor
        {
            get
            {
                if (_tracker != null)
                    return _tracker.Target;
                else
                    return null;
            }
            set
            {
                if (value == null)
                    _tracker = null;
                else
                    _tracker = new PathTracker()
                    {                       
                        Target = value
                    };
            }
        }

        public Color Color
        {
            get { return _streakRenderer.Color; }
            set { _streakRenderer.Color = value; }
        }

        public void Draw(GameTime gameTime)
        {
            if (_streakRenderer.Path != null)
                _streakRenderer.Draw(gameTime);
        }

        public SurfaceEffect Effect
        {
            get { return _streakRenderer.Effect; }
        }

        public float NodeLength { get; set; }

        public int MaxNodeCount { get; set; }
    }
}
