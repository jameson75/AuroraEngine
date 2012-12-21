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
    public class Mesh
    {
        private DXBuffer _vertexBuffer = null;
        private DXBuffer _indexBuffer = null;
        private IGameApp _app = null;        
        private int _vertexCount = 0;
        private int _indexCount = 0;
        private int _vertexStride = 0;
        private InputLayout _vertexLayout = null;
        private PrimitiveTopology _topology = PrimitiveTopology.TriangleList;      

        public Mesh(IGameApp app, MeshDescription meshDescription)
        {
            _app = app;
            _vertexStride = meshDescription.VertexStride;
            _vertexLayout = meshDescription.VertexLayout;
            _topology = meshDescription.Topology;
            _vertexCount = meshDescription.VertexCount;
            _vertexBuffer = meshDescription.VertexBuffer;
            if (meshDescription.IndexBuffer != null)
            {
                _indexCount = meshDescription.IndexCount;
                _indexBuffer = meshDescription.IndexBuffer;
            }
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
        public DXBuffer IndexBuffer { get; set; }
        public DXBuffer VertexBuffer { get; set; }
        public int VertexStride { get; set; }
        public int VertexCount { get; set; }
        public int IndexCount { get; set; }
        public InputLayout VertexLayout { get; set; }
        public PrimitiveTopology Topology { get; set; }
    }

    public class ReferenceGrid
    {
        private Mesh _mesh = null;
        private BasicEffect _effect = null;       

        public ReferenceGrid(IGameApp app, DrawingSizeF gridSize, Vector2 gridSteps, Color4 color)
        {
            //*******************************************************************
            //TODO: Port this logic into a static ReferenceGrid.Create() method.*
            //*******************************************************************
            _effect = new BasicEffect(app.GraphicsDevice);
            _effect.SetWorld(Matrix.Identity);
            _effect.SetVertexColorEnabled(true);
            byte[] shaderCode = _effect.SelectShaderByteCode();
            Vector4 gridColor = color.ToVector4();
            int xSteps = (int)gridSteps.X;
            int zSteps = (int)gridSteps.Y;
            float halfWidth = gridSize.Width / 2.0f;
            float halfHeight = gridSize.Height / 2.0f;
            int nVertices = 2 * ( xSteps + zSteps );
            float xStepSize = (float)gridSize.Width / (gridSteps.X - 1);
            float zStepSize = (float)gridSize.Height / (gridSteps.Y - 1);
            VertexPositionColor[] vertices = new VertexPositionColor[nVertices];
            Vector3 vOrigin = new Vector3(-halfWidth, 0f, halfHeight);
            Vector3 vBackRight = new Vector3(gridSize.Width - halfWidth, 0f, halfHeight);
            Vector3 vFrontLeft = new Vector3(-halfWidth, 0f, -gridSize.Height + halfHeight);
            Vector3 vx1 = vOrigin;
            Vector3 vx2 = vFrontLeft;
            for (int i = 0; i < xSteps; i++)
            {
                vertices[i * 2] = new VertexPositionColor(vx1, Color.Gray.ToVector4());
                vertices[i * 2 + 1] = new VertexPositionColor(vx2, Color.Gray.ToVector4());
                vx1 += new Vector3(xStepSize, 0f, 0f);
                vx2 += new Vector3(xStepSize, 0f, 0f);
            }
            int k = zSteps * 2;
            Vector3 vz1 = vOrigin;
            Vector3 vz2 = vBackRight;
            for (int i = 0; i < zSteps; i++)
            {
                vertices[i * 2 + k] = new VertexPositionColor(vz1, Color.Gray.ToVector4());
                vertices[i * 2 + 1 + k] = new VertexPositionColor(vz2, Color.Gray.ToVector4());
                vz1 += new Vector3(0f, 0f, -zStepSize);
                vz2 += new Vector3(0f, 0f, -zStepSize);
            }
            //NOTE: We must get the shader byte code
            MeshDescription meshDesc = new MeshDescription();
            BufferDescription vertexBufferDesc = new BufferDescription();
            vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
            vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            vertexBufferDesc.SizeInBytes = vertices.Length * VertexPositionColor.ElementSize;
            vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            vertexBufferDesc.StructureByteStride = 0;
            meshDesc.VertexCount = vertices.Length;
            meshDesc.VertexBuffer = DXBuffer.Create<VertexPositionColor>(app.GraphicsDevice, vertices, vertexBufferDesc);
            meshDesc.VertexLayout = new InputLayout(app.GraphicsDevice, shaderCode, VertexPositionColor.InputElements);
            meshDesc.VertexStride = VertexPositionColor.ElementSize;
            meshDesc.Topology = PrimitiveTopology.LineList;
            _mesh = new Mesh(app, meshDesc);           
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
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
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
            Position = new Vector4(position, 1.0f);
            Color = color;
        }
    }
}
