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
using CipherPark.Aurora.Core.Utils.Toolkit;


namespace CipherPark.Aurora.Core.World.Geometry
{
    public class ReferenceGrid
    {
        private Mesh _mesh = null;
        private BasicEffect _effect = null;

        public ReferenceGrid(SharpDX.Direct3D11.Device device, Size2F gridSize, Vector2 gridSteps, Color4 color)
        {
            Transform = Matrix.Identity;

            //*******************************************************************
            //TODO: Port this logic into a static ReferenceGrid.Create() method.*
            //*******************************************************************
            _effect = new BasicEffect(device);
            _effect.SetWorld(Matrix.Identity);
            _effect.SetVertexColorEnabled(true);
            byte[] shaderCode = _effect.SelectShaderByteCode();
            Vector4 gridColor = color.ToVector4();
            int xSteps = (int)gridSteps.X;
            int zSteps = (int)gridSteps.Y;
            float halfWidth = gridSize.Width / 2.0f;
            float halfHeight = gridSize.Height / 2.0f;
            int nVertices = 2 * (xSteps + zSteps);
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
            meshDesc.VertexBufferDescription = vertexBufferDesc;
            meshDesc.VertexLayout = new InputLayout(device, shaderCode, VertexPositionColor.InputElements);
            meshDesc.VertexStride = VertexPositionColor.ElementSize;
            meshDesc.Topology = PrimitiveTopology.LineList;
            _mesh = Mesh.Create(device, meshDesc, vertices, null);
        }

        public void Draw()
        {
            _effect.SetWorld(Transform);
            _effect.SetView(Camera.ViewMatrix);
            _effect.SetProjection(Camera.ProjectionMatrix);
            _effect.Apply();
            _mesh.Draw();
        }

        public Camera Camera { get; set; }

        public Matrix Transform { get; set; }
    }
}
