using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.KillScript.Core.World.Geometry;
using CipherPark.KillScript.Core.Utils;
using CipherPark.KillScript.Core.Animation;
using CipherPark.KillScript.Core.Animation.Controllers;
using CipherPark.KillScript.Core.Effects;
using CipherPark.KillScript.Core.Systems;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.KillScript.Core.World.Geometry.Modifiers
{
    public class StackGridDeformer : ParticleKeyframeSolver
    {
        public ITransformable TrackedNode { get; set; }

        public int MaxGridRows { get; set; }       

        private Range? RowClipRange { get; set; }

        public StackGridDeformer()
        { }

        public override void Step(GameTime gameTime, ParticleSystem system)
        {
            GridForm form = (GridForm)system;

            if (form != null)
            {
                //If a clip range has not been initialized or the clip range. 
                if (RowClipRange == null)
                    RowClipRange = new Range(0, MaxGridRows - 1);

                //Get the tracked node's transform in the GridForm-Space.
                Transform fsTrackedNodeTransform = form.WorldToLocal(TrackedNode.ParentToWorld(TrackedNode.Transform));

                int targetRow = form.CalculateRowFromZ(fsTrackedNodeTransform.Translation.Z);        

                if(targetRow > RowClipRange.Value.Max)
                {
                    RowClipRange = new Range(targetRow - MaxGridRows + 1, targetRow);
                    List<Particle> targetRowParticles = form.GetRowElements(targetRow);                  
                    foreach (Particle particle in targetRowParticles)
                    {
                        TransformAnimation animation = new TransformAnimation();
                        animation.SetKeyFrame(new AnimationKeyFrame(0, new Transform(particle.Transform.Rotation, particle.Transform.Translation + new Vector3(-800, 0, 0))));
                        animation.SetKeyFrame(new AnimationKeyFrame(300, particle.Transform));
                        KeyframeAnimationController animationController = new KeyframeAnimationController();
                        animationController.Animation = animation;
                        animationController.Target = particle;
                        Controllers.Add(animationController);
                    }
                }
            }
            base.Step(gameTime, system);
        }     
    }

    public static class GridFormExtension
    {
        public static int CalculateRowFromZ(this GridForm form, float z)
        {
            Vector3 cellSize = form.CalculateRenderedCellSize();
            int row = (int)(z / cellSize.Z);
            return (row < form.Dimensions.Z && row >= 0) ? row : -1;
        }
    }
}
