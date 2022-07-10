using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Aurora.Sample.Editor.Windows
{
    /// <summary>
    /// Interaction logic for SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : Window
    {
        public SettingsDialog()
        {            
            InitializeComponent();            
        }

        public SharpDX.Color SelectedViewportColor
        {
            get => ((ViewModel)DataContext).SelectedColor.Color;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            InitializeData();
        }

        private void InitializeData()
        {
            var viewModel = DataContext as ViewModel;
           
            string[] windowsBrushColors = typeof(Brushes).GetProperties(BindingFlags.Public | BindingFlags.Static)
                                                         .Where(p => p.PropertyType.IsAssignableFrom(typeof(SolidColorBrush)))
                                                         .Select(p => p.Name)
                                                         .ToArray();

            var directXColors = typeof(SharpDX.Color).GetFields(BindingFlags.Public | BindingFlags.Static)
                                                 .Where(p => p.FieldType.IsAssignableFrom(typeof(SharpDX.Color)))
                                                 .Where(p => windowsBrushColors.Contains(p.Name))
                                                 .Select(p => new
                                                 {
                                                     Color = (SharpDX.Color)p.GetValue(null),
                                                     Name = p.Name
                                                 })                                                  
                                                 .ToArray();

            viewModel.ViewportColors = new ObservableCollection<ViewportColor>();
            foreach(var directXColor in directXColors)
            {
                viewModel.ViewportColors.Add(new ViewportColor { WindowsBrushName = directXColor.Name, Color = directXColor.Color });
            }

            viewModel.SelectedColor = viewModel.ViewportColors.First();
        }
    }

    public class ViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ViewportColor> viewportColors;
        private ViewportColor selectedColor;

        public ObservableCollection<ViewportColor> ViewportColors
        {
            get => viewportColors;
            set
            {
                viewportColors = value;
                RaisePropertyChanged(nameof(ViewportColors));
            }
        }

        public ViewportColor SelectedColor
        {
            get => selectedColor;
            set
            {
                selectedColor = value;
                RaisePropertyChanged(nameof(SelectedColor));
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
