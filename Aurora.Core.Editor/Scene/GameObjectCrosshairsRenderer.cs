using SharpDX;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.World;
using System.Linq;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Effects;
using System;

namespace CipherPark.Aurora.Core.Services
{
    [Obsolete]
    public class GameObjectCrossHairsRenderer : IRenderer
    {
        private IGameApp gameApp;
        private GameObject targetGameObject;
        private Model model;
        private bool isInitialized = false;

        public GameObjectCrossHairsRenderer(IGameApp gameApp, GameObject targetGameObject)
        {
            this.gameApp = gameApp;
            this.targetGameObject = targetGameObject;
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
            model.Effect.World = container?.WorldTransform().ToMatrix() ?? Matrix.Identity;
            model.Effect.View = cameraNode.RiggedViewMatrix;
            model.Effect.Projection = cameraNode.ProjectionMatrix;
            model.Draw();
        }

        public void Update(GameTime gameTime)
        {
            OnetimeInitialize();
        }

        private void OnetimeInitialize()
        {
            if (!isInitialized)
            {
                model = BuildCrossHairsModel();
                isInitialized = true;
            }
        }

        private Model BuildCrossHairsModel()
        {
            const float HairLength = 1000.0f;
            const float Padding = 0.5f;
            BoundingBox renderBox = targetGameObject.GetBoundingBox()
                                                    .GetValueOrDefault()
                                                    .Inflate(Padding);
            
            Vector3[] points = new Vector3[12];
            points[0] = new Vector3(-renderBox.GetLengthX(), 0, 0);
            points[1] = new Vector3(-renderBox.GetLengthX() - HairLength, 0, 0);
            points[2] = new Vector3(renderBox.GetLengthX(), 0, 0);
            points[3] = new Vector3(renderBox.GetLengthX() + HairLength, 0, 0);
            points[4] = new Vector3(0, -renderBox.GetLengthY(), 0);
            points[5] = new Vector3(0, -renderBox.GetLengthY() - HairLength, 0);
            points[6] = new Vector3(0, renderBox.GetLengthY(), 0);
            points[7] = new Vector3(0, renderBox.GetLengthY() + HairLength, 0);
            points[8] = new Vector3(0, 0, -renderBox.GetLengthZ());
            points[9] = new Vector3(0, 0, -renderBox.GetLengthZ() - HairLength);
            points[10] = new Vector3(0, 0, renderBox.GetLengthZ());
            points[11] = new Vector3(0, 0, renderBox.GetLengthZ() + HairLength);

            VertexPositionColor[] vertices = points.Select((p, i) => new VertexPositionColor()
            {
                Position = new Vector4(p, 1.0f),
                Color = (i < 4 ? Color.Red : i < 8 ? Color.Yellow : Color.Blue).ToVector4(),
            }).ToArray();          

            var effect = new FlatEffect(GameApp, SurfaceVertexType.PositionColor);

            var mesh = ContentBuilder.BuildMesh(GameApp.GraphicsDevice,
                                            effect.GetVertexShaderByteCode(),
                                            vertices,
                                            null,
                                            VertexPositionColor.InputElements,
                                            VertexPositionColor.ElementSize,
                                            BoundingBox.FromPoints(points),
                                            SharpDX.Direct3D.PrimitiveTopology.LineList);

            return new StaticMeshModel(GameApp)
            {
                Effect = effect,
                Mesh = mesh,
            };            
        }        
    }
}
