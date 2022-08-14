namespace Aurora.Core.Editor
{
    public class LookupViewModel
    {
        public string[] EffectNames
        {
            get => Environment.EffectNames.All;
        }
    }
}