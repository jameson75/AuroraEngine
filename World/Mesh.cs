using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using DXBuffer = SharpDX.Direct3D11.Buffer;

namespace CipherPark.AngelJacket.Core.World
{
    public class Mesh<VertexType> where VertexType : struct
    {
        private DXBuffer _vertexBuffer = null;
        private DXBuffer _indexBuffer = null;
        private IGameApp _app = null;
        private MeshDescription _meshDescription;

        public Mesh(IGameApp app, string fileName)
        {
            _app = app;
        }

        public Mesh(IGameApp app, MeshDescription meshDescription, VertexType[] meshData, short[] indexData = null)
        {
            _app = app;
            _meshDescription = meshDescription;
            _meshDescription.VertexCount = meshData.Length;
            _vertexBuffer = DXBuffer.Create<VertexType>(app.GraphicsDevice, meshData, meshDescription.VertexBufferDescription);
            if (indexData != null)
            {
                _meshDescription.IndexCount = indexData.Length;
                _indexBuffer = DXBuffer.Create<short>(app.GraphicsDevice, indexData, meshDescription.IndexBufferDescription);
            }
        }

        public void WriteVertices(VertexType[] vertices, long bufferOffset = 0, int vertexOffset = 0 )
        {          
            DataStream stream = null;
            _app.GraphicsDeviceContext.MapSubresource(_vertexBuffer, MapMode.Write, SharpDX.Direct3D11.MapFlags.None, out stream);
            stream.Position = bufferOffset;
            for (int i = vertexOffset; i < vertices.Length; i++ )
                stream.Write(vertices[i]);                
            _app.GraphicsDeviceContext.UnmapSubresource(_vertexBuffer, 0);
            stream.Dispose();
        }

        public void WriteIndices(short[] indices, long bufferOffset = 0, short indexOffset = 0)
        {
            DataStream stream = null;
            _app.GraphicsDeviceContext.MapSubresource(_vertexBuffer, MapMode.Write, SharpDX.Direct3D11.MapFlags.None, out stream);
            stream.Position = bufferOffset;
            for (short i = indexOffset; i < indices.Length; i++ )
                stream.Write(indices[i]);                
            _app.GraphicsDeviceContext.UnmapSubresource(_vertexBuffer, 0);
            stream.Dispose();
        }

        public void AddTriangle(long bufferOffset, VertexType v1, VertexType v2, VertexType v3)
        {
            WriteVertices(new VertexType[] { v1, v2, v3 }, bufferOffset );
        }

        public void AddIndexedTriangle(long bufferOffset, short i1, short i2, short i3)
        {
            WriteIndices(new short[] {i1, i2, i3}, bufferOffset);
        }

        public void AddLine(long bufferOffset, VertexType v1, VertexType v2)
        {
            WriteVertices( new VertexType[] {v1, v2} );
        }

        public void AddIndexLine(long bufferOffset, short i1, short i2)
        {
            WriteIndices( new short[] { i1, i2 }, bufferOffset );
        }

        public void Draw(long gameTime)
        {
            _app.GraphicsDeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _app.GraphicsDeviceContext.InputAssembler.SetVertexBuffers(0, new[] { _vertexBuffer }, new[] { _meshDescription.VertexStride, }, new[] { 0 });
            if (_indexBuffer != null)
            {
                _app.GraphicsDeviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
                _app.GraphicsDeviceContext.DrawIndexed(_meshDescription.IndexCount, 0, 0);
            }
            else
                _app.GraphicsDeviceContext.Draw(_meshDescription.VertexCount, 0);            
        }
    }

    public struct MeshDescription
    {
        public BufferDescription IndexBufferDescription { get; set; }
        public BufferDescription VertexBufferDescription { get; set; }
        public int VertexStride { get; set; }
        public int VertexCount { get; set; }
        public int IndexCount { get; set; }
    }

    public class ReferenceGrid
    {
        private Mesh<VertexPositionColor> _mesh = null;
        public ReferenceGrid(IGameApp app, DrawingSize quadSize, DrawingSizeF unitSize, Color4 color)
        {
            int gridColor = color.ToRgba();
            int xSteps = (quadSize.Width * 2 + 1);
            int zSteps = (quadSize.Height * 2 + 1);
            int nVertices = 2 * ( xSteps + zSteps );
            VertexPositionColor[] vertices = new VertexPositionColor[nVertices];
            Vector3 v1 = new Vector3(-quadSize.Width, 0.0f, -quadSize.Height);
            Vector3 v2 = new Vector3(-quadSize.Width, 0.0f, quadSize.Height);
            for(int i = 0; i < xSteps; i+=2)
            {
                vertices[i] = new VertexPositionColor(v1, gridColor);
                vertices[i+1] = new VertexPositionColor(v2, gridColor);
                v1+= new Vector3(unitSize.Width, 0f, 0f);
                v2 += new Vector3(unitSize.Width, 0f, 0f);
            }

            v1 = new Vector3(-quadSize.Width, 0.0f, -quadSize.Height);
            v2 = new Vector3(quadSize.Width, 0.0f, -quadSize.Height);
            for (int i = 0; i < zSteps; i += 2)
            {
                vertices[i] = new VertexPositionColor(v1, gridColor);
                vertices[i + 1] = new VertexPositionColor(v2, gridColor);
                v1 += new Vector3(0f, 0f, unitSize.Height);
                v2 += new Vector3(0f, 0f, unitSize.Height);
            }
            _mesh = new Mesh<VertexPositionColor>(app, new MeshDescription(), vertices);
        }

        public void Draw(long gameTime)
        {
            _mesh.Draw(gameTime);
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct VertexPositionColor
    {
        public Vector3 Position;
        public int Color;
        public static InputElement[] _inputElements = null;
        public InputElement[] InputElements { get { return _inputElements; } }
        static VertexPositionColor()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                 new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, 12, 0)
             };
        }
        public VertexPositionColor(Vector3 position)
        {
            Position = position;
            Color = SharpDX.Color.Transparent.ToRgba();
        }
        public VertexPositionColor(Vector3 position, int color)
        {
            Position = position;
            Color = color;
        }
    }
}
