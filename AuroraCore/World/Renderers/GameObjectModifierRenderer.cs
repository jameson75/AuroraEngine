using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.World;
using System.Linq;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Effects;

namespace CipherPark.Aurora.Core.Services
{
    public class GameObjectModifierRenderer : IRenderer
    {
        private IGameApp gameApp;
        private EditorObjectContext modifierContext;
        /*
        private ModelRenderer translateGlyphRenderer;
        private ModelRenderer rotateGlyphRenderer;
        private ModelRenderer scaleGlyphRenderer;
        */
        private ModelRenderer boundsGlyphRenderer;        
        private bool isInitialized = false;

        public GameObjectModifierRenderer(IGameApp gameApp, EditorObjectContext modifierContext)
        {
            this.gameApp = gameApp;
            this.modifierContext = modifierContext;
            this.modifierContext.ModifierMode = GameObjectModifierMode.Translate;
        }

        public IGameApp GameApp { get => gameApp; }

        public void Dispose()
        {
            /*
            translateGlyphRenderer?.Dispose();
            rotateGlyphRenderer?.Dispose();
            scaleGlyphRenderer?.Dispose();
            */
            boundsGlyphRenderer?.Dispose();
        }

        public void Draw(ITransformable container)
        {
            OnetimeInitialize();

            var oldState = DisableDepthBuffer();
            /*
            switch (modifierContext.ModifierMode)
            {
                case GameObjectModifierMode.Translate:
                    translateGlyphRenderer.Draw(container);
                    break;
                case GameObjectModifierMode.Rotate:
                    rotateGlyphRenderer.Draw(container);
                    break;
                case GameObjectModifierMode.Scale:
                    scaleGlyphRenderer.Draw(container);
                    break;                
            }
            */
            RestoreDepthBuffer(oldState);
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
                /*
                translateGlyphRenderer = new ModelRenderer(GameApp)
                {
                    Model = BuildTranslateGlyphModel(),
                };

                rotateGlyphRenderer = new ModelRenderer(GameApp)
                {
                    Model = BuildRotateGlyphModel()
                };

                scaleGlyphRenderer = new ModelRenderer(GameApp)
                {
                    Model = BuildScaleGlyphModel()
                };
                */
                boundsGlyphRenderer = new ModelRenderer(GameApp)
                {
                    Model = BuildBoundsGlyphModel()
                };

                isInitialized = true;
            }
        }

