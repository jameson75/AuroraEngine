using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace CipherPark.AngelJacket.Core.Utils
{
    public class InputState
    {
        KeyboardStateWindow keyboardStateWindow = null;
        MouseStateWindow mouseStateWindow = null;
        Keys[] _releasedKeys = null;
        MouseButton[] _mouseButtonsReleased = null;
        MouseButton[] _mouseButtonsDown = null;
        MouseButton[] _mouseButtonsPressed = null;
        long _stateUpdateTime = 0;

        public long StateUpdateTime { get { return _stateUpdateTime; } }

        public void UpdateState()
        {
            _stateUpdateTime = Environment.TickCount;
            _releasedKeys = null;
            _mouseButtonsDown = null;
            _mouseButtonsReleased = null;

            //Update Old/New Snapshots
            //------------------------
            if (keyboardStateWindow == null)
            {
                keyboardStateWindow = new KeyboardStateWindow();
                keyboardStateWindow.OldState = Keyboard.GetState();
            }
            else
                keyboardStateWindow.OldState = keyboardStateWindow.NewState;
            keyboardStateWindow.NewState = Keyboard.GetState();
            
            //Update Key Press Time Stamps
            //----------------------------
            //Remove time stamps of keys which have just been released.
            foreach (Keys releasedKey in this.GetKeysReleased())
                keyboardStateWindow.PressTime.Remove(releasedKey);
            //Set time stamps for keys which have just been pressed.
            foreach (Keys newPressedKey in keyboardStateWindow.NewState.GetPressedKeys())
            {               
                if(!keyboardStateWindow.PressTime.ContainsKey(newPressedKey))
                    keyboardStateWindow.PressTime.Add(newPressedKey, _stateUpdateTime);                
            }

            //Update Old/New Snapshots
            //------------------------
            if (mouseStateWindow == null)
            {
                mouseStateWindow = new MouseStateWindow();
                mouseStateWindow.OldState = Mouse.GetState();
            }
            else
                mouseStateWindow.OldState = mouseStateWindow.NewState;
            mouseStateWindow.NewState = Mouse.GetState();            
            
            //Update Mouse Press Time Stamps
            //------------------------------
            //Remove time stamps of buttons which have just been released.
            foreach (MouseButton releasedButton in this.GetMouseButtonsReleased())
            {
                mouseStateWindow.PressTime.Remove(releasedButton);
                mouseStateWindow.PressLocation.Remove(releasedButton);
            }
            //Set time stamps for buttons which have just been pressed.
            foreach (MouseButton newPressedButtons in this.GetMouseButtonsDown())
            {
                if (!mouseStateWindow.PressTime.ContainsKey(newPressedButtons))
                    mouseStateWindow.PressTime.Add(newPressedButtons, _stateUpdateTime);
                if (!mouseStateWindow.PressLocation.ContainsKey(newPressedButtons))
                    mouseStateWindow.PressLocation.Add(newPressedButtons, new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            }
        }
        
        public bool IsKeyUp(Keys key)
        { 
             if(keyboardStateWindow == null)
                return false;
             else 
                 return keyboardStateWindow.NewState.IsKeyUp(key);
        }

        public bool IsKeyDown(Keys key)
        {
              if(keyboardStateWindow == null)
                  return false;
              else
                  return keyboardStateWindow.NewState.IsKeyDown(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            if(keyboardStateWindow == null)
                return false;
            else 
                return keyboardStateWindow.OldState.IsKeyDown(key) && keyboardStateWindow.NewState.IsKeyUp(key);
        }

        public Keys[] GetKeysDown()
        {
            if(keyboardStateWindow == null)
                return new Keys[0];
            else
                return keyboardStateWindow.NewState.GetPressedKeys();
        }

        public Keys[] GetKeysReleased()
        {
            if (keyboardStateWindow == null)
                return new Keys[0];
            else
            {
                if (_releasedKeys == null)
                {
                    List<Keys> _releasedKeyList = new List<Keys>();
                    foreach (Keys oldPressedKey in keyboardStateWindow.OldState.GetPressedKeys())
                        if (keyboardStateWindow.NewState.IsKeyUp(oldPressedKey))
                            _releasedKeyList.Add(oldPressedKey);
                    _releasedKeys = _releasedKeyList.ToArray();
                }
                return _releasedKeys;
            }
        }
      
        /// <summary>
        /// Returns the tick count at the point where UpdateState() was called the first time after the key was pressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long GetKeyPressTime(Keys key)
        {
            if (keyboardStateWindow == null)
                return 0;
            else if (!keyboardStateWindow.PressTime.ContainsKey(key))
                return 0;
            else
                return keyboardStateWindow.PressTime[key];                
        }
       
       /// <summary>
       /// Returns the elapsed time between the last time UpdateState() was called and the time returned by GetKeyPressTime()
       /// for the specified key.
       /// </summary>
       /// <param name="key"></param>
       /// <returns></returns>
        public TimeSpan GetKeyPressTimeSpan(Keys key)
        {
            return (IsKeyDown(key)) ? new TimeSpan(0, 0, 0, 0, (int)(_stateUpdateTime - GetKeyPressTime(key))) : TimeSpan.Zero;             
        }

        public bool IsMouseButtonUp(MouseButton button)
        {
            return !IsMouseButtonDown(button);
        }

        public bool IsMouseButtonDown(MouseButton button)
        {
            foreach (MouseButton b in GetMouseButtonsDown())
                if (b == button)
                    return true;
            return false;
        }

        public bool IsMouseButtonReleased(MouseButton button)
        {
            return (_GetMouseButtonsInState(mouseStateWindow.OldState, ButtonState.Pressed).Contains(button) &&
                    _GetMouseButtonsInState(mouseStateWindow.NewState, ButtonState.Released).Contains(button));
        }

        public MouseButton[] GetMouseButtonsReleased()
        {
            if (keyboardStateWindow == null)
                return new MouseButton[0];
            else
            {
                if (_mouseButtonsReleased == null)
                {
                    List<MouseButton> _mouseButtonsReleasedList = new List<MouseButton>();
                    foreach (MouseButton oldPressedMouseButton in InputState._GetMouseButtonsInState(mouseStateWindow.OldState, ButtonState.Pressed))
                        if (this.IsMouseButtonUp(oldPressedMouseButton))
                            _mouseButtonsReleasedList.Add(oldPressedMouseButton);
                    _mouseButtonsReleased = _mouseButtonsReleasedList.ToArray();
                }
                return _mouseButtonsReleased;
            }
        }     

        public MouseButton[] GetMouseButtonsDown()
        {
            if (keyboardStateWindow == null)
                return new MouseButton[0];
            else
            {
                if (_mouseButtonsDown == null)
                    _mouseButtonsDown = InputState._GetMouseButtonsInState(mouseStateWindow.NewState, ButtonState.Pressed);
                return _mouseButtonsDown;
            }
        }

        public MouseButton[] GetMouseButtonsPressed()
        {
            if (keyboardStateWindow == null)
                return new MouseButton[0];
            else
            {
                if (_mouseButtonsPressed != null)
                {
                    List<MouseButton> _mouseButtonsPressedList = new List<MouseButton>();
                    MouseButton[] oldMouseButtonsUp = InputState._GetMouseButtonsInState(mouseStateWindow.OldState, ButtonState.Released);
                    for (int i = 0; i < oldMouseButtonsUp.Length; i++ )
                        if (IsMouseButtonDown(oldMouseButtonsUp[i]))
                            _mouseButtonsPressedList.Add(oldMouseButtonsUp[i]);
                    _mouseButtonsPressed = _mouseButtonsPressedList.ToArray();
                }
                return _mouseButtonsPressed;
            }
        }

        public Point GetMouseLocation()
        {
            if (mouseStateWindow == null)
                return new Point(-1, -1);
            else
                return new Point(mouseStateWindow.NewState.X, mouseStateWindow.NewState.Y);
        }

        public Point GetPreviousMouseLocation()
        {
            if (mouseStateWindow == null)
                return new Point(-1, -1);
            else
                return new Point(mouseStateWindow.OldState.X, mouseStateWindow.OldState.Y);
        }

        public int GetMouseWheelDelta()
        {
            if (mouseStateWindow == null)
                return 0;
            else
                return mouseStateWindow.NewState.ScrollWheelValue - mouseStateWindow.OldState.ScrollWheelValue;
        }

        private static MouseButton[] _GetMouseButtonsInState(MouseState mouseState, ButtonState buttonState)
        {
            List<MouseButton> buttonsInState = new List<MouseButton>();
            if (mouseState.LeftButton == buttonState)
                buttonsInState.Add(MouseButton.Left);
            if (mouseState.RightButton == buttonState)
                buttonsInState.Add(MouseButton.Right);
            if (mouseState.MiddleButton == buttonState)
                buttonsInState.Add(MouseButton.Middle);
            return buttonsInState.ToArray();
        }      


        public class StateWindow<TState, TPressable>
        {       
            private Dictionary<TPressable, long> _pressTime = null;
            public Dictionary<TPressable, long> PressTime { get { return _pressTime; } }
            public StateWindow() 
            {
                _pressTime = new Dictionary<TPressable,long>();
            }    
            public TState OldState { get; set; }
            public TState NewState { get; set; }            
        }

        public class KeyboardStateWindow : StateWindow<KeyboardState, Keys> 
        { }

        public class MouseStateWindow : StateWindow<MouseState, MouseButton>
        {
            private Dictionary<MouseButton, Vector2> _pressLocation = null;
            public MouseStateWindow()
            {
                _pressLocation = new Dictionary<MouseButton, Vector2>();
            }
            public Dictionary<MouseButton, Vector2> PressLocation { get { return _pressLocation; } }
        }

        public enum MouseButton
        {
            Right,
            Left,
            Middle,
        }

        //public class KeyboardStateWindow
        //{
        //    private Dictionary<Keys, long> _keyPressTime = null;
        //    public KeyboardState OldState {get; set; }
        //    public KeyboardState NewState { get; set; }
        //    public Dictionary<Keys, long> KeyPressTime { get { return _keyPressTime; } }
        //    public KeyboardStateWindow() { _keyPressTime = new Dictionary<Keys,long>(); }
        //}               
    }
}
