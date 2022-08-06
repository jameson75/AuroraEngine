using SharpDX;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.World;
using System.Linq;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.World.Scene;
using System;
using SharpDX.Direct3D11;

namespace CipherPark.Aurora.Core.Services
{
    public class GameObjectShadowRenderer : IRenderer
    {
        private IGameApp gameApp;
        private GameObjectSceneNode targetGameNode;
        private Model model;
        private bool isInitialized = false;
        private RasterizerState cachedRasterizerState;

        public GameObjectShadowRenderer(IGameApp gameApp, GameObjectSceneNode targetGameNode)
        {
            this.gameApp = gameApp;
            this.targetGameNode = targetGameNode;
        }

        public IGameApp GameApp { get => gameApp; }

        public void Dispose()
        {           
            model?.Dispose();
        }

        public void Draw(ITransformable container)
        {
            OnetimeInitialize();
            var cameraNode = gameApp.GetActiveScene()
                                    .CameraNode;
            
            var referenceGrid = gameApp.GetActiveScene()
                                       .Select(n => n.As<GameObjectSceneNode>()?.GameObject.IsReferenceGridObject() ?? false)
                                       .FirstOrDefault()
                                       ?.As<GameObjectSceneNode>();

            if (referenceGrid != null)
            {
                var targetNodeWorldPosition = targetGameNode.WorldPosition();
                var shadowWorldPosition = new Vector3(targetNodeWorldPosition.X, referenceGrid.WorldPosition().Y, targetNodeWorldPosition.Z);
                model.Effect.World = Matrix.Translation(shadowWorldPosition);
                model.Effect.View = cameraNode.RiggedViewMatrix;
                model.Effect.Projection = cameraNode.ProjectionMatrix;
                DisableBackfaceCulling();
                model.Draw();
                RestoreBackfaceCulling();
            }            
        }        

        public void Update(GameTime gameTime)
        {
            OnetimeInitialize();          
        }

        private void OnetimeInitialize()
        {
            if (!isInitialized)
            {
                model = BuildShadowModel();
                isInitialized = true;
            }
        }

        private Model BuildShadowModel()
        {            
            const float Padding = 0.5f;
            BoundingBox renderBox = targetGameNode.GameObject
                                                  .GetBoundingBox()
                                                  .GetValueOrDefault()
                                                  .Inflate(Padding);

            var points = new Vector3[4]
            {
                new Vector3(renderBox.Minimum.X, 0, renderBox.Minimum.Z),
                new Vector3(renderBox.Minimum.X, 0, renderBox.Maximum.Z),
                new Vector3(renderBox.Maximum.X, 0, renderBox.Maximum.Z),
                new Vector3(renderBox.Maximum.X, 0, renderBox.Minimum.Z),
            };

            var indices = new short[6] { 0, 1, 2, 2, 3, 0 };           

            VertexPositionColor[] vertices = points.Select(p => new VertexPositionColor()
            {
                Position = new Vector4(p, 1.0f),
                Color = Color.Orange.ToVector4(),
            }).ToArray();          

            var effect = new FlatEffect(GameApp, SurfaceVertexType.PositionColor);

            var mesh = ContentBuilder.BuildMesh(GameApp.GraphicsDevice,
                                            effect.GetVertexShaderByteCode(),
                                            vertices,
                                            indices,
                                            VertexPositionColor.InputElements,
                                            VertexPositionColor.ElementSize,
                                            BoundingBox.FromPoints(points),
                                            SharpDX.Direct3D.PrimitiveTopology.TriangleList);

            return new StaticMeshModel(GameApp)
            {
                Effect = effect,
                Mesh = mesh,
            };            
        }


        private void RestoreBackfaceCulling()
        {
            gameApp.GraphicsDeviceContext.Rasterizer.State = cachedRasterizerState;
        }

        private void DisableBackfaceCulling()
        {
            cachedRasterizerState = gameApp.GraphicsDeviceContext.Rasterizer.State;
            RasterizerStateDescription newRasterizerStateDesc = cachedRasterizerState?.Description ?? RasterizerStateDescription.Default();
            newRasterizerStateDesc.CullMode = CullMode.None;
            gameApp.GraphicsDeviceContext.Rasterizer.State = new RasterizerState(gameApp.GraphicsDevice, newRasterizerStateDesc);                        
        }
    }
}
