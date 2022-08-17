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
                DataLookup.GetNameFromFilename(DataModel.As<GameObjectSceneNode>()?.GameObject.GetContext<GameObjectMeta>()?.ModelFileName);
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

        public float[] Matrix
        {
            get => DataModel.Transform.ToMatrix().ToArray();
            set
            {
                DataModel.Transform = new Transform(new Matrix(value));
                OnPropertyChanged(nameof(Matrix));
                OnPropertyChanged(nameof(Location));
            }
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

        public float[] Location
        {
            get => DataModel.Transform.Translation.ToArray();
        }

        public NodeType NodeType
        {
            get => DataLookup.GetNodeType(DataModel);
        }       

        public GameObjectViewModel GameObject
        {
            get;
        }
    }
}
