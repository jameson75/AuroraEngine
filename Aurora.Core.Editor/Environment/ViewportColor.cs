using System.ComponentModel;

namespace Aurora.Sample.Editor.Windows
{
    public class ViewportColor : INotifyPropertyChanged
    {
        private string windowsBrushName;
        private SharpDX.Color color;

        public SharpDX.Color Color
        {
            get => color;
            set
            {
                color = value;
                RaisePropertyChanged(nameof(Color));
            }
        }

        public string WindowsBrushName
        {
            get => windowsBrushName;
            set
            {
                windowsBrushName = value;
                RaisePropertyChanged(nameof(WindowsBrushName));
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
