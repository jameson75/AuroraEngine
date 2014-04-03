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

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    public class Deformer : ParticleKeyframeSolver
    {
        public ITransformable TrackedNode { get; set; }

        public override void Step(ulong time, ParticleSystem system)
        {           
            GridForm form = system as GridForm;
            if (form != null)
            {
                Transform fsTrackedNodeTransform = form.WorldToParent(TrackedNode.ParentToWorld(TrackedNode.Transform));
                float gridFrontPlane = form.BoundingBox.GetLengthZ();
                if (fsTrackedNodeTransform.Translation.Z <= gridFrontPlane)
                {
                    Deformer.AnimateGrid(form);
                }
            }

            base.Step(time, system);
        }       

        public static AnimateGrid(GridForm form)
        {
            foreach (var e in elements)
                        MakeInvisible(e);
                    CreateTransformsToHeadRow(elements);

                    var tail = form.GetRow(form.RowCount - 1);
                    CreateTransformsFromTailRow(tail);
                    foreach (var e in tail)
                        MakeInvisible(e);
                    form.RemoveTailRow();
        }
    }
}
