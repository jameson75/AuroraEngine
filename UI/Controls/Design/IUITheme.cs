using System;

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public interface IUITheme
    {
        ButtonTemplate Button { get; }
        CheckBoxTemplate CheckBox { get; }
        ContentControlTemplate ContentControl { get; }
        ImageControlTemplate ImageControl { get; }
        LabelTemplate Label { get; }
    }
}
