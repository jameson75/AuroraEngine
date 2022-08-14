using CipherPark.Aurora.Core.Effects;

namespace Aurora.Core.Editor
{
    public class BlinnPhongEffectViewModel : EffectViewModel
    {
        private float ambientColor;
        private float specularPower;
        private float eccentricity;

        public BlinnPhongEffectViewModel(SurfaceEffect dataModel)
            : base(dataModel)
        {
        }

        public float AmbientColor
        {
            get => ambientColor;
            set
            {
                ambientColor = value;
                OnPropertyChanged(nameof(AmbientColor));
            }
        }

        public float SpecularPower
        {
            get => specularPower;
            set
            {
                specularPower = value;
                OnPropertyChanged(nameof(SpecularPower));
            }
        }

        public float Eccentricity
        {
            get => eccentricity;
            set
            {
                eccentricity = value;
                OnPropertyChanged(nameof(Eccentricity));
            }
        }
    }
}
