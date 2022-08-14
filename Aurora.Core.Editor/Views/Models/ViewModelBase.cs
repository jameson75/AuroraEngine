using System.ComponentModel;

namespace Aurora.Core.Editor
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));            
        }

        public event PropertyChangedEventHandler PropertyChanged;        
    }

    public class ViewModelBase<T> : ViewModelBase
    {
        public ViewModelBase(T dataModel)
        {
            DataModel = dataModel;
        }

        public T DataModel { get; }
    }
}
