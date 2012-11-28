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
using CipherPark.AngelJacket.Core.Utils.Interop;

namespace CipherPark.AngelJacket.Core.World
{
    public class Mesh<VertexType> where VertexType : struct
    {
        private DXBuffer _vertexBuffer = null;
        private DXBuffer _indexBuffer = null;
        private IGameApp _app = null;        
        private int _vertexCount = 0;
        private int _indexCount = 0;
        private int _vertexStride = 0;
        private InputLayout _vertexLayout = null;
        private PrimitiveTopology _topology = PrimitiveTopology.TriangleList;

        public Mesh(IGameApp app, string fileName)
        {
            _app = app;
        }

        public Mesh(IGameApp app, MeshDescription meshDescription, VertexType[] meshData, short[] indexData = null)
        {
            _app = app;
            _vertexStride = meshDescription.VertexStride;
            _vertexLayout = meshDescription.VertexLayout;
            _topology = meshDescription.Topology;
            _vertexCount = meshData.Length;           
            _vertexBuffer = DXBuffer.Create<VertexType>(app.GraphicsDevice, meshData, meshDescription.VertexBufferDescription);
            if (indexData != null)
            {
                _indexCount = indexData.Length;
                _indexBuffer = DXBuffer.Create<short>(app.GraphicsDevice, indexData, meshDescription.IndexBufferDescription);
            }
        }

        public void WriteVertices(VertexType[] vertices, long bufferOffset = 0, int vertexOffset = 0 )
        {
            if (_vertexBuffer == null)
                throw new InvalidOperationException("Vertex buffer was not created.");

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
            if (_indexBuffer == null)
                throw new InvalidOperationException("Index buffer was not created");

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
            if (_vertexBuffer == null)
                throw new InvalidOperationException("Vertex buffer was not created.");

            if (_vertexLayout == null)
                throw new InvalidOperationException("Input layout was not set.");

            _app.GraphicsDeviceContext.InputAssembler.InputLayout = _vertexLayout;
            _app.GraphicsDeviceContext.InputAssembler.PrimitiveTopology = _topology;
            _app.GraphicsDeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, _vertexStride, 0));
            if (_indexBuffer != null)
            {
                _app.GraphicsDeviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
                _app.GraphicsDeviceContext.DrawIndexed(_indexCount, 0, 0);
            }
            else
                _app.GraphicsDeviceContext.Draw(_vertexCount, 0);            
        }
    }

    public struct MeshDescription
    {
        public BufferDescription IndexBufferDescription { get; set; }
        public BufferDescription VertexBufferDescription { get; set; }
        public int VertexStride { get; set; }
        public InputLayout VertexLayout { get; set; }
        public PrimitiveTopology Topology { get; set; }
    }

    public class ReferenceGrid
    {
        private Mesh<VertexPositionColor> _mesh = null;
        private BasicEffect _effect = null;
        
        public ReferenceGrid(IGameApp app, DrawingSize quadSize, DrawingSizeF unitSize, Color4 color)
        {
            _effect = new BasicEffect(app.GraphicsDevice);
            _effect.SetWorld(Matrix.Identity);
            _effect.SetVertexColorEnabled(true);
            byte[] shaderCode = _effect.SelectShaderByteCode();

            Vector4 gridColor = color.ToVector4();
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

            //NOTE: We must get the shader byte code
            MeshDescription meshDesc = new MeshDescription();
            BufferDescription vertexBufferDesc = new BufferDescription();
            vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
            vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            vertexBufferDesc.SizeInBytes = vertices.Length * VertexPositionColor.ElementSize;
            vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            vertexBufferDesc.StructureByteStride = 0;
            meshDesc.VertexBufferDescription = vertexBufferDesc;
            meshDesc.VertexLayout = new InputLayout(app.GraphicsDevice, shaderCode, VertexPositionColor.InputElements);
            meshDesc.VertexStride = VertexPositionColor.ElementSize;
            meshDesc.Topology = PrimitiveTopology.LineList;
            _mesh = new Mesh<VertexPositionColor>(app, meshDesc, vertices);           
        }

        public void Draw(long gameTime)
        {
            _effect.SetWorld(Matrix.Identity);
            _effect.SetView(Camera.ViewMatrix);
            _effect.SetProjection(Camera.ProjectionMatrix);
            _effect.Apply();
            _mesh.Draw(gameTime);
        }

        public Camera Camera { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct VertexPositionColor
    {       
        public Vector4 Position;
        public Vector4 Color;     
        
        private static InputElement[] _inputElements = null;
        private  static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        static VertexPositionColor()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_Position", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
             };
            _elementSize = 32;
        }        
        public VertexPositionColor(Vector3 position)
        {
            Position = new Vector4(position, 0);
            Color = SharpDX.Color.Transparent.ToVector4();
        }
        public VertexPositionColor(Vector3 position, Vector4 color)
        {
            Position = new Vector4(position, 0);
            Color = color;
        }
    }
}
