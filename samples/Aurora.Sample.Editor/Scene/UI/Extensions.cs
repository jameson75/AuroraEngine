using CipherPark.Aurora.Core.UI.Controls;


namespace Aurora.Sample.Editor.Scene.UI
{
    public static class Extensions
    {
        public static string GetText(this ContentControl control)
        {
            return control.Content.As<TextContent>().Text;
        }

        public static void SetText(this ContentControl control, string text)
        {
            control.Content.As<TextContent>().Text = text;
        }
    }
}
