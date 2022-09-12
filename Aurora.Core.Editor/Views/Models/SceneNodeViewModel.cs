using Aurora.Core.Editor.Environment;
using Aurora.Core.Editor.Util;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Services;
using CipherPark.Aurora.Core.World.Scene;
using SharpDX;

namespace Aurora.Core.Editor
{
    public class SceneNodeViewModel : ViewModelBase<SceneNode>
    {
        public SceneNodeViewModel(SceneNode dataModel)
            : base(dataModel)
        {
            if (dataModel is GameObjectSceneNode)
            {
                GameObject = new GameObjectViewModel(dataModel.As<GameObjectSceneNode>().GameObject);
            }
        }
        
        public string Name
        {
            get => DataModel.Name ?? 
                DataMap.GetModelDisplayNameFromFilename(DataModel.As<GameObjectSceneNode>()?.GameObject.GetContext<GameObjectMeta>()?.ResourceFilename);
            set
            {
                DataModel.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public bool HasExplicitName
        {
            get => !string.IsNullOrWhiteSpace(DataModel.Name);
        }        

        public ulong Flags
        {
            get => DataModel.Flags;
            set
            {
                DataModel.Flags = value;
                OnPropertyChanged(nameof(Flags));
            }
        }       

        public bool Visible 
        {
            get => DataModel.Visible;
            set
            {
                DataModel.Visible = value;
                OnPropertyChanged(nameof(Visible));
            }
        }

        public Vector3 Position
        {
            get => DataModel.Transform.Translation;
            set
            {
                DataModel.Transform = new Transform(DataModel.Transform.Rotation, value);
                OnPropertyChanged(nameof(Position));
            }
        }

        public Quaternion Orientation
        {
            get => DataModel.Transform.Rotation;
            set
            {
                DataModel.Transform = new Transform(value, DataModel.Transform.Translation);
                OnPropertyChanged(nameof(Orientation));
            }
        }

        public Dom.NodeType NodeType
        {
            get => DataMap.GetDomNodeType(DataModel);
        }       

        public GameObjectViewModel GameObject
        {
            get;
        }

        public void NotifyTransform()
        {
            OnPropertyChanged(nameof(Position));
            OnPropertyChanged(nameof(Orientation));
        }
    }
}
