using CipherPark.Aurora.Core.Effects;

namespace Aurora.Core.Editor
{
    public class FlatEffectViewModel : EffectViewModel
    {
        private float globalColor;

        public FlatEffectViewModel(SurfaceEffect dataModel) 
            : base(dataModel)
        {
        }

        public float GlobalColor
        {
            get => globalColor;
            set
            {
                globalColor = value;
                OnPropertyChanged(nameof(GlobalColor));
            }
        }
    }
}
