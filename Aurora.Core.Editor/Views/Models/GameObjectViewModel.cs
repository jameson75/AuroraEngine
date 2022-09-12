using Aurora.Core.Editor.Environment;
using Aurora.Core.Editor.Util;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World;
using SharpDX;
using System;

namespace Aurora.Core.Editor
{
    public class GameObjectViewModel : ViewModelBase<GameObject>
    {        
        private EffectViewModel selectedEffect;

        public GameObjectViewModel(GameObject dataModel)
            : base(dataModel)
        {
            InitializeSelectedEffect();
        }        

        public string ModelFileName
        {
            get
            {
                return DataModel.GetContext<GameObjectMeta>()?.ResourceFilename;
            }
        }

        public string ModelType
        {
            get => DataMap.GetModelDisplayName(DataModel.GetGameModel());
        }

        public string ModelStatistics
        {
            get => GenerateStatistics();
        }

        public bool ModelHasMesh
        {
            get => DataModel.GetGameModel().GetMesh() != null;
        }

        public string EffectName
        {
            get => SelectedEffect?.Name;
            set
            {               
                UpdateSelectedEffectByName(value);
                OnPropertyChanged(nameof(EffectName));
            }
        }

        public EffectViewModel SelectedEffect
        {
            get => selectedEffect;            
        }

        public GameObjectType GameObjectType
        {
            get => DataMap.GetDomGameObjectType(DataModel);
        }

        public Color LightDiffuse
        {
            get => DataModel.GetLighting().Diffuse;
            set
            {
                DataModel.GetLighting().Diffuse = value;;
                OnPropertyChanged(nameof(LightDiffuse));
            }
        }

        public Vector3 LightDirection
        {
            get => DataModel.GetLighting().As<DirectionalLight>().Direction;
            set
            {
                DataModel.GetLighting().As<DirectionalLight>().Direction = value;
                OnPropertyChanged(nameof(LightDirection));               
            }
        }

        public Vector3 LightPosition
        {
            get => DataModel.GetLighting().As<PointLight>().WorldPosition();           
        }

        public Dom.LightType LightType
        {
            get => DataMap.GetDomLightType(DataModel.GetLighting());
        }

        private string GenerateStatistics()
        {
            var mesh = DataModel.GetGameModel()
                                .GetMesh();

            if (mesh != null)
            {
                if (mesh.Description.Topology == SharpDX.Direct3D.PrimitiveTopology.TriangleList)
                {
                    var count = mesh.Description.IndexCount > 0 ?
                        mesh.Description.IndexCount / 3 :
                        mesh.Description.VertexCount / 3;

                    return $"{count} Triangles";
                }

                if (mesh.Description.Topology == SharpDX.Direct3D.PrimitiveTopology.LineList)
                {
                    var count = mesh.Description.IndexCount > 0 ?
                        mesh.Description.IndexCount / 2 :
                        mesh.Description.VertexCount / 2;

                    return $"{count} Lines";
                }
            }

            return "N/A";
        }

        private void UpdateSelectedEffectByName(string effectName)
        {            
            switch (effectName)
            {
                case EffectNames.BlinnPhong:
                    var effect = new BlinnPhongEffect2(DataModel.Game, SurfaceVertexType.PositionNormalColor);
                    effect.AmbientColor = Color.White;
                    selectedEffect = new BlinnPhongEffectViewModel(effect);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported effect name.");
            }

            var gameModel = DataModel.GetGameModel();
            if (gameModel != null)
            {
                gameModel.Effect = selectedEffect.DataModel;                                           
            }
            
            OnPropertyChanged(nameof(SelectedEffect));     
        }

        private void InitializeSelectedEffect()
        {
            var effect = DataModel.GetGameModel()?.Effect;
            if (effect != null)
            {
                if (effect is BlinnPhongEffect2)
                {
                    selectedEffect = new BlinnPhongEffectViewModel(effect.As<BlinnPhongEffect2>());
                }

                else
                {
                    throw new InvalidOperationException("Unspported effect.");
                }
            }
        }
    }
}
