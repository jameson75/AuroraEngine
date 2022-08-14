using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Geometry;
using Aurora.Core.Editor.Environment;

namespace Aurora.Core.Editor.Util
{
    public class DataMapper
    {
        public static GameObjectType GetGameObjectType(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return GameObjectType.Unknown;
            }

            else if( gameObject.Renderer is ModelRenderer)
            {
                return GameObjectType.GeometricModel;
            }

            return GameObjectType.Unknown;
        }

        public static string GetEffectName(SurfaceEffect dataModel)
        {
            if (dataModel is BlinnPhongEffect2)
            {
                return EffectNames.BlinnPhong;
            }

            else if (dataModel is FlatEffect)
            {
                return EffectNames.FlatEffect;
            }

            return EffectNames.Unknown;
        }       

        public static string GetGameModelTypeName(Model gameModel)
        {
            if (gameModel is StaticMeshModel)
            {
                return ModelTypeNames.Static;
            }

            else if (gameModel is MultiMeshModel)
            {
                return ModelTypeNames.Multi;
            }

            else if (gameModel is SkinnedMeshModel)
            {
                return ModelTypeNames.Rigged;
            }

            return ModelTypeNames.Unknown;
        }
        
        public static string GetNameFromFilename(string filename)
        {
            if (filename == null)
            {
                return null;
            }

            return System.IO.Path.GetFileNameWithoutExtension(filename);
        }
    }
}
