using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.World.Scene;
using Aurora.Core.Editor.Environment;

namespace Aurora.Core.Editor.Util
{
    public class DataMap
    {
        public static GameObjectType GetDomGameObjectType(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return GameObjectType.Unknown;
            }

            else if( gameObject.Renderer is ModelRenderer)
            {
                return GameObjectType.GameModel;
            }

            else if (gameObject.GetContext<Light>() != null)
            {
                return GameObjectType.Light;
            }

            return GameObjectType.Unknown;
        }

        public static string GetEffectDisplayName(SurfaceEffect dataModel)
        {
            if (dataModel is BlinnPhongEffect2)
            {
                return EffectNames.BlinnPhong;
            }

            return EffectNames.Unknown;
        }       

        public static string GetModelDisplayName(Model gameModel)
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
        
        public static string GetModelDisplayNameFromFilename(string filename)
        {
            if (filename == null)
            {
                return null;
            }

            return System.IO.Path.GetFileNameWithoutExtension(filename);
        }

        public static Dom.NodeType GetDomNodeType(SceneNode dataModel)
        {
            if( dataModel is CameraSceneNode)
            {
                return Dom.NodeType.Camera;
            }

            else if( dataModel is GameObjectSceneNode)
            {
                if( dataModel.IsEditorNode())
                {
                    return Dom.NodeType.Designer;
                }

                return Dom.NodeType.GameObject;
            }

            return Dom.NodeType.Unknown;
        }

        internal static Dom.LightType GetDomLightType(Light light)
        {
            if (light is PointLight)
            {
                return Dom.LightType.Point;
            }

            else if (light is DirectionalLight)
            {
                return Dom.LightType.Directional;
            }

            return Dom.LightType.Unknown;
        }
    }   
}
