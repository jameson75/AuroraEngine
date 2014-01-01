using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Kinetics;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Content;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Kinetics.Animation
{
    public class ParticleAnimationBuilder
    {
        //public static CompositeAnimationController BuildQuadMorphAnimation(RectangleF quadDimension, ParticleAnimationPresets preset, ulong totalAnimationTime, Emitter emitter, ParticleKeyframeSolver solver = null)
        //{
        //    CompositeAnimationController masterController = new CompositeAnimationController();
        //    ParticleKeyframeSolver _solver = (solver != null) ? solver : new ParticleKeyframeSolver();              
        //    EmitterAnimationController emitterController = new EmitterAnimationController();
        //    emitterController.IsExplicitMode = true;
        //    emitterController.Target = emitter;
        //    //emitterController.Solver = _solver;
        //    Vector3[] quadPoints = ContentBuilder.CreateQuadPoints(quadDimension, true); //NOTE: We expect 5 points.
        //    const int i_center = 4;
        //    switch(ParticleAnimationPresets.QuadMorphExpand)
        //    {
        //        case ParticleAnimationPresets.QuadMorphExpand:
        //            List<EmitterAction> actions = new List<EmitterAction>();
        //            //1. Create Action which transforms emitter to center of quad...
        //            Transform centerPointTransform = new Transform(quadPoints[i_center]);
        //            EmitterAction transformEmitterAction = new EmitterAction(0, centerPointTransform);
        //            actions.Add(transformEmitterAction);
        //            //2. Create Action which, explicitly, emits four particles from the emitter.
        //            List<Particle> fourParticles = emitter.CreateParticles(4);
        //            EmitterAction emitFourParticlesAction = new EmitterAction(0, fourParticles, EmitterAction.EmitterTask.EmitExplicit);
        //            actions.Add(emitFourParticlesAction);
        //            //3. Create Action which links particles 0->1, 1->2, 2->3 and 3->0.
        //            Particle[] linkedParticles = new int[] { 0, 1, 1, 2, 2, 3, 3, 0 }.Select(i => fourParticles[i]).ToArray();
        //            EmitterAction linkParticlesAction = new EmitterAction(0, linkedParticles, EmitterAction.EmitterTask.Link);
        //            actions.Add(linkParticlesAction);
        //            //4. Create an animation which will animate the four particles from the center of the quad
        //            //(where they were emitted) to each corner of the quad.
        //            //ParticleKeyframeSolver.AnimationLookup targetAnimations = new ParticleKeyframeSolver.AnimationLookup();
        //            List<KeyframeAnimationController> keyFrameControllers = new List<KeyframeAnimationController>();
        //            for (int i = 0; i < 4; i++)
        //            {
        //                AnimationKeyFrame[] f = new AnimationKeyFrame[2];
        //                f[0] = new AnimationKeyFrame(0, centerPointTransform);
        //                Transform finalTransform = new Transform(quadPoints[i]);
        //                f[1] = new AnimationKeyFrame(totalAnimationTime, finalTransform);
        //                TransformAnimation animation = new TransformAnimation(f);                       
        //                //targetAnimations.Add(fourParticles[i], animation);
        //                keyFrameControllers.Add(new KeyframeAnimationController(animation, fourParticles[i]));
        //            }
        //            //Set the actions and animation to the controller.
        //            emitterController.ExplicitTasks = actions;
        //            //((ParticleKeyframeSolver)controller.Solver).TargetAnimations = targetAnimations;
        //            masterController.Children.AddRange(keyFrameControllers);                   
        //            break;
        //        default:
        //            throw new ArgumentException("This method does not support the specified preset.");
        //    }

        //    //return emitterController;
        //    masterController.Children.Add(emitterController);
        //    return masterController;
        //}

        //public EmitterAnimationController BuildMeshSweepAnimation(Mesh mesh, ParticleSweepDirection direction, ulong totalAnimationTime)
        //{
        //    EmitterAnimationController controller = new EmitterAnimationController();
            
        //    return controller;
        //}
    }

    public enum ParticleAnimationPresets
    {
        QuadMorphExpand
    }

    public enum ParticleSweepDirection
    {        
        Left,
        Right,
        Up,
        Down,
        Forward,
        Backward
    }
}