        private Model BuildTranslateGlyphModel()
        { 
            const float ArrowWidth = 5;
            const float ArrowBaseLength = 20;
            const float ArrowTipLength = 5;
            const float ArrowBaseExtent = ArrowWidth + ArrowBaseLength;
            const float ArrowTipExtent = ArrowBaseExtent + ArrowTipLength;
            const float HalfArrowWidth = ArrowWidth / 2f;

            Vector3[] axisBoxPoints = new Vector3[]
            {
                new Vector3(0, 0, 0),                           //0
                new Vector3(0, ArrowWidth, 0),                  //1
                new Vector3(ArrowWidth, ArrowWidth, 0),         //2
                new Vector3(ArrowWidth, 0, 0),                  //3
                new Vector3(ArrowWidth, 0, ArrowWidth),         //4
                new Vector3(ArrowWidth, ArrowWidth, ArrowWidth),//5
                new Vector3(0, ArrowWidth, ArrowWidth),         //6
                new Vector3(0, 0, ArrowWidth),                  //7
            };
            //Note: These are the visible faces only.
            short[] axisBoxIndicies = new short[]
            {
                0, 1, 2,
                2, 3, 0,
                2, 6, 1,
                1, 0, 2,
                4, 7, 0,
                0, 3, 4
            };
            Vector3[] xAxisPoints = new Vector3[]
            {
                new Vector3(ArrowWidth, 0, 0),                  //0
                new Vector3(ArrowWidth, ArrowWidth, 0),         //1
                new Vector3(ArrowWidth, ArrowWidth, ArrowWidth),//2
                new Vector3(ArrowWidth, 0, ArrowBaseLength),    //3
                new Vector3(ArrowBaseExtent, 0, 0),                  //4
                new Vector3(ArrowBaseExtent, ArrowWidth, 0),         //5
                new Vector3(ArrowBaseExtent, ArrowWidth, ArrowWidth),//6
                new Vector3(ArrowBaseExtent, 0, ArrowBaseLength),        //7
                new Vector3(ArrowTipExtent, HalfArrowWidth, HalfArrowWidth), //8
            };
            Vector3[] yAxisPoints = new Vector3[]
            {                 
                new Vector3(0, ArrowWidth, 0),                  //0          
                new Vector3(0, ArrowWidth, ArrowWidth),         //1
                new Vector3(ArrowWidth, ArrowWidth, ArrowWidth),//2
                new Vector3(ArrowWidth, ArrowWidth, 0),         //3
                new Vector3(0, ArrowBaseExtent, 0),                  //4
                new Vector3(0, ArrowBaseExtent, ArrowWidth),         //5
                new Vector3(ArrowWidth, ArrowBaseExtent, ArrowWidth),//6
                new Vector3(ArrowWidth, ArrowBaseExtent, 0),         //7
                new Vector3(HalfArrowWidth, ArrowTipExtent, HalfArrowWidth), //8
            };
            Vector3[] zAxisPoints = new Vector3[]
            {
                new Vector3(ArrowWidth, 0, ArrowWidth),         //0
                new Vector3(ArrowWidth, ArrowWidth, ArrowWidth),//1
                new Vector3(0, ArrowWidth, ArrowWidth),         //2
                new Vector3(0, 0, ArrowWidth),                  //
                new Vector3(ArrowWidth, 0, ArrowBaseExtent),         //4
                new Vector3(ArrowWidth, ArrowWidth, ArrowBaseExtent),//5
                new Vector3(0, ArrowWidth, ArrowBaseExtent),         //6
                new Vector3(0, 0, ArrowBaseExtent),                  //7
                new Vector3(HalfArrowWidth, HalfArrowWidth, ArrowTipExtent), //8
            };
            short[] axisIndices = new short[]
            {
                0, 1, 2,
                2, 3, 0,
                1, 6, 5,
                5, 2, 1,
                4, 5, 6,
                6, 7, 4,
                7, 0, 3,
                3, 4, 7,
                8, 3, 2,
                8, 2, 5,
                8, 5, 4,
                8, 4, 3
            };

            var effect = new FlatEffect(GameApp, SurfaceVertexType.PositionColor);

            var boxMesh = ContentBuilder.BuildMesh(
                GameApp.GraphicsDevice,
                effect.GetVertexShaderByteCode(),
                axisBoxPoints.Select(p => new VertexPositionColor(p, Color.Wheat)).ToArray(),
                axisBoxIndicies,
                VertexPositionColor.InputElements,
                VertexPositionColor.ElementSize,
                BoundingBox.FromPoints(axisBoxPoints),
                SharpDX.Direct3D.PrimitiveTopology.TriangleList);

            var xAxisMesh = ContentBuilder.BuildMesh(
                GameApp.GraphicsDevice,
                effect.GetVertexShaderByteCode(),
                xAxisPoints.Select(p => new VertexPositionColor(p, Color.Blue)).ToArray(),
                axisIndices,
                VertexPositionColor.InputElements,
                VertexPositionColor.ElementSize,
                BoundingBox.FromPoints(xAxisPoints),
                SharpDX.Direct3D.PrimitiveTopology.TriangleList);

            var yAxisMesh = ContentBuilder.BuildMesh(
                GameApp.GraphicsDevice,
                effect.GetVertexShaderByteCode(),
                yAxisPoints.Select(p => new VertexPositionColor(p, Color.Green)).ToArray(),
                axisIndices,
                VertexPositionColor.InputElements,
                VertexPositionColor.ElementSize,
                BoundingBox.FromPoints(yAxisPoints),
                SharpDX.Direct3D.PrimitiveTopology.TriangleList);

            var zAxisMesh = ContentBuilder.BuildMesh(
                GameApp.GraphicsDevice,
                effect.GetVertexShaderByteCode(),
                zAxisPoints.Select(p => new VertexPositionColor(p, Color.Red)).ToArray(),
                axisIndices,
                VertexPositionColor.InputElements,
                VertexPositionColor.ElementSize,
                BoundingBox.FromPoints(zAxisPoints),
                SharpDX.Direct3D.PrimitiveTopology.TriangleList);

            var model = new MultiMeshModel(GameApp);
            model.Meshes.Add(boxMesh);
            model.Meshes.Add(xAxisMesh);
            model.Meshes.Add(yAxisMesh);
            model.Meshes.Add(zAxisMesh);
            model.Effect = effect;

            return model;
        }

        private Model BuildRotateGlyphModel()
        {
            //TODO: Replace this line with implementation.
            return BuildTranslateGlyphModel();
        }

        private Model BuildScaleGlyphModel()
        {
            //TODO: Replace this line with implementation.
            return BuildTranslateGlyphModel();
        }

        private Model BuildBoundsGlyphModel()
        {
            const float EdgeLength = 5.0f;
            const float Padding = 0.5f;
            BoundingBox renderBox = modifierContext.ModifierTargetNode.GameObject.GetBoundingBox()
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

        private DepthStencilState DisableDepthBuffer()
        {
            var oldState = GameApp.GraphicsDeviceContext.OutputMerger.DepthStencilState;
            var newDescription = (oldState != null) ? oldState.Description : DepthStencilStateDescription.Default();
            newDescription.IsDepthEnabled = true;
            newDescription.DepthComparison = Comparison.Always;
            
            GameApp.GraphicsDeviceContext.OutputMerger.DepthStencilState = new DepthStencilState(
                GameApp.GraphicsDevice,
                newDescription);
            return oldState;
        }

        private void RestoreDepthBuffer(DepthStencilState state)
        {
            GameApp.GraphicsDeviceContext.OutputMerger.DepthStencilState = state;
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
