using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Scene;

namespace Aurora.Sample.Editor
{
    public class MainWindowController
    {
        public string ChooseFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".x";
            dialog.Filter = "Direct X (.x)|*.x";
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public byte[] LoadFile(string filePath)
        {
            return System.IO.File.ReadAllBytes(filePath);
        }

        public void ImportModel(EditorGameApp game, string filePath)
        {
            /*
            BlinnPhongEffect2 effect = new BlinnPhongEffect2(game, SurfaceVertexType.InstancePositionNormalColor);                        
            effect.AmbientColor = SharpDX.Color.White;
            effect.Lighting = new Light[]
            {
                    new PointLight
                    {
                        Diffuse = SharpDX.Color.White,
                        Transform = new CipherPark.Aurora.Core.Animation.Transform(new Vector3(500, 500, 500))
                    },
                    new PointLight
                    {
                        Diffuse = SharpDX.Color.White,
                        Transform = new CipherPark.Aurora.Core.Animation.Transform(new Vector3(-500, -500, -500))
                    }
            };
            */
            FlatEffect effect = new FlatEffect(game, SurfaceVertexType.PositionColor);
            var model = ContentImporter.ImportX(game, filePath, effect, XFileChannels.Mesh | XFileChannels.DefaultMaterialColor , XFileImportOptions.IgnoreMissingColors);
            game.Scene.Nodes.Add(new GameObjectSceneNode(game)
            {
                GameObject = new GameObject(game)
                {
                    Renderer = new ModelRenderer(game)
                    {
                        Model = model
                    }
                },
            });
        }
    }
}
