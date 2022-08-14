using Aurora.Core.Editor.Environment;
using Aurora.Core.Editor.Util;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Content;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Scene;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using GameSceneNode = CipherPark.Aurora.Core.World.Scene.SceneNode;

namespace Aurora.Core.Editor.Dom
{
    public class DomSerializer
    {
        #region Loading
        public static ProjectViewModel LoadProject(IEditorGameApp app, string filePath)
        {
            var json = System.IO.File.ReadAllText(filePath);
            var project = JsonConvert.DeserializeObject<Project>(json);
            ProjectViewModel viewModel = new ProjectViewModel(app);
            //app.ResetScene();
            RestoreScene(project.Scene, viewModel.Scene, app);
            return viewModel;
        }

        private static void RestoreScene(Scene scene, SceneViewModel viewModel, IEditorGameApp app)
        {
            foreach (var sceneNode in scene.Nodes)
            {
                RestoreSceneNode(sceneNode, app.Scene.Nodes, viewModel.Nodes, app);
            }
        }

        private static void RestoreSceneNode(SceneNode sceneNode, IList<GameSceneNode> destinationDataModels, IList<SceneNodeViewModel> destinationViewModels, IEditorGameApp app)
        {          
            var dataModel = new GameObjectSceneNode(app)
            {
                Name = sceneNode.Name,
                Transform = new Transform(new Matrix(sceneNode.Matrix)),
                Flags = sceneNode.Flags,
                Visible = sceneNode.Visible,
                GameObject = CreateGameObject(app, sceneNode),
            };
            destinationDataModels.Add(dataModel);

            var viewModel = new SceneNodeViewModel(dataModel);
            destinationViewModels.Add(viewModel);

            foreach (var childSceneNode in sceneNode.Children)
            {
                RestoreSceneNode(childSceneNode, dataModel.Children, destinationViewModels, app);
            }            
        }

        private static GameObject CreateGameObject(IEditorGameApp game, SceneNode sceneNode)
        {
            if (sceneNode.GameObjectType == GameObjectType.GeometricModel)
            {
                SurfaceEffect effect = CreateEffect(game, sceneNode.GameObjectDescription.EffectName);

                var model = ContentImporter.ImportX(
                    game,
                    sceneNode.GameObjectDescription.Filename,
                    effect,
                    XFileChannels.Mesh | XFileChannels.DefaultMaterialColor | XFileChannels.DeclNormals, XFileImportOptions.IgnoreMissingColors);

                return new GameObject(game)
                {
                    Renderer = new ModelRenderer(model),
                };
            }

            return null;
        }

        private static SurfaceEffect CreateEffect(IEditorGameApp app, string effectName)
        {
            switch (effectName)
            {
                case EffectNames.BlinnPhong:
                    return new BlinnPhongEffect2(app, SurfaceVertexType.PositionNormalColor);
                
                case EffectNames.FlatEffect:
                    return new FlatEffect(app, SurfaceVertexType.PositionNormalColor);
                
                default:
                    throw new InvalidOperationException($"Unsupported effectName {effectName}");
            }
        }
        #endregion

        #region Storage
        public static void SaveProject(ProjectViewModel viewModel, string filePath)
        {
            var project = BuildProjectDom(viewModel);
            var json = JsonConvert.SerializeObject(project);
            File.WriteAllText(filePath, json);
        }

        private static Project BuildProjectDom(ProjectViewModel viewModel)
        {
            return new Project()
            {
                Scene = BuildSceneDom(viewModel.Scene),
            };
        }

        private static Scene BuildSceneDom(SceneViewModel viewModel)
        {
            var scene = new Scene();
            foreach(var gameSceneNode in viewModel.DataModel.Nodes)
            {
                BuildSceneNode(gameSceneNode, scene.Nodes);
            }
            return scene;
        }

        private static void BuildSceneNode(GameSceneNode gameSceneNode, IList<SceneNode> domNodes)
        {
            var domNode = new SceneNode()
            {
                Name = gameSceneNode.Name,
                Matrix = gameSceneNode.Transform.ToMatrix().ToArray(),
                Flags = gameSceneNode.Flags,
                Visible = gameSceneNode.Visible,
                GameObjectType = CreateGameObjectType(gameSceneNode),
                GameObjectDescription = CreateGameObjectDescription(gameSceneNode),
            };

            domNodes.Add(domNode);

            foreach(var gameSceneChildNode in gameSceneNode.Children)
            {
                BuildSceneNode(gameSceneChildNode, domNode.Children);
            }
        }

        private static GameObjectDescription CreateGameObjectDescription(GameSceneNode gameSceneNode)
        {
            var gameModel = gameSceneNode.As<GameObjectSceneNode>()
                            ?.GameObject
                            .Renderer.As<ModelRenderer>()
                            ?.Model;

            if (gameModel != null)
            {
                return new GameObjectDescription()
                {
                    EffectName = DataMapper.GetEffectName(gameModel.Effect),
                    Filename = gameSceneNode.As<GameObjectSceneNode>()
                                            ?.GameObject
                                            .GetContext<GameObjectMeta>()
                                            ?.Filename,
                };
            }

            return null;
        }

        private static GameObjectType CreateGameObjectType(GameSceneNode gameSceneNode)
            => DataMapper.GetGameObjectType(gameSceneNode.As<GameObjectSceneNode>()?.GameObject);        

        #endregion
    }
}
