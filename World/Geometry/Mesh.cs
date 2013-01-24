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
using CipherPark.AngelJacket.Core.Utils.Toolkit;

namespace CipherPark.AngelJacket.Core.World.Geometry
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
                _app.GraphicsDeviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R16_SInt, 0);
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
            Transform = Matrix.Identity;

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
            BasicVertexPositionColor[] vertices = new BasicVertexPositionColor[nVertices];
            Vector3 vOrigin = new Vector3(-halfWidth, 0f, halfHeight);
            Vector3 vBackRight = new Vector3(gridSize.Width - halfWidth, 0f, halfHeight);
            Vector3 vFrontLeft = new Vector3(-halfWidth, 0f, -gridSize.Height + halfHeight);
            Vector3 vx1 = vOrigin;
            Vector3 vx2 = vFrontLeft;
            for (int i = 0; i < xSteps; i++)
            {
                vertices[i * 2] = new BasicVertexPositionColor(vx1, Color.Gray.ToVector4());
                vertices[i * 2 + 1] = new BasicVertexPositionColor(vx2, Color.Gray.ToVector4());
                vx1 += new Vector3(xStepSize, 0f, 0f);
                vx2 += new Vector3(xStepSize, 0f, 0f);
            }
            int k = zSteps * 2;
            Vector3 vz1 = vOrigin;
            Vector3 vz2 = vBackRight;
            for (int i = 0; i < zSteps; i++)
            {
                vertices[i * 2 + k] = new BasicVertexPositionColor(vz1, Color.Gray.ToVector4());
                vertices[i * 2 + 1 + k] = new BasicVertexPositionColor(vz2, Color.Gray.ToVector4());
                vz1 += new Vector3(0f, 0f, -zStepSize);
                vz2 += new Vector3(0f, 0f, -zStepSize);
            }
            //NOTE: We must get the shader byte code
            MeshDescription meshDesc = new MeshDescription();
            BufferDescription vertexBufferDesc = new BufferDescription();
            vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
            vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            vertexBufferDesc.SizeInBytes = vertices.Length * BasicVertexPositionColor.ElementSize;
            vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            vertexBufferDesc.StructureByteStride = 0;
            meshDesc.VertexCount = vertices.Length;
            meshDesc.VertexBuffer = DXBuffer.Create<BasicVertexPositionColor>(app.GraphicsDevice, vertices, vertexBufferDesc);
            meshDesc.VertexLayout = new InputLayout(app.GraphicsDevice, shaderCode, BasicVertexPositionColor.InputElements);
            meshDesc.VertexStride = BasicVertexPositionColor.ElementSize;
            meshDesc.Topology = PrimitiveTopology.LineList;
            _mesh = new Mesh(app, meshDesc);           
        }

        public void Draw(long gameTime)
        {
            _effect.SetWorld(Transform);
            _effect.SetView(Camera.ViewMatrix);
            _effect.SetProjection(Camera.ProjectionMatrix);
            _effect.Apply();
            _mesh.Draw(gameTime);
        }

        public Camera Camera { get; set; }

        public Matrix Transform { get; set; }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BasicVertexPositionColor
    {       
        public Vector4 Position;
        public Vector4 Color;     
        
        private static InputElement[] _inputElements = null;
        private  static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        static BasicVertexPositionColor()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
             };
            _elementSize = 32;
        }        
        public BasicVertexPositionColor(Vector3 position)
        {
            Position = new Vector4(position, 1.0f);
            Color = SharpDX.Color.Transparent.ToVector4();
        }
        public BasicVertexPositionColor(Vector3 position, Vector4 color)
        {
            Position = new Vector4(position, 1.0f);
            Color = color;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BasicVertexPositionTexture
    {
        public Vector4 Position;
        public Vector2 TextureCoord;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        static BasicVertexPositionTexture()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0)
             };
            _elementSize = 24;
        }
        public BasicVertexPositionTexture(Vector3 position)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = Vector2.Zero; 
        }
        public BasicVertexPositionTexture(Vector3 position, Vector2 textureCoord)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = textureCoord;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct ScreenVertexPositionTexture
    {
        public Vector2 Position;
        public Vector2 TextureCoord;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        static ScreenVertexPositionTexture()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32_Float, 0, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 8, 0)
             };
            _elementSize = 16;
        }
        public ScreenVertexPositionTexture(Vector2 position)
        {
            Position = position;
            TextureCoord = Vector2.Zero;
        }
        public ScreenVertexPositionTexture(Vector2 position, Vector2 textureCoord)
        {
            Position = position;
            TextureCoord = textureCoord;
        }
    }
}
