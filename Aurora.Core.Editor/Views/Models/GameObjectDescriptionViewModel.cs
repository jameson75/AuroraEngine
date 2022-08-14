using Aurora.Core.Editor.Environment;
using Aurora.Core.Editor.Util;
using CipherPark.Aurora.Core.Effects;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World;
using System.Collections.Generic;

namespace Aurora.Core.Editor
{
    public class GameObjectDescriptionViewModel : ViewModelBase<GameObject>
    {
        private Dictionary<string, EffectViewModel> cachedEffects = new Dictionary<string, EffectViewModel>();        
        private EffectViewModel selectedEffect;
        private GameObject dataModel;        

        public GameObjectDescriptionViewModel(GameObject dataModel) 
            : base(dataModel)
        {            
        }

        public string FileName 
        {
            get
            {
                return dataModel.GetContext<GameObjectMeta>()?.Filename;
            }
        }
        
        public string ModelType
        {
            get => DataMapper.GetGameModelTypeName(dataModel.Renderer.As<ModelRenderer>().Model);
        }

        public string EffectName
        {
            get => SelectedEffect?.Name;
            set
            { 
                OnUpdateCurrentEffect(value);                             
            }
        }

        public EffectViewModel SelectedEffect
        {
            get => selectedEffect;
            set
            {
                selectedEffect = value;
                var model = DataModel.Renderer.As<ModelRenderer>().Model;
                model.Effect = selectedEffect.DataModel;
                OnPropertyChanged(nameof(SelectedEffect));
                OnPropertyChanged(nameof(EffectName));
            }
        }

        private void OnUpdateCurrentEffect(string effectName)
        {
            EffectViewModel effectViewModel;
            switch (effectName)
            {
                case EffectNames.BlinnPhong:
                    if (!cachedEffects.ContainsKey(EffectNames.BlinnPhong))
                    {
                        var effect = new BlinnPhongEffect2(DataModel.Game, SurfaceVertexType.PositionNormalColor);
                        effectViewModel = new BlinnPhongEffectViewModel(effect);
                        cachedEffects.Add(EffectNames.BlinnPhong, effectViewModel);
                    }
                    SelectedEffect = cachedEffects[EffectNames.BlinnPhong];
                    break;

                case EffectNames.FlatEffect:
                    if (!cachedEffects.ContainsKey(EffectNames.FlatEffect))
                    {
                        var effect = new FlatEffect(dataModel.Game, SurfaceVertexType.PositionNormalColor);
                        effectViewModel = new FlatEffectViewModel(effect);
                        cachedEffects.Add(EffectNames.FlatEffect, effectViewModel);
                    }
                    SelectedEffect = cachedEffects[EffectNames.FlatEffect];
                    break;
            }
        }       
    }
}
