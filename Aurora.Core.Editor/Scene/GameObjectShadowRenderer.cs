using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Extensions;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.World.Scene;
using SharpDX;
using SharpDX.Direct3D11;
using Aurora.Core.Editor.Util;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Effects;
using System.Linq;
using Aurora.Sample.Editor.Scene;

namespace CipherPark.Aurora.Core.Services
{
    //TODO: Replace this class with a shadow effect class and shader.

    public class GameObjectShadowRenderer : IRenderer
    {
        private IGameApp gameApp;
        private GameObjectSceneNode targetGameNode;
        private Vector3[] meshPoints_ws;
        private short[] meshIndices;
        private Mesh dynamicMesh;
        private SurfaceEffect meshEffect;
        private RasterizerState cachedRasterizerState;
        private bool isInitialized = false;

        public GameObjectShadowRenderer(IGameApp gameApp, GameObjectSceneNode targetGameNode)
        {
            this.gameApp = gameApp;
            this.targetGameNode = targetGameNode;
        }

        public IGameApp GameApp { get => gameApp; }

        public void Dispose()
        {
            dynamicMesh?.Dispose();
        }

        public void Draw(ITransformable container)
        {
            //TODO: Move this method to Update() once we fix the guarantee of Update() being called before Draw().
            OnTimeInitialize();

            CalcMeshData();

            var referenceObjectRoot = gameApp.GetActiveScene()
                                             .SelectReferenceObjectRoot();

            if (referenceObjectRoot != null)
            {
                foreach (var referenceObjectNode in referenceObjectRoot.GameObjectChildren())
                {
                    if (referenceObjectNode.GetGameObject().IsReferenceGridObject() == true)
                    {
                        var referenceGrid = referenceObjectNode.GetGameObject().GetReferenceGrid();
                        ProjectShadowToReferenceGrid(referenceObjectNode, referenceGrid);
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {

        }

        public void OnTimeInitialize()
        {
            if (!isInitialized)
            {
                CreateMesh();
                isInitialized = true;
            }
        }

        private void CalcMeshData()
        {
            const float Padding = 0.5f;
            BoundingBoxOA renderBox_ws = targetGameNode.GetWorldBoundingBox();

            renderBox_ws.Inflate(Padding, Padding, Padding);

            meshPoints_ws = renderBox_ws.GetCorners();
        }

        private void CreateMesh()
        {
            const int MeshPointsMaxLength = 8;

            meshIndices = new short[]
            {
               7, 4, 5, //Front 1
               5, 6, 7, //Front 2
               3, 2, 1, //Back 1
               1, 0, 3, //Back 2
               4, 0, 1, //Top 1
               1, 5, 4, //Top 2
               7, 6, 2, //Bottom 1
               2, 3, 7, //Bottom 2
               6, 5, 1, //Right 1
               1, 2, 6, //Right 2
               7, 3, 0, //Left 1
               0, 4, 7, //Left 2
            };

            meshEffect = new FlatEffect(GameApp, SurfaceVertexType.PositionColor);

            dynamicMesh = ContentBuilder.BuildDynamicMesh(GameApp.GraphicsDevice,
                                            meshEffect.GetVertexShaderByteCode(),
                                            (VertexPositionColor[])null,
                                            MeshPointsMaxLength,
                                            meshIndices,
                                            meshIndices.Length,
                                            VertexPositionColor.InputElements,
                                            VertexPositionColor.ElementSize,
                                            new BoundingBox(),
                                            SharpDX.Direct3D.PrimitiveTopology.TriangleList);
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

        private void ProjectShadowToReferenceGrid(GameObjectSceneNode referenceObjectNode, ReferenceGrid referenceGrid)
        {
            Plane plane_ws = new Plane(-referenceObjectNode.WorldPosition(),
                                       referenceObjectNode.LocalToWorldNormal(referenceGrid.Normal));
            var projectedMeshPoints_ws = ProjectPoints(plane_ws, meshPoints_ws);
            var vertices = projectedMeshPoints_ws.Select(p => new VertexPositionColor
            {
                Position = new Vector4(p, 1),
                Color = Color.Orange.ToVector4(),
            }).ToArray();
            dynamicMesh.UpdateVertexStream(vertices);
            RenderWorldSpaceDynamicMesh();
        }

        private void RenderWorldSpaceDynamicMesh()
        {
            var cameraNode = gameApp.GetRenderingCamera();
            meshEffect.World = Matrix.Identity;
            meshEffect.View = cameraNode.RiggedViewMatrix;
            meshEffect.Projection = cameraNode.ProjectionMatrix;            
            DisableBackfaceCulling();
            meshEffect.Apply();
            dynamicMesh.Draw();
            meshEffect.Restore();
            RestoreBackfaceCulling();
        }

        private static Vector3[] ProjectPoints(Plane plane, Vector3[] points)
            => points.Select(x => plane.ProjectPoint(x).Value).ToArray();
    }
}
