using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using SharpDX;
using SharpDX.Direct3D11;
using DXBuffer = SharpDX.Direct3D11.Buffer;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Effects;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Systems
{
    /// <summary>
    /// 
    /// </summary>
    public class Emitter : ITransformable
    {      
        #region ITransformable Members
        public Transform Transform { get; set; }
        public ITransformable TransformableParent { get; set; }
        #endregion    

        public Vector3 EmissionDirection { get; set; }
        
        public float EmissionRangePitch { get; set; }
        
        public float EmissionRangeYaw { get; set; }

        public int BirthRate { get; set; }
        
        public int BirthRateRandomness { get; set; }
        
        public ulong Life { get; set; }
        
        public int LifeRandomness { get; set; }
        
        public float InitialVelocity { get; set; }
        
        public int InitialVelocityRandomness { get; set; }   

        public ParticleDescription DefaultParticleDescription { get; set; }                     

        //public List<Particle> CreateParticles(int count)
        //{
        //    List<Particle> pList = new List<Particle>();
        //    for (int i = 0; i < count; i++)
        //    {              
        //        Particle p = new Particle();
        //        p.Life = 0;
        //        p.Age = 0;
        //        p.Velocity = 0;
        //        p.Transform = this.Transform;
        //        p.Description = null;
        //        pList.Add(p);
        //    }
        //    return pList;           
        //}       

        public List<Particle> Spawn(ParticleDescription description, GameTime gameTime, int count = 0)
        {          
            ParticleDescription particleDesc = (description != null) ? description : this.DefaultParticleDescription;            
            Random randomGen = new Random();
            List<Particle> pList = new List<Particle>();            
            int birthCount = (count != 0) ? count : (this.BirthRateRandomness == 0) ? this.BirthRate : 
                this.BirthRate + randomGen.Next(this.BirthRateRandomness);                
            for (int i = 0; i < birthCount; i++)
            {
                ulong randomLife = (this.LifeRandomness == 0) ? this.Life :
                    this.Life + (ulong)randomGen.Next(this.LifeRandomness);
                float randomVelocity = (this.InitialVelocityRandomness == 0) ? this.InitialVelocity :
                    this.InitialVelocity + (float)randomGen.Next(this.InitialVelocityRandomness);               
                Particle p = new Particle();
                p.Life = randomLife;
                p.Birth = gameTime.GetTotalSimtime();                
                p.Velocity = randomVelocity;
                p.Transform = this.Transform;
                p.Description = particleDesc;                
                pList.Add(p);
            }
            return pList;
        }       
    }        

    /*
    /// <summary>
    /// 
    /// </summary>
    public class EmitterAction
    {
        public EmitterAction() { }
        public EmitterAction(ulong time, EmitterTask task) { Time = time; Task = task; }
        public EmitterAction(ulong time, ParticleDescription customParticleDescription) { Time = time; CustomParticleDescriptionArg = customParticleDescription; Task = EmitterTask.EmitCustom; }
        public EmitterAction(ulong time, IEnumerable<Particle> particleArgs, EmitterTask task) { Time = time; ParticleArgs = particleArgs; Task = task; }
        public EmitterAction(ulong time, Transform transform) { Time = time; Transform = transform; Task = EmitterTask.Transform; }
        public ulong Time { get; set; }
        public ParticleDescription CustomParticleDescriptionArg { get; set; }
        public IEnumerable<Particle> ParticleArgs { get; set; }
        public Transform Transform { get; set; }
        public enum EmitterTask
        {
            Emit,
            EmitCustom,
            EmitExplicit,
            Kill,
            KillAll,
            Link,
            Transform
        }
        public EmitterTask Task { get; set; }
    }    
    */
}
    
