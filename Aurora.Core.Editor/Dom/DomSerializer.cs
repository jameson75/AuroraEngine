using Aurora.Core.Editor.Environment;
using Aurora.Core.Editor.Util;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Services;
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
            RestoreScene(project.Scene, viewModel.Scene, app);
            return viewModel;
        }

        private static void RestoreScene(Scene scene, SceneViewModel viewModel, IEditorGameApp app)
        {
            foreach (var sceneNode in scene.Nodes)
            {
                RestoreSceneNodeTree(sceneNode, app.Scene.Nodes, viewModel.Nodes, app);
            }
        }

        private static void RestoreSceneNodeTree(SceneNode sceneNode, IList<GameSceneNode> destinationDataModels, IList<SceneNodeViewModel> destinationViewModels, IEditorGameApp app)
        {
            if (sceneNode.NodeType == NodeType.GameObject)
            {
                var dataModel = new GameObjectSceneNode(app)
                {
                    Name = sceneNode.Name,
                    Transform = new Transform(new Matrix(sceneNode.Matrix)),
                    Flags = sceneNode.Flags,
                    Visible = sceneNode.Visible,
                    GameObject = CreateGameObject(app, sceneNode.GameObject),
                };                


                var viewModel = new SceneNodeViewModel(dataModel);
                
                destinationDataModels.Add(dataModel);
                destinationViewModels.Add(viewModel);

                foreach (var childSceneNode in sceneNode.Children)
                {
                    RestoreSceneNodeTree(childSceneNode, dataModel.Children, destinationViewModels, app);
                }
            }
        }

        private static CipherPark.Aurora.Core.World.GameObject CreateGameObject(IEditorGameApp game, GameObject gameObject)
        {
            if (gameObject.GameObjectType == GameObjectType.GameModel)
            {
                SurfaceEffect effect = CreateGameEffect(game, gameObject.ModelEffect);
                var dataModel = ContentHelper.ImportGameObject(gameObject.ResourceFilename, effect);
                return dataModel;
            }

            else if (gameObject.GameObjectType == GameObjectType.Light)
            {
                var dataModel = new CipherPark.Aurora.Core.World.GameObject(game, new[] { CreateGameLight(gameObject.Light) });
                return dataModel;
            }

            return null;
        }

        private static CipherPark.Aurora.Core.Effects.Light CreateGameLight(Light light)
        {
            switch (light.Type)
            {
                case LightType.Point:
                    return new PointLight()
                    {
                        Diffuse = Color.FromRgba(light.Diffuse),
                        Transform = new Transform(new Matrix()),
                    };
                case LightType.Directional:
                    return new DirectionalLight()
                    {
                        Diffuse = Color.FromRgba(light.Diffuse),
                        Direction = new Vector3(light.Direction),
                    };
                default:
                    throw new InvalidOperationException("Unsupported light type.");
            }
        }

        private static SurfaceEffect CreateGameEffect(IEditorGameApp app, ModelEffect modelEffect)
        {
            switch (modelEffect.EffectName)
            {
                case EffectNames.BlinnPhong:
                    var effect = new BlinnPhongEffect2(app, SurfaceVertexType.PositionNormalColor);
                    if (modelEffect != null)
                    {                        
                        effect.AmbientColor = Color.FromRgba(modelEffect.AmbientColor);
                        effect.SpecularPower = modelEffect.SpecularPower;
                        effect.Eccentricity = modelEffect.Eccentricity;
                        effect.EnableBackFace = modelEffect.EnableBackFace;
                        effect.UseSceneLighting = modelEffect.UseSceneLighting;
                    }                  
                    return effect;
                default:
                    throw new InvalidOperationException($"Unsupported effectName {modelEffect.EffectName}");
            }
        }

        private static CipherPark.Aurora.Core.Effects.Light CreateGameLight(LightType lightType, Light light)
        {
            switch (lightType)
            {
                case LightType.Point:
                    return new PointLight()
                    {
                        Diffuse = Color.FromRgba(light.Diffuse),
                        Transform = new Transform(new Matrix(light.Matrix)),
                    };
                case LightType.Directional:
                    return new DirectionalLight
                    {
                        Diffuse = Color.FromRgba(light.Diffuse),
                        Direction = new Vector3(light.Direction),
                    };
                default:
                    throw new InvalidOperationException($"Unspported light type {lightType}");
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
            foreach (var gameSceneNode in viewModel.DataModel.Nodes)
            {
                BuildSceneNodeTree(gameSceneNode, scene.Nodes);
            }
            return scene;
        }

        private static void BuildSceneNodeTree(GameSceneNode gameSceneNode, IList<SceneNode> domNodes)
        {
            var gameSceneNodeType = DataMap.GetDomNodeType(gameSceneNode);
            if (gameSceneNodeType == NodeType.GameObject)
            {
                var domNode = new SceneNode()
                {
                    Name = gameSceneNode.Name,
                    Matrix = gameSceneNode.Transform.ToMatrix().ToArray(),
                    Flags = gameSceneNode.Flags,
                    Visible = gameSceneNode.Visible,
                    NodeType = gameSceneNodeType,
                    GameObject = CreateDomGameObject(gameSceneNode),
                };

                domNodes.Add(domNode);

                foreach (var gameSceneChildNode in gameSceneNode.Children)
                {
                    BuildSceneNodeTree(gameSceneChildNode, domNode.Children);
                }
            }
        }

        private static GameObject CreateDomGameObject(GameSceneNode gameSceneNode)
        {
            var gameObject = gameSceneNode.GetGameObject();
            
            var gameModel = gameObject.GetGameModel();
            if (gameModel != null)
            {
                return new GameObject()
                {
                    GameObjectType = DataMap.GetDomGameObjectType(gameObject),
                    ResourceFilename = gameObject.GetResourceFilename(),
                    ModelEffect = CreateDomModelEffect(gameModel.Effect),                    
                };
            }

            var light = gameObject.GetLighting();
            if (light != null)
            {
                return new GameObject()
                {
                    GameObjectType = DataMap.GetDomGameObjectType(gameSceneNode.As<GameObjectSceneNode>()?.GameObject),                    
                    Light = CreateDomLight(light),
                };
            }

            return null;
        }

        private static ModelEffect CreateDomModelEffect(SurfaceEffect gameEffect)
        {
            if (gameEffect is BlinnPhongEffect2)
            {
                var blinnPhongEffect = gameEffect.As<BlinnPhongEffect2>();
                return new ModelEffect
                {
                    EffectName = DataMap.GetEffectDisplayName(blinnPhongEffect),
                    AmbientColor = blinnPhongEffect.AmbientColor.ToRgba(),
                    EnableBackFace = blinnPhongEffect.EnableBackFace,
                    SpecularPower = blinnPhongEffect.SpecularPower,
                    Eccentricity = blinnPhongEffect.Eccentricity,
                    UseSceneLighting = blinnPhongEffect.UseSceneLighting,
                };
            }

            return null;
        }
        
        private static Light CreateDomLight(CipherPark.Aurora.Core.Effects.Light gameLight)
        {
            if (gameLight is PointLight)
            {
                return new Light()
                {
                    Type = LightType.Point,
                    Diffuse = gameLight.Diffuse.ToRgba(),
                    Matrix = ((PointLight)gameLight).Transform.ToMatrix().ToArray(),
                };
            }

            else if (gameLight is DirectionalLight)
            {
                return new Light()
                {
                    Type = LightType.Directional,
                    Diffuse = gameLight.Diffuse.ToRgba(),
                    Direction = ((DirectionalLight)gameLight).Direction.ToArray(),
                };
            }

            return null;
        }

        #endregion
    }

    public class ModelEffect
    {
        public string EffectName { get; set; }
        public int AmbientColor { get; set; }
        public float SpecularPower { get; set; }
        public float Eccentricity { get; set; }
        public bool EnableBackFace { get; set; }
        public bool UseSceneLighting { get; set; }
    }

    public class Light
    {
        public LightType Type { get; set; }
        public int Diffuse { get; set; }
        public float[] Matrix { get; set; }
        public float[] Direction { get; set; }
    }

    public enum LightType
    {
        Unknown,
        Point,
        Directional
    }
}
