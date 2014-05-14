using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Kinetics;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public class GridDeformer : ParticleKeyframeSolver
    {
        public ITransformable TrackedNode { get; set; }
        public int MaxGridRows { get; set; }

        public GridDeformer()
        { }

        public override void Step(GameTime gameTime, ParticleSystem system)
        {
            GridForm form = system as GridForm;
            if (form != null)
            {
                //if a clip range has not been initialized or the clip range 
                if (form.RowClipRange == null)
                    form.RowClipRange = new Range(0, MaxGridRows);

                //Get the tracked node's transform in the GridForm-Space.
                Transform fsTrackedNodeTransform = form.WorldToLocal(TrackedNode.ParentToWorld(TrackedNode.Transform));
                int targetRow = form.CalculateRowFromZ(fsTrackedNodeTransform.Translation.Z);        

                if(targetRow > form.RowClipRange.Value.Max)
                {
                    form.RowClipRange = new Range(targetRow, targetRow - MaxGridRows);
                    List<Particle> targetRowElements = form.GetRowElements(targetRow);
                    foreach (Particle element in targetRowElements)
                    {
                        TransformAnimation animation = new TransformAnimation();
                        animation.SetKeyFrame(new AnimationKeyFrame(0, new Transform(element.Transform.Rotation, element.Transform.Translation + new Vector3(-200, 0, 0))));
                        animation.SetKeyFrame(new AnimationKeyFrame(100, element.Transform));
                        KeyframeAnimationController animationController = new KeyframeAnimationController();
                        animationController.Animation = animation;
                        animationController.Target = element;
                        Controllers.Add(animationController);
                    }

                    if (form.Dimensions.Z > (float)MaxGridRows)
                        form.HideRow((int)form.Dimensions.Z - (MaxGridRows + 1));
                }
            }
            base.Step(gameTime, system);
        }
     
        //public override void Step(GameTime gameTime, ParticleSystem system)
        //{           
        //    GridForm form = system as GridForm;
        //    if (form != null)
        //    {
        //        Transform fsTrackedNodeTransform = form.WorldToLocal(TrackedNode.ParentToWorld(TrackedNode.Transform));
        //        float gridFrontPlane = form.BoundingBox.GetLengthZ();
        //        if (fsTrackedNodeTransform.Translation.Z >= gridFrontPlane)
        //        {
        //            form.AddBackRow();
        //            List<Particle> addedList = form.GetRow((int)form.Dimensions.Z - 1);
        //            foreach (Particle p in addedList)
        //            {                        
        //                TransformAnimation animation = new TransformAnimation();
        //                animation.SetKeyFrame(new AnimationKeyFrame(0, new Transform(p.Transform.Rotation, p.Transform.Translation + new Vector3(-200, 0, 0))));
        //                animation.SetKeyFrame(new AnimationKeyFrame(100, p.Transform));
        //                KeyframeAnimationController animationController = new KeyframeAnimationController();
        //                animationController.Animation = animation;
        //                animationController.Target = p;
        //                Controllers.Add(animationController);
        //            }
        //            if (form.Dimensions.Z > (float)MaxGridRows)
        //                form.HideRow((int)form.Dimensions.Z  - (MaxGridRows + 1));
        //        }
        //    }
        //    base.Step(gameTime, system);
        //}              
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
