using SharpDX.Direct3D11;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.World.Geometry
{
    /// <summary>
    /// 
    /// </summary>
    public class MeshTextures
    {            
        public string MeshName { get; set; }
        public Texture2D Texture { get; set; }
        public ModelTextureChannel Channel { get; set; }
    }

    /*
    public static class ModelExtension
    {       
        public static void UpdateInstanceData(this Model model, IEnumerable<Matrix> data)
        {
            //TODO: Add support for Rigged Model

            if (model is BasicModel == false)
                throw new NotSupportedException("This method is not supported for the model type. Only BasicModel is supported");

            Mesh mesh = GetMesh(model);

            if (mesh == null)
                throw new InvalidOperationException("Model does not contain a mesh");                

            if (mesh.IsInstanced == false || mesh.IsDynamic == false)
                throw new InvalidOperationException("Cannot set instance data to a mesh that is not both dynamic and instanced.");

            if (data.Count() > 0)
                mesh.UpdateVertexStream<InstanceVertexData>(data.Select(m => new InstanceVertexData() { Matrix = Matrix.Transpose(m) }).ToArray());
        }
        
        public static bool IsDynamicAndInstanced(this Model model)
        {
            Mesh mesh = GetMesh(model);
            if (mesh == null)
                return false;
            else
                return mesh.IsDynamic && mesh.IsInstanced;
        }

        private static Mesh GetMesh(Model model)
        {
            if (model is BasicModel)
            {
                return ((BasicModel)model).Mesh;
            }

            else
                return null;
        }
    }
    */
}
