using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CipherPark.AngelJacket.Core.Utils.Interop;
using SharpDX;
using SharpDX.Direct3D11;
using DXBuffer = SharpDX.Direct3D11.Buffer;

namespace CipherPark.AngelJacket.Core.World
{
    public class Triangle
    {
        public Camera Camera { get; set; }
        public Matrix Transform { get; set; }
        private BasicEffect _effect = null;
        //private TriEffect _effect = null;
        private IGameApp _app = null;
        private Mesh _mesh = null;

        public Triangle(IGameApp app)
        {
            _app = app;
            _effect = new BasicEffect(app.GraphicsDevice);            
            _effect.SetVertexColorEnabled(true);
            byte[] shaderCode = _effect.SelectShaderByteCode();
            //_effect = new TriEffect(app.GraphicsDevice);
            //_effect.Load("..\\..\\..\\Debug\\angelShader-vs.cso", "..\\..\\..\\Debug\\angelShader-ps.cso");
            //_effect.World = Matrix.Identity;
            //byte[] shaderCode = _effect.VertexShaderByteCode;
            VertexPositionColor[] verts = new VertexPositionColor[3];
            verts[0] = new VertexPositionColor(new Vector3(0, 0, 20), Color.Blue.ToVector4());
            verts[1] = new VertexPositionColor(new Vector3(15, 0, 0), Color.Blue.ToVector4());
            verts[2] = new VertexPositionColor(new Vector3(-15, 0, 0), Color.Blue.ToVector4());
            MeshDescription meshDesc = new MeshDescription();
            BufferDescription vertexBufferDesc = new BufferDescription();
            vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
            vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            vertexBufferDesc.SizeInBytes = verts.Length * VertexPositionColor.ElementSize;
            vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            vertexBufferDesc.StructureByteStride = 0;            
            DXBuffer vBuffer = DXBuffer.Create<VertexPositionColor>(app.GraphicsDevice, verts, vertexBufferDesc);
            meshDesc.VertexBuffer = vBuffer;
            meshDesc.VertexCount = verts.Length;
            meshDesc.VertexLayout = new InputLayout(app.GraphicsDevice, shaderCode, VertexPositionColor.InputElements);
            meshDesc.VertexStride = VertexPositionColor.ElementSize;
            meshDesc.Topology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            _mesh = new Mesh(app, meshDesc); 
        }

        public void Draw(long gameTime)
        {
            _effect.SetWorld(Transform);
            _effect.SetView(Camera.ViewMatrix);
            _effect.SetProjection(Camera.ProjectionMatrix);
            //_effect.World = Matrix.Identity;
            //_effect.View = Camera.ViewMatrix;
            //_effect.Projection = Camera.ProjectionMatrix;
            _effect.Apply();
            RasterizerStateDescription stateDesc = this._app.GraphicsDeviceContext.Rasterizer.State.Description;
            stateDesc.CullMode = CullMode.None;
            this._app.GraphicsDeviceContext.Rasterizer.State = new RasterizerState(_app.GraphicsDevice, stateDesc);
            _mesh.Draw(gameTime);
        }
    }
}
