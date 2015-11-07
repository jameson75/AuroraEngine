using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.World.Scene;
using CipherPark.AngelJacket.Core.World.Collision;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Animation.Controllers;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Systems;
using CipherPark.AngelJacket.Core.Services;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World
{
    public interface IWeapon
    {
        void Initialize();
        void Fire();
        void Uninitialize();
    }

    public abstract class BasicWeapon : BasicWorldObject, IWeapon
    {
        public BasicWeapon(IGameApp game)
            : base(game)
        { }

        public abstract void Fire();
        public abstract void Initialize();
        public abstract void Uninitialize();
    }

    public class PlayerGun : BasicWeapon
    {
        private bool _isInitialized = false;
        private CompositeAnimationController _animationController = null;
        private GameContext? _gameContext = null;
        private long? _lastFireTime = null;
        private RendererSceneNode _rendererNode = null;
           
        public Model ProjectileInstanceModel { get; set; }        
        public SceneNode ProjectileRootNode { get; set; }
        public Vector3 DischargeLocation { get; set; }
        public long DischargeLatency { get; set; }
        public float ProjectileVelocity { get; set; }

        public PlayerGun( IGameApp game  )
            : base(game)
        { }       

        public override void Fire()
        {
            if (!_isInitialized)
                throw new InvalidOperationException();

            IGameStateService gameStateService =_gameContext.Value.Game.Services.GetService<IGameStateService>();            
            long gameSimTime = gameStateService.GameTime.GetTotalSimtime();
            if (DischargeLatency == 0 ||
                _lastFireTime == null ||
                gameSimTime - _lastFireTime.Value >= DischargeLatency)
            {                
                //Create place holder.
                //--------------------
                InstancePlaceHolderWorldObject projectileWO = new InstancePlaceHolderWorldObject(this.Game, ProjectileInstanceModel.BoundingBox);                                               
               
                //Initialize transform of the new projectile instance place holder
                //----------------------------------------------------------------
                //- Transform it to the world coordinates of the discharge location of this weapon.
                //- Temporarily rotate the projectile to make my sample player-bullet work - We'll remove this code when the model is fixed and this
                //  logic is fully tested.
                Vector3 actionFrameDischargeLocation = ProjectileRootNode.WorldToLocalCoordinate(ContainerNode.LocalToWorldCoordinate(DischargeLocation));                
                projectileWO.Transform = new Transform(Matrix.RotationX(MathUtil.DegreesToRadians(90f)) * Matrix.Translation(actionFrameDischargeLocation));                              

                //Create and register animation for place holder
                //----------------------------------------------
                RigidBodyAnimationController animationController = new RigidBodyAnimationController();
                animationController.Target = projectileWO;
                animationController.Motion = new Motion()
                {
                    Direction = Vector3.UnitZ,
                    LinearVelocity = ProjectileVelocity,
                    TimeDomainUnits = TimeDomainUnits.Seconds
                };
                _gameContext.Value.Simulator.AnimationControllers.Add(animationController);
                
                //Register place holder with this weapons' projectile renderer.
                //(The renderer will render an instance of the projectile mesh at the place holder's location in the scene
                //--------------------------------------------------------------------------------------------------------
                ((InstanceWorldObjectRenderer)_rendererNode.Renderer).PlaceHolders.Add(projectileWO); 

                //Create a bounding box collider for the world object and register it with the collision detector
                //-----------------------------------------------------------------------------------------------
                projectileWO.Collider = new BoxCollider()
                {
                    Box = BoundingBoxOA.FromBox(projectileWO.BoundingBox),
                    //Filter = IsObjectSubjectToCollisionTesting //Filter out the object's we want to collision test against.
                };
                _gameContext.Value.Simulator.CollisionDetector.AddObservedObject(projectileWO);
                _lastFireTime = gameSimTime;
            }
        }

        public override void Initialize()
        {
            //Register our animation controller.
            //----------------------------------
            _animationController = new CompositeAnimationController();
            IGameContextService service = Game.Services.GetService<IGameContextService>();
            _gameContext = service.Context;
            _gameContext.Value.Simulator.AnimationControllers.Add(_animationController);

            //Create the projectile renderer and it to to the scene.
            //------------------------------------------------------
            var renderer = InstanceWorldObjectRenderer.Create(ProjectileInstanceModel);
            _rendererNode = new RendererSceneNode(Game) { Renderer = renderer };
            _gameContext.Value.Scene.Nodes.Add(_rendererNode);

            _isInitialized = true;
        }

        public override void Uninitialize()
        {
            _animationController.SetComplete();
            _gameContext.Value.Scene.Nodes.Remove(_rendererNode);
        }

        public override void Draw(GameTime gameTime)
        {
            //Currently the player gun doesn't have a model associated with it.
            base.Draw(gameTime);
        }
    }

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
            _model.Draw(gameTime);
        }
    }


    public static class ModelExtension
    {
        public static void UpdateInstanceData(this ComplexModel model, string meshName, IEnumerable<Matrix> data)
        {
            Mesh mesh = model.Meshes.FirstOrDefault(m => m.Name == meshName);

            if (mesh.IsInstanced == false || mesh.IsDynamic == false)
                throw new InvalidOperationException("Cannot set instance data to a mesh that is not both dynamic and instanced.");

            mesh.UpdateVertexStream<InstanceVertexData>(data.Select(m => new InstanceVertexData() { Matrix = Matrix.Transpose(m) }).ToArray());
        }

        public static void UpdateInstanceData(this BasicModel model, IEnumerable<Matrix> data)
        {
            if (model.Mesh.IsInstanced == false || model.Mesh.IsDynamic == false)
                throw new InvalidOperationException("Cannot set instance data to a mesh that is not both dynamic and instanced.");

            model.Mesh.UpdateVertexStream<InstanceVertexData>(data.Select(m => new InstanceVertexData() { Matrix = Matrix.Transpose(m) }).ToArray());
        }

        public static void UpdateInstanceData(this RiggedModel model, IEnumerable<Matrix> orientationData, IEnumerable<SkinOffset> skinningData)
        {
            throw new NotImplementedException();
        }
    }
}
