using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CipherPark.Aurora.Core.Utils.Toolkit;
using SharpDX;
using SharpDX.Direct3D11;
using DXBuffer = SharpDX.Direct3D11.Buffer;

namespace CipherPark.Aurora.Core.World.Geometry
{
    public class Triangle
    {
        public Camera Camera { get; set; }
        public Matrix Transform { get; set; }
        private BasicEffect _effect = null;
        //private TriEffect _effect = null;
        private Device _device = null;
        private Mesh _mesh = null;

        public Triangle(Device device)
        {
            _device = device;
            _effect = new BasicEffect(device);            
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
            DXBuffer vBuffer = DXBuffer.Create<VertexPositionColor>(device, verts, vertexBufferDesc);
            meshDesc.VertexBuffer = vBuffer;
            meshDesc.VertexCount = verts.Length;
            meshDesc.VertexLayout = new InputLayout(device, shaderCode, VertexPositionColor.InputElements);
            meshDesc.VertexStride = VertexPositionColor.ElementSize;
            meshDesc.Topology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            Vector3[] positions = (from v in verts select new Vector3(v.Position.X, v.Position.Y, v.Position.Z)).ToArray();
            meshDesc.BoundingBox = BoundingBox.FromPoints(positions);         
            _mesh = new Mesh(device, meshDesc); 
        }

        public void Draw()
        {
            _effect.SetWorld(Transform);
            _effect.SetView(Camera.ViewMatrix);
            _effect.SetProjection(Camera.ProjectionMatrix);
            _effect.Apply();            
            _mesh.Draw();
        }
    }
}
