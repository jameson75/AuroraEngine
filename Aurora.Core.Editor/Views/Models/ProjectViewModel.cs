namespace Aurora.Core.Editor
{
    public class ProjectViewModel : ViewModelBase<IEditorGameApp>
    {
        public ProjectViewModel(IEditorGameApp dataModel) 
            : base(dataModel)
        {
            Scene = new SceneViewModel(dataModel.Scene);
        }

        public SceneViewModel Scene
        {
            get;
        }       
    }
}
