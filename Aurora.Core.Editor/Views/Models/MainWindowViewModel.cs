namespace Aurora.Core.Editor
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ProjectViewModel project;        
        private SceneNodeViewModel selectedNode;
        private bool isProjectDirty;
        private string projectFileName;

        public LookupViewModel Lookup { get; } = new LookupViewModel();

        public ProjectViewModel Project
        {
            get => project;
            set
            {
                project = value;
                OnPropertyChanged(nameof(Project));                
            }
        }

        public bool IsProjectDirty
        {
            get => isProjectDirty;
            set
            {
                isProjectDirty = value;
                OnPropertyChanged(nameof(IsProjectDirty));                
            }
        }

        public string ProjectFileName
        {
            get => projectFileName;
            set
            {
                projectFileName = value;
                OnPropertyChanged(nameof(ProjectFileName));                
            }
        }

        public SceneNodeViewModel SelectedNode
        {
            get => selectedNode;
            set
            {
                selectedNode = value;
                OnPropertyChanged(nameof(SelectedNode));
            }
        }            
    }
}
