using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Aurora.Core.Editor;

namespace Aurora.Sample.Editor
{
    public partial class MainWindow
    {
        public class SelectedNodeTwoWayBinding
        {            
            private readonly TreeView treeView;
            private readonly MainWindowViewModel mainWindowViewModel;
            private ProjectViewModel projectViewModel;
            private SceneViewModel sceneViewModel;
            private bool updatingViewModel;           

            public SelectedNodeTwoWayBinding(TreeView treeView, MainWindowViewModel mainWindowViewModel)
            {
                this.treeView = treeView;
                this.mainWindowViewModel = mainWindowViewModel;
                mainWindowViewModel.PropertyChanged += MainViewModel_PropertyChanged;              
                treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
                UpdateListenersForProjectViewModel();
            }

            private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(MainWindowViewModel.Project))
                {
                    UpdateListenersForProjectViewModel();
                }
            }

            private void ProjectViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(ProjectViewModel.Scene))
                {
                    UpdateListenersForSceneViewModel();
                }
            }

            private void SceneViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(SceneViewModel.SelectedNode))
                {
                    if (!updatingViewModel)
                    {
                        if (sceneViewModel.SelectedNode == null)
                        {
                            if (treeView.SelectedItem != null)
                            {
                                treeView.ItemContainerGenerator.ContainerFromItem(treeView.SelectedItem);
                            }
                        }
                        else
                        {
                            TreeViewItem tvi = treeView.ItemContainerGenerator.ContainerFromItem(sceneViewModel.SelectedNode) as TreeViewItem;
                            if (tvi != null)
                            {
                                tvi.IsSelected = true;
                            }
                        }
                    }
                }
            }

            private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
            {
                updatingViewModel = true;
                sceneViewModel.SelectedNode = (SceneNodeViewModel)e.NewValue;
                updatingViewModel = false;
            }

            private void UpdateListenersForProjectViewModel()
            {
                if (projectViewModel != null)
                {
                    projectViewModel.PropertyChanged -= ProjectViewModel_PropertyChanged;
                    projectViewModel = null;
                }

                if (mainWindowViewModel.Project != null)
                {
                    projectViewModel = mainWindowViewModel.Project;
                    projectViewModel.PropertyChanged += ProjectViewModel_PropertyChanged;
                }
                
                UpdateListenersForSceneViewModel();
            }

            private void UpdateListenersForSceneViewModel()
            {
                if (sceneViewModel != null)
                {
                    sceneViewModel.PropertyChanged -= SceneViewModel_PropertyChanged;
                    sceneViewModel = null;
                }

                if (mainWindowViewModel.Project?.Scene != null)
                {
                    sceneViewModel = mainWindowViewModel.Project.Scene;
                    sceneViewModel.PropertyChanged += SceneViewModel_PropertyChanged;
                }
            }
        }
    }
}
