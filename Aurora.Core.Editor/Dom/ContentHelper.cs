using Aurora.Core.Editor.Environment;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.World;

namespace Aurora.Core.Editor
{
    public class ContentHelper
    {
        public static GameObject ImportGameObject(string resourceFilename, SurfaceEffect effect)
        {
            /*
            var model = ContentImporter.ImportX(
                   effect.Game,
                   resourceFilename,
                   effect,
                   XFileChannels.Mesh | XFileChannels.DefaultMaterialColor | XFileChannels.DeclNormals, XFileImportOptions.IgnoreMissingColors);
            */

            var model = XFileImporter.ImportX(
                effect.Game,
                resourceFilename,
                effect,
                XFileImportOptions.IgnoreMissingColors | XFileImportOptions.GenerateMissingNormals);

            return new GameObject(
                effect.Game, 
                new object []
                { 
                    new GameObjectMeta { ResourceFilename = resourceFilename },
                    model,
                })
            {
                Renderer = new ModelRenderer(model),
            };           
        }
    }
}