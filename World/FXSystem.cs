using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherPark.AngelJacket.Core.World
{
    public class FXSystem
    {
        private Game _game = null;
        public FXSystem(Game game)
        {
            _game = game;
        }

        public void LaserEtch(ISceneObject target, LaserEtchParameters parameters)
        {

        }

        public void Morph(ISceneObject target, MorphParameters parameters)
        {

        }

        public void Composite(ISceneObject target, CompositeParameters parameters)
        {

        }

        public void Fill(ISceneObject target, FillParameters parameters)
        {

        }
    }

    public class FXSystemParameters
    { }

    public class LaserEtchParameters : FXSystemParameters
    { }

    public class MorphParameters : FXSystemParameters
    { }

    public class CompositeParameters : FXSystemParameters
    { }

    public class FillParameters : FXSystemParameters
    { }
}
