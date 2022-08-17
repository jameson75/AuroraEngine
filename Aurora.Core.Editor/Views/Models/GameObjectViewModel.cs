using Aurora.Core.Editor.Environment;
using Aurora.Core.Editor.Util;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World;
using SharpDX;
using System;
using System.Collections.Generic;

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
                return DataModel.GetContext<GameObjectMeta>()?.ModelFileName;
            }
        }

        public string ModelType
        {
            get => DataLookup.GetGameModelTypeName(DataModel.GetGameModel());
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
            get => DataLookup.GetGameObjectType(DataModel);
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
