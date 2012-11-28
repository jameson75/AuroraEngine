using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CipherPark.AngelJacket.Core.Utils.Interop;
using SharpDX;
using SharpDX.Direct3D11;

namespace CipherPark.AngelJacket.Core.World
{
    public class Triangle
    {
        public Camera Camera { get; set; }
        private BasicEffect _effect = null;
        private IGameApp app = null;
        private Mesh<VertexPositionColor> _mesh = null;

        public Triangle(IGameApp app)
        {
            _effect = new BasicEffect(app.GraphicsDevice);
            _effect.SetWorld(Matrix.Identity);
            _effect.SetVertexColorEnabled(true);
            byte[] shaderCode = _effect.SelectShaderByteCode();
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
            meshDesc.VertexBufferDescription = vertexBufferDesc;
            meshDesc.VertexLayout = new InputLayout(app.GraphicsDevice, shaderCode, VertexPositionColor.InputElements);
            meshDesc.VertexStride = VertexPositionColor.ElementSize;
            meshDesc.Topology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            _mesh = new Mesh<VertexPositionColor>(app, meshDesc, verts); 
        }

        public void Draw(long gameTime)
        {
            _effect.SetWorld(Matrix.Identity);
            _effect.SetView(Camera.ViewMatrix);
            _effect.SetProjection(Camera.ProjectionMatrix);
            _effect.Apply();
            _mesh.Draw(gameTime);
        }
    }
}
