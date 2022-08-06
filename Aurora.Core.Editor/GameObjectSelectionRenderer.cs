using SharpDX;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.World;
using System.Linq;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Effects;

namespace CipherPark.Aurora.Core.Services
{
    public class GameObjectSelectionRenderer : IRenderer
    {
        private IGameApp gameApp;
        private GameObject targetGameObject;
        private ModelRenderer boundsGlyphRenderer;        
        private bool isInitialized = false;

        public GameObjectSelectionRenderer(IGameApp gameApp, GameObject targetGameObject)
        {
            this.gameApp = gameApp;
            this.targetGameObject = targetGameObject;
        }

        public IGameApp GameApp { get => gameApp; }

        public void Dispose()
        {           
            boundsGlyphRenderer?.Dispose();
        }

        public void Draw(ITransformable container)
        {
            OnetimeInitialize();         
            boundsGlyphRenderer.Draw(container);
        }

        public void Update(GameTime gameTime)
        {
            OnetimeInitialize();
        }

        private void OnetimeInitialize()
        {
            if (!isInitialized)
            {                
                boundsGlyphRenderer = new ModelRenderer(GameApp)
                {
                    Model = BuildBoundsGlyphModel()
                };

                isInitialized = true;
            }
        }

        private Model BuildBoundsGlyphModel()
        {
            const float EdgeLength = 5.0f;
            const float Padding = 0.5f;
            BoundingBox renderBox = targetGameObject.GetBoundingBox()
                                                    .GetValueOrDefault()
                                                    .Inflate(Padding);
            Vector3[] corners = renderBox.GetCorners();
            Vector3[] points = new Vector3[32];
            short[] indices = new short[48];

            for (int i = 0; i < corners.Length; i++)
            {
                int a = i;
                int b = (i <= 3) ? ((i == 0) ? 3 : i - 1) : ((i == 4) ? 7 : i - 1);
                int c = (i <= 3) ? ((i == 3) ? 0 : i + 1) : ((i == 7) ? 4 : i + 1);
                int d = (i <= 3) ? i + 4 : i - 4;
                int v = i * 4;
                points[v] = corners[a];
                points[v + 1] = corners[a] + Vector3.Normalize(corners[b] - corners[a]) * EdgeLength;
                points[v + 2] = corners[a] + Vector3.Normalize(corners[c] - corners[a]) * EdgeLength;
                points[v + 3] = corners[a] + Vector3.Normalize(corners[d] - corners[a]) * EdgeLength;

                int n = i * 6;
                indices[n] = (short)v;
                indices[n + 1] = (short)(v + 1);
                indices[n + 2] = (short)v;
                indices[n + 3] = (short)(v + 2);
                indices[n + 4] = (short)v;
                indices[n + 5] = (short)(v + 3);
            }

            VertexPositionColor[] vertices = points.Select(p => new VertexPositionColor()
            {
                Position = new Vector4(p, 1.0f),
                Color = Color.Gray.ToVector4(),
            }).ToArray();          

            var effect = new FlatEffect(GameApp, SurfaceVertexType.PositionColor);

            var mesh = ContentBuilder.BuildMesh(GameApp.GraphicsDevice,
                                            effect.GetVertexShaderByteCode(),
                                            vertices,
                                            indices,
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

    public static class BoundingBoxExtensions
    {
        public static BoundingBox Inflate(this BoundingBox box, float x, float y, float z)
        {
            return new BoundingBox(new Vector3(box.Minimum.X - x, box.Minimum.Y - y, box.Minimum.Z - z),
                                   new Vector3(box.Maximum.X + x, box.Maximum.Y + y, box.Maximum.Z + z));
        }

        public static BoundingBox Inflate(this BoundingBox box, float uniformValue)
        {
            return Inflate(box, uniformValue, uniformValue, uniformValue);
        }
    }
}
