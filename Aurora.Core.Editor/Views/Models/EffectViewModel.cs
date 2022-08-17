using Aurora.Core.Editor.Util;
using CipherPark.Aurora.Core.Effects;

namespace Aurora.Core.Editor
{
    public class EffectViewModel : ViewModelBase<SurfaceEffect>
    {
        public EffectViewModel(SurfaceEffect dataModel)
            : base(dataModel)
        {           
        }
        
        public string Name
        {
            get => DataLookup.GetEffectName(DataModel);
        }       
    }
}
