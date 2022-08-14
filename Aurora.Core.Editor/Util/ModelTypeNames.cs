namespace Aurora.Core.Editor.Util
{
    public static class ModelTypeNames
    {
        public const string Static = "Static Mesh";
        public const string Multi = "Multi Mesh";
        public const string Rigged = "Rigged Mesh";
        public const string Unknown = "Unknown";
        public static readonly string[] All =
            new string[]
            {
                Static,
                Multi,
                Rigged
            };
    }
}