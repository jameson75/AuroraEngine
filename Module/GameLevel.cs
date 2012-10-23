using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.World;
using CipherPark.AngelJacket.Core.Services;
using CipherPark.AngelJacket.Core.Utils;

using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Multimedia;
using SharpDX.DirectInput;

public enum RunningState
{
    Stopped,
    Started,
    Paused
}

namespace CipherPark.AngelJacket.Core.Module
{
    public class GameLevel : GameModule
    {
        public Scene _scene = null;
        public List<Trigger> _triggers = new List<Trigger>();
        public Queue<Trigger> _triggerQueue = new Queue<Trigger>();
        public Scene _levelScene = null;
        //public SoundEffect _bgMusic = null;
        //public SoundEffectInstance _bgMusicInstance = null;
        //public List<SoundEffect> _audioEffects = new List<SoundEffect>();
        //public Dictionary<string, SoundEffectInstance> _audioEffectInstances = new Dictionary<string, SoundEffectInstance>();
        public MasteringVoice __bgMusic = null;
        public SourceVoice __bgMusicInstance = null;
        public List<MasteringVoice> __audioEffects = new List<MasteringVoice>();
        public Dictionary<string, SourceVoice> __audioEffectInstances = new Dictionary<string, SourceVoice>();
        public XAudio2 __audioDevice = null;
        private RunningState _levelRunningState = RunningState.Stopped;
        private int _levelStartTime = 0;
        private AnimationSystem AnimationSystem = null;
        private FXSystem FXSystem = null;
        private UITree _uiTree = null;
        private bool _isInitialized = false;
        private bool _isLoaded = false;

        public GameLevel(IGameApp game) : base(game)
        {
            _scene = new Scene(game);
            this.AnimationSystem = new AnimationSystem(game);
            _uiTree = new UITree(game);
        }

        public bool IsLevelStarted
        {
            get { return _levelRunningState == RunningState.Started || _levelRunningState == RunningState.Paused; }
        }

        public void Start()
        {
            if( IsLevelStarted )
                throw new InvalidOperationException("Level already started.");

            _levelStartTime = Environment.TickCount;
        }

        public void SetSceneObjectProperty(object so, string propertyName, object propertyValue)
        {
        
        //    //NOTE: WE DON'T WANT TO RELY ON REFLECTION (TOO SLOW FOR GAME LOOP), SO WE HARD CODE...

        //    //Handle SceneObject properties...
        //    switch(propertyName)
        //    {
        //        case "Visible":
        //            so.Visible = Convert.ToBoolean(propertyValue);
        //            break;
        //        case "Transform":
        //            so.Transform = (Matrix)propertyValue;
        //            break;
        //    }

        //    //Handle CameraObject properties...
        //    if( so is CameraObject )
        //    {
        //        CamerObject co = (CameraObject)so;
        //        switch(propertyName)
        //        {
        //            case "EyeTarget":
        //                co.EyeTarget = (SceneObject)propertyValue;
        //                break;
        //            case "LockEyeTarget":
        //                co.LockEyeTarget = (bool)propertyValue;
        //                break;
        //        }
        //    }

        //    //Handle FlowStream object properties...
        //    if( so is FlowStreamObject )
        //    {
        //        FlowStreamObject fso = (FlowStreamObject)so;
        //        switch(propertyName)
        //        {
        //            case "Direction":
        //                fso.Direction = (FlowStreamDirection)propertyValue;
        //                break;
        //        }
        //    }

            //CipherPark.AngelJacket.ContentPipeline.Content.LevelConfigurationContent;
        }    

        private SceneNodes LoadScene(GameLevelConfiguration config)
        {
            SceneNodes sceneNodes = new SceneNodes();
            
            return sceneNodes;
        }
        
        private GameLevelConfiguration LoadConfig(string path, string levelConfigNodeName )
        {
            GameLevelConfiguration config = new GameLevelConfiguration();
            return config;
        }

        public override void LoadContent()
        {
            IGlobalGameStateService globalGameStateService = (IGlobalGameStateService)Game.Services.GetService(typeof(IGlobalGameStateService));
            if (globalGameStateService == null)
                throw new InvalidOperationException("GlobalGameStateServices not provided.");  

            //LOAD CONTENT
            //------------            
            //I. Load level 1 config.
            GameLevelConfiguration config = LoadConfig("GameConfig.xml", "GameLevel1");
            
            //II. Load global (game-wide) content.
            //NOTE: We do this here, at the level-scope, to ensure we reset any overrides created from the previously loaded level.
            SceneNodes playerNodes = LoadScene(config );
            _levelScene.Nodes.AddRange(playerNodes);
            for(int i = 0; i < playerNodes.Count; i++ )
                if( globalGameStateService.State.Players[i].Active )
                {
                    globalGameStateService.State.Players[i].Avatar = playerNodes[i].SceneObject;
                }            
        
            //III. Load level 1.
            //1. Load music
            //_bgMusic = Game.Content.Load<SoundEffect>(config.BackgroundMusicPath);
            //_bgMusicInstance = _bgMusic.CreateInstance();
           
            CipherPark.AngelJacket.Core.Utils.Interop.VoiceData vd = CipherPark.AngelJacket.Core.Utils.Interop.ContentImporter.LoadVoiceDataFromWav(config.BackgroundMusicPath);
            WaveFormat sourceVoiceFormat = WaveFormat.CreateCustomFormat((WaveFormatEncoding)vd.Format.FormatTag, (int)vd.Format.SamplesPerSec, (int)vd.Format.Channels, (int)vd.Format.AvgBytesPerSec, (int)vd.Format.BlockAlign, (int)vd.Format.BitsPerSample);
            __audioDevice = new XAudio2();
            __bgMusic = new MasteringVoice(__audioDevice);
            __bgMusicInstance = new SourceVoice(__audioDevice, sourceVoiceFormat);
            AudioBuffer ab = new AudioBuffer();
            ab.AudioBytes = vd.AudioBytes;
            ab.Flags = BufferFlags.EndOfStream;
            ab.Stream = new DataStream(vd.AudioBytes, true, false);
            vd.AudioData.CopyTo(ab.Stream);
            __bgMusicInstance.SubmitSourceBuffer(ab, null);

            //2. Load level scene content
            SceneNodes levelNodes = LoadScene(config);
            _scene.Nodes.AddRange(levelNodes);

            ////3. Override global game setup states.       
            //for( int i = 0; i < config.GlobalOverride.Players.Length; i++ )
            //{
            //    if( globalGameStateService.State.Players[i].Active )
            //    {
            //        //override default visual object.
            //        if( config.GlobalOverride.Players[i].VisualObjectName != null )              
            //            globalGameStateService.State.Players[i].VisualObject = _levelSceneNodes[config.GlobalOverride.Players[i].VisualObjectName];
                
            //        //override default visibility.
            //        if( config.GlobalOverride.Players[i].Visible.HasValue )
            //            globalGameStateService.State.Players[i].VisualObject.Visible = config.GlobalOverride.Players[i].Visible.Value;
                
            //        //override default player-control.
            //        if( config.GlobalOverride.Players[i].HasControl.HasValue)
            //            globalGameStateService.State.Players[i].HasControl = config.GlobalOverride.Players[i].HasControl.Value;
            //    }
            //}

            //LOAD TRIGGERS
            //-------------        

            _isLoaded = true;
        }

        public override void Update(long gameTime)
        {
            IInputService inputServices = (IInputService)Game.Services.GetService(typeof(IInputService));
            if (inputServices == null)
                throw new InvalidOperationException("InputServices not provided.");

            InputState inputStateManager = inputServices.GetInputState();

            //The ESCAPE key on the key board Exits this module.
            if (inputStateManager.IsKeyReleased(Key.Escape))
                this.SignalExitModule();
            else
            {
                if( !IsLevelStarted )
                    Start();
            }           
      
            //Execute Trigger sytem...
            //------------------------
            TimeSpan elapsedLevelTime = new TimeSpan(0, 0, 0, 0, Environment.TickCount - _levelStartTime);
            while( _triggerQueue.Peek().Time <= elapsedLevelTime )
            {
                Trigger trigger = _triggerQueue.Dequeue();
                OnProcessTrigger(trigger);
            }

            //Execute Physics system...
            //-------------------------

        
            //Execute UI system...
            //--------------------        

        }

        public virtual void OnProcessTrigger(Trigger trigger)
        {
            switch( trigger.TriggerType )
            {
                case TriggerType.ObjectState:
                    ObjectStateTrigger soTrigger = (ObjectStateTrigger)trigger;
                    for( int i = 0; i < soTrigger.TargetPropertyChanges.Count; i++ )
                        SetSceneObjectProperty(soTrigger.Target, soTrigger.TargetPropertyChanges[i].Name, soTrigger.TargetPropertyChanges[i].Value);
                        //so.Target.SetPropertyByName(soTrigger.TargetPropertyChagnes[i].Name, soTrigger.TargetPropertyChanges[i].Value);
                    break;

                case TriggerType.Animation:
                    AnimationTrigger aTrigger = (AnimationTrigger)trigger;                
                    if( aTrigger.AnimationState == AnimationState.Start )
                        AnimationSystem.Start(aTrigger.Animation);                
                    else if( aTrigger.AnimationState == AnimationState.Stop )
                        AnimationSystem.Stop(aTrigger.Animation);
                    break;

                case TriggerType.FX:
                    FXTrigger fxTrigger = (FXTrigger)trigger;
                    if( fxTrigger.FXType == FXType.LaserEtch )
                        FXSystem.LaserEtch(fxTrigger.Target, (LaserEtchParameters)fxTrigger.Parameters);
                    else if(fxTrigger.FXType == FXType.Morph)
                        FXSystem.Morph(fxTrigger.Target, (MorphParameters)fxTrigger.Parameters);
                    else if(fxTrigger.FXType == FXType.Composite)
                        FXSystem.Composite(fxTrigger.Target, (CompositeParameters)fxTrigger.Parameters);
                    else if(fxTrigger.FXType == FXType.Fill)
                        FXSystem.Fill(fxTrigger.Target, (FillParameters)fxTrigger.Parameters);                
                    break;

                case TriggerType.PlayerState:
                    PlayerStateTrigger psTrigger = (PlayerStateTrigger)trigger;
                    IGlobalGameStateService globalGameStateService = (IGlobalGameStateService)Game.Services.GetService(typeof(IGlobalGameStateService));
                    if (globalGameStateService == null)
                        throw new InvalidOperationException("GlobalGameStateServices not provided.");    
                    if( psTrigger.StateChange == PlayerStateChange.Avatar )
                        globalGameStateService.State.Players[psTrigger.PlayerIndex].Avatar = psTrigger.PlayerAvatar;
                    else if( psTrigger.StateChange == PlayerStateChange.Control )
                        globalGameStateService.State.Players[psTrigger.PlayerIndex].HasControl = psTrigger.PlayerHasControl;
                    break;          
      
                case TriggerType.Audio:
                    AudioTrigger bmTrigger = (AudioTrigger)trigger;
                    switch(bmTrigger.Action)
                    {
                        case AudioTriggerAction.Play:
                            //_bgMusicInstance.Play();
                            __bgMusicInstance.Start();
                            break;
                        case AudioTriggerAction.Stop:
                            //_bgMusicInstance.Stop();
                            __bgMusicInstance.Stop();
                            break;
                    }
                    break;
            }
        }

        public void LoadHardLevel()
        {
            //ObjectStateTrigger overlayStateTrigger = new ObjectStateTrigger();
            //ScreenOverlay overlay = new ScreenOverlay(this.Game);
            //overlay.Name = "Main_Overlay";
            //overlay.Position = new Vector2(viewService.DisplayArea.Right, viewService.DisplayArea.Top); //TODO: Figure out offscreen positions.
            //overlay.Visible = false;
            //this._uiTree.Controls.Add(overlay);

            //PropertyAnimation slideInAnimation = new PropertyAnimation();
            //slideInAnimation.Frames.Add(new Vector2PropertyAnimationFrame(0, overlay.Position));
            //slideInAnimation.Frames.Add(new Vector2PropertyAniamtionFrame(5, new Vector2(overlay.Position, viewService.DisplayArea.Y)));

            //overlayStateTrigger.Time = new TimeSpan(0, 0, 15);
            //overlayStateTrigger.Target = _uiTree.Controls["Main_Overlay"];
            //overlayStateTrigger.TargetPropertyChanges.Add(new ObjectStateInfo("Visible", true));

            //AnimationTrigger overlayAnimationTrigger = new AnimationTrigger();
            //overlayAnimationTrigger.AnimationState = AnimationState.Start; //TODO: Do I really need this??
            //overlayAnimationTrigger.Animation = slideInAnimation;

            //_triggerQueue.Enqueue(overlayStateTrigger);
            //_triggerQueue.Enqueue(overlayAnimationTrigger);
        }

        public override void Draw(long gameTime)
        {
            
        }

        public override void Initialize()
        {
            _isInitialized = true;    
        }

      

        public override bool IsInitialized
        {
            get { return _isInitialized; }
        }

        public override bool IsLoaded
        {
            get { return _isLoaded; }
        }

        public override void Uninitialize()
        {
            
        }

        public override void UnloadContent()
        {
            
        }
    }
}
