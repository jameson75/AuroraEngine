using System;
using System.Linq;
using CipherPark.Aurora.Core;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Extensions;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Geometry;
using SharpDX;
using SharpDX.Direct3D;

namespace Aurora.Sample.Editor.Scene
{
    public class ReferenceGridRenderer : IRenderer, IProvideBoundingContext
    {
        private readonly IGameApp game;
        private readonly ModelRenderer modelRenderer;
        private readonly ReferenceGrid referenceGrid;
        private bool isInitialized;

        public ReferenceGridRenderer(IGameApp game, ReferenceGrid referenceGrid)
        {
            this.game = game;
            this.referenceGrid = referenceGrid;
            modelRenderer = new ModelRenderer(game);
        }

        public void Dispose()
        {
            modelRenderer.Dispose();
        }

        public void Draw(ITransformable container)
        {
            modelRenderer.Draw(container);
        }       

        public void Update(GameTime gameTime)
        {
            if (!isInitialized)
            {
                float totalWidth = referenceGrid.Width;
                float totalHeight = referenceGrid.Height;
                int widthSegments = referenceGrid.WidthSegments;
                int heightSegments = referenceGrid.HeightSegments;
                int widthSectors = referenceGrid.WidthSectors;
                int heightSectors = referenceGrid.HeightSectors;               
                var totalWidthSegments = widthSegments * widthSectors;
                var totalHeightSegments = heightSegments * heightSectors;

                var gridColor = Color.Gray.ToVector4();

                var vertices = new VertexPositionColor[((totalWidthSegments + 1) * 2) + ((totalHeightSegments + 1) * 2)];
                var segmentWidth = totalWidth / totalWidthSegments;
                var segmentHeight = totalHeight / totalHeightSegments;
                var halfWidth = totalWidth / 2;
                var halfHeight = totalHeight / 2;

                var currentHeight = 0f;                
                for(int i = 0; i <= totalWidthSegments; i++)
                {
                    var k = i * 2;
                    var segmentColor = (i % widthSegments != 0) ? gridColor : Color.WhiteSmoke.ToVector4();
                    switch (referenceGrid.Orientation)
                    {
                        case ReferenceGridOrientation.Z:
                            vertices[k] = new VertexPositionColor(new Vector3(-halfWidth, currentHeight, 0), segmentColor);
                            vertices[k + 1] = new VertexPositionColor(new Vector3(halfWidth, currentHeight, 0), segmentColor);
                            break;                        
                        case ReferenceGridOrientation.X:
                            vertices[k] = new VertexPositionColor(new Vector3(0, currentHeight, -halfWidth), segmentColor);
                            vertices[k + 1] = new VertexPositionColor(new Vector3(0, currentHeight, halfWidth), segmentColor);
                            break;                       
                        case ReferenceGridOrientation.Y:
                            vertices[k] = new VertexPositionColor(new Vector3(-halfWidth, 0, currentHeight - halfHeight), segmentColor);
                            vertices[k + 1] = new VertexPositionColor(new Vector3(halfWidth, 0, currentHeight - halfHeight), segmentColor);
                            break;
                        default:
                            throw new InvalidOperationException("Unsupported orientation");
                    }
                    currentHeight += segmentHeight;
                }

                var currentWidth = -halfWidth;
                var kOffset = (totalWidthSegments + 1) * 2;
                for (int i = 0; i <= totalHeightSegments; i++)
                {
                    var k = kOffset + (i * 2);
                    var segmentColor = (i % heightSegments != 0) ? gridColor : Color.WhiteSmoke.ToVector4();
                    switch (referenceGrid.Orientation)
                    {
                        case ReferenceGridOrientation.Z:
                            vertices[k] = new VertexPositionColor(new Vector3(currentWidth, 0, 0), segmentColor);
                            vertices[k + 1] = new VertexPositionColor(new Vector3(currentWidth, totalHeight, 0), segmentColor);
                            break;
                        case ReferenceGridOrientation.X:
                            vertices[k] = new VertexPositionColor(new Vector3(0, 0, currentWidth), segmentColor);
                            vertices[k + 1] = new VertexPositionColor(new Vector3(0, totalHeight, currentWidth), segmentColor);
                            break;
                        case ReferenceGridOrientation.Y:
                            vertices[k] = new VertexPositionColor(new Vector3(currentWidth, 0, -halfHeight), segmentColor);
                            vertices[k + 1] = new VertexPositionColor(new Vector3(currentWidth, 0, halfHeight), segmentColor);
                            break;
                        default:
                            throw new InvalidOperationException("Unsupported orientation");
                    }                    
                    currentWidth += segmentWidth;
                }

                var effect = new FlatEffect(game, SurfaceVertexType.PositionColor);

                var mesh = ContentBuilder.BuildMesh(game.GraphicsDevice,
                                         effect.GetVertexShaderByteCode(),
                                         vertices,
                                         VertexPositionColor.InputElements,
                                         VertexPositionColor.ElementSize,
                                         BoundingBox.FromPoints(vertices.Select(v => v.Position.XYZ())
                                                                        .Concat(new[] { new Vector3(0, 0.001f, 0), new Vector3(0, -0.001f, 0) })
                                                                        .ToArray()),
                                         PrimitiveTopology.LineList);

                var model = new StaticMeshModel(game)
                {
                    Mesh = mesh,
                    Effect = effect,
                    Name = "Reference Grid",  
                };

                modelRenderer.Model = model;

                isInitialized = true;
            }

            modelRenderer.Update(gameTime);
        }

        public BoundingBox? GetBoundingBox()
        {
            return modelRenderer.Model.BoundingBox;
        }
    }

    public enum ReferenceGridOrientation
    {
        Y,
        X,
        Z
    }

}
