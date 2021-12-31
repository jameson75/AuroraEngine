using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class InstanceWorldObjectRenderer : IRenderer
    {
        private IGameApp _game = null;
        private Model _model = null;
        private List<InstancePlaceHolderWorldObject> _placeHolders = new List<InstancePlaceHolderWorldObject>();

        public List<InstancePlaceHolderWorldObject> PlaceHolders { get { return _placeHolders; } }

        protected InstanceWorldObjectRenderer(Model model)
        {
            _model = model;
            _game = model.Game;
        }

        public static InstanceWorldObjectRenderer Create(Model model)
        {
            return new InstanceWorldObjectRenderer(model);
        }

        public SurfaceEffect Effect
        {
            get
            {
                return _model.Effect;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_model is BasicModel)
                ((BasicModel)_model).UpdateInstanceData(PlaceHolders.Select(p => p.WorldTransform().ToMatrix()).ToArray());

            else if (_model is ComplexModel)
            {
                var referenceNames = _placeHolders.Select(p => p.ReferenceObjectName).Distinct();
                foreach (string referenceName in referenceNames)
                    ((ComplexModel)_model).UpdateInstanceData(referenceName,
                                                               PlaceHolders.Where(p => p.ReferenceObjectName == referenceName)
                                                                           .Select(p => p.WorldTransform().ToMatrix())
                                                                           .ToArray());
            }
        }

        public void Draw(GameTime gameTime)
        {
            Effect.World = Matrix.Identity;
            _model.Draw(gameTime);
        }
    }
}
