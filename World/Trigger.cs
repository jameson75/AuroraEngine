using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherPark.AngelJacket.Core.World
{
    public abstract class Trigger
    {
        public TimeSpan Time { get; set; }
        //TODO: Make this property abstract.
        public TriggerType TriggerType { get; set; }
    }

    public enum TriggerType
    {
        ObjectState,
        Animation,
        FX,
        PlayerState,
        Audio
    }

    public class ObjectStateInfo
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ObjectStateInfo()
        { }
        public ObjectStateInfo(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }

    public class ObjectStateTrigger : Trigger
    {
        private List<ObjectStateInfo> _targetPropertyChanges = new List<ObjectStateInfo>();
        public List<ObjectStateInfo> TargetPropertyChanges
        {
            get { return _targetPropertyChanges; }
        }
        public object Target { get; set; }
    }
  

    public enum AnimationState
    {
        Start,
        Stop
    }

    public class AnimationTrigger : Trigger
    {
        public AnimationState AnimationState { get; set; }
        public Animation Animation { get; set; }
    }

    public enum FXType
    {
        LaserEtch,
        Morph,
        Composite,
        Fill
    }

    public class FXTrigger : Trigger
    {
        public FXType FXType { get; set; }
        public ISceneObject Target { get; set; }
        public FXSystemParameters Parameters { get; set; }
    }

    public enum PlayerStateChange
    {
        Avatar,
        Control
    }

    public class PlayerStateTrigger : Trigger
    {
        public PlayerStateChange StateChange { get; set; }
        public int PlayerIndex { get; set; }
        public ISceneObject PlayerAvatar { get; set; }
        public bool PlayerHasControl { get; set; }
    }

    public enum AudioTriggerAction
    {
        Play,
        Stop,
        Pause,
        Seek
    }

    public class AudioTrigger : Trigger
    {
        public AudioTriggerAction Action { get; set; }
    }

 
}
