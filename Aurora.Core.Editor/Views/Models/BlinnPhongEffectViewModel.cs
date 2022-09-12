using Aurora.Core.Editor.Util;
using CipherPark.Aurora.Core.Effects;
using SharpDX;

namespace Aurora.Core.Editor
{
    public class BlinnPhongEffectViewModel : EffectViewModel
    {
        public BlinnPhongEffectViewModel(BlinnPhongEffect2 dataModel)
            : base(dataModel)
        {
        }

        public Color AmbientColor
        {
            get => DataModel.As<BlinnPhongEffect2>().AmbientColor;
            set
            {
                DataModel.As<BlinnPhongEffect2>().AmbientColor = value;
                OnPropertyChanged(nameof(AmbientColor));
            }
        }

        public float SpecularPower
        {
            get => DataModel.As<BlinnPhongEffect2>().SpecularPower;
            set
            {
                DataModel.As<BlinnPhongEffect2>().SpecularPower = value;
                OnPropertyChanged(nameof(SpecularPower));
            }
        }

        public float Eccentricity
        {
            get => DataModel.As<BlinnPhongEffect2>().Eccentricity;
            set
            {
                DataModel.As<BlinnPhongEffect2>().Eccentricity = value;
                OnPropertyChanged(nameof(Eccentricity));
            }
        }

        public bool UseSceneLighting
        {
            get => DataModel.As<BlinnPhongEffect2>().UseSceneLighting;
            set
            {
                DataModel.As<BlinnPhongEffect2>().UseSceneLighting = value;
                OnPropertyChanged(nameof(UseSceneLighting));
            }
        }
    }
}
