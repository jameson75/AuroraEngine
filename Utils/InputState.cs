﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.DirectInput;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Utils
{
    public class InputState
    {
        KeyboardStateWindow keyboardStateWindow = null;
        MouseStateWindow mouseStateWindow = null;
        Key[] _releasedKeys = null;
        MouseButton[] _mouseButtonsReleased = null;
        MouseButton[] _mouseButtonsDown = null;
        MouseButton[] _mouseButtonsPressed = null;
        long _stateUpdateTime = 0;
        IGameApp _game = null;
        HwndMessageHook _messageHook = null;

        public InputState(IGameApp game)
        {
            _game = game;
        }

        public long StateUpdateTime { get { return _stateUpdateTime; } }

       
        public void UpdateState()
        {
            _stateUpdateTime = Environment.TickCount;
            _releasedKeys = null;
            _mouseButtonsDown = null;
            _mouseButtonsReleased = null;
            _mouseButtonsPressed = null;

            if (_messageHook == null)
            {
                _messageHook = new HwndMessageHook();
                _messageHook.Initialize(_game.DeviceHwnd);
            }

            //Update Old/New Snapshots
            //------------------------
            if (keyboardStateWindow == null)
            {
                keyboardStateWindow = new KeyboardStateWindow();
                keyboardStateWindow.OldState = _game.Keyboard.GetCurrentState();
            }
            else
                keyboardStateWindow.OldState = keyboardStateWindow.NewState;
            keyboardStateWindow.NewState = _game.Keyboard.GetCurrentState();
            
            //Update Key Press Time Stamps
            //----------------------------
            //Remove time stamps of keys which have just been released.
            foreach (Key releasedKey in this.GetKeysReleased())
                keyboardStateWindow.PressTime.Remove(releasedKey);
            //Set time stamps for keys which have just been pressed.
            foreach (Key newPressedKey in keyboardStateWindow.NewState.PressedKeys)
            {               
                if(!keyboardStateWindow.PressTime.ContainsKey(newPressedKey))
                    keyboardStateWindow.PressTime.Add(newPressedKey, _stateUpdateTime);                
            }

            //Update Old/New Snapshots
            //------------------------
            if (mouseStateWindow == null)
            {
                mouseStateWindow = new MouseStateWindow();
                mouseStateWindow.OldState = _game.Mouse.GetCurrentState();
                SetMouseCurrentPosition(mouseStateWindow.OldState);
            }
            else
                mouseStateWindow.OldState = mouseStateWindow.NewState;
            mouseStateWindow.NewState = _game.Mouse.GetCurrentState();
            SetMouseCurrentPosition(mouseStateWindow.NewState);          
          
            
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
                    mouseStateWindow.PressLocation.Add(newPressedButtons, new Vector2(_game.Mouse.GetCurrentState().X, _game.Mouse.GetCurrentState().Y));
            }
        }
        
        public bool IsKeyUp(Key key)
        { 
             if(keyboardStateWindow == null)
                return false;
             else 
                 return keyboardStateWindow.NewState.IsKeyUp(key);
        }

        public bool IsKeyDown(Key key)
        {
              if(keyboardStateWindow == null)
                  return false;
              else
                  return keyboardStateWindow.NewState.IsKeyDown(key);
        }

        public bool IsKeyReleased(Key key)
        {
            if(keyboardStateWindow == null)
                return false;
            else 
                return keyboardStateWindow.OldState.IsKeyDown(key) && keyboardStateWindow.NewState.IsKeyUp(key);
        }

        public Key[] GetKeysDown()
        {
            if(keyboardStateWindow == null)
                return new Key[0];
            else
                return keyboardStateWindow.NewState.PressedKeys.ToArray();
        }

        public Key[] GetKeysReleased()
        {
            if (keyboardStateWindow == null)
                return new Key[0];
            else
            {
                if (_releasedKeys == null)
                {
                    List<Key> _releasedKeyList = new List<Key>();
                    foreach (Key oldPressedKey in keyboardStateWindow.OldState.PressedKeys)
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
        public long GetKeyPressTime(Key key)
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
        public TimeSpan GetKeyPressTimeSpan(Key key)
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
                if (_mouseButtonsPressed == null)
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

        public DrawingPoint GetMouseLocation()
        {
            if (mouseStateWindow == null)
                return new DrawingPoint(-1, -1);
            else
                return new DrawingPoint(mouseStateWindow.NewState.X, mouseStateWindow.NewState.Y);
        }

        public DrawingPoint GetPreviousMouseLocation()
        {
            if (mouseStateWindow == null)
                return new DrawingPoint(-1, -1);
            else
                return new DrawingPoint(mouseStateWindow.OldState.X, mouseStateWindow.OldState.Y);
        }

        public int GetMouseWheelDelta()
        {
            if (mouseStateWindow == null)
                return 0;
            else
                return mouseStateWindow.NewState.Z - mouseStateWindow.OldState.Z;
        }

        private static MouseButton[] _GetMouseButtonsInState(MouseState mouseState, ButtonState buttonState)
        {
            List<MouseButton> buttonsInState = new List<MouseButton>();
            if (mouseState.LeftButton() == buttonState)
                buttonsInState.Add(MouseButton.Left);
            if (mouseState.RightButton() == buttonState)
                buttonsInState.Add(MouseButton.Right);
            if (mouseState.MiddleButton() == buttonState)
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

        public class KeyboardStateWindow : StateWindow<KeyboardState, Key> 
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

        private void SetMouseCurrentPosition(MouseState state)
        {
            DrawingPoint mousePos = this.GetMousePosition();
            state.X = mousePos.X;
            state.Y = mousePos.Y;
            state.Z = _messageHook.LastMouseWheelDelta;
            _messageHook.LastMouseWheelDelta = 0;
        }
        
        public DrawingPoint GetMousePosition()
        {         
            UnsafeNativeMethods.WIN32_POINT point;
            if (!UnsafeNativeMethods.GetCursorPos(out point))
                throw new InvalidOperationException("Failed retreiving win32 mouse coordinates.");

            if(!UnsafeNativeMethods.ScreenToClient(this._game.DeviceHwnd, ref point))
                throw new InvalidOperationException("Failed converting mouse position to client coordinates.");

            return new DrawingPoint(point.X, point.Y);            
        }

        
        private static class UnsafeNativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct WIN32_POINT
            {
                public int X;
                public int Y;
            }

            [DllImport("user32.dll")]
            public static extern bool GetCursorPos(out WIN32_POINT point);

            [DllImport("user32.dll")]
            public static extern bool ScreenToClient(IntPtr hWnd, ref WIN32_POINT point);
        }

        private class HwndMessageHook
        {
            private const int WM_MOUSEWHEEL = 0x020A;
            private delegate IntPtr WndProcDelegate(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
            private WndProcDelegate _hookWndProcDelegate = null;
            private Delegate _originalWndProcDelegate = null;
            public void Initialize(IntPtr hWnd)
            {
                _hookWndProcDelegate = new WndProcDelegate(WndProc);             
                IntPtr hookWndProcFuncPtr = Marshal.GetFunctionPointerForDelegate(_hookWndProcDelegate);
                IntPtr originalWndProcFuncPtr = (UnsafeNativeMethods.SetWindowLong(hWnd, UnsafeNativeMethods.GWL_WNDPROC, hookWndProcFuncPtr));
                if (originalWndProcFuncPtr != IntPtr.Zero)
                    _originalWndProcDelegate = Marshal.GetDelegateForFunctionPointer(originalWndProcFuncPtr, typeof(WndProcDelegate));
                
            }

            private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
            {
                //OnWndProc(hWnd, msg, wParam, lParam);
                switch (msg)
                {
                    case WM_MOUSEWHEEL:
                        LastMouseWheelDelta = ((short)((int)wParam >> 16));
                        break;
                }
                return (IntPtr)_originalWndProcDelegate.DynamicInvoke(hWnd, msg, wParam, lParam);
            }

            private static class UnsafeNativeMethods
            {
                public const int GWL_WNDPROC = -4;

                [DllImport("user32.dll")]
                public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

                [DllImport("user32.dll")]
                public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

                [DllImport("user32.dll")]
                public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
            }

            public int LastMouseWheelDelta { get; set; }
        }
    }

    public enum ButtonState
    {
        Pressed,
        Released,
    }

    /*
    public enum VirtualKey
    {
        Unknown = 0,
        BackSpace = 8,
        Tab = 9,
        Enter = 13,
        RightShift = 16,
        LeftShift = RightShift,
        RightControl = 17,
        LeftControl = RightControl,
        RightAlt = 18,
        LeftAlt = RightAlt,
        Pause = 19,
        CapsLock = 20,
        Escape = 27,
        Space = 32,
        PageUp = 33,
        PageDown = 34,
        End = 35,
        Home = 36,
        Left = 37,
        Up = 38,
        Right = 39,
        Down = 40,
        PrintScreen = 44,
        Insert = 45,
        Delete = 46,
        D0 = 48,
        D1 = 49,
        D2 = 50,
        D3 = 51,
        D4 = 52,
        D5 = 53,
        D6 = 54,
        D7 = 55,
        D8 = 56,
        D9 = 57,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        LWin = 91,
        RWin = 92,
        Apps = 93,
        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        Multiply = 106,
        Add = 107,
        Subtract = 109,
        Decimal = 110,
        Divide = 111,
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,
        F13 = 124,
        F14 = 125,
        F15 = 126,
        F16 = 127,
        NumLock = 144,
        ScrollLock = 145,
        LShift = 160,
        RShift = 161,
        LControl = 162,
        RControl = 163,
        LAlt = 164,
        RAlt = 165,
        Semicolon = 186,
        Equals = 187,
        Comma = 188,
        Underscore = 189,
        Period = 190,
        ForwardSlash = 191,
        Tilde = 192,
        BackSlash = 220,
        RightBrace = 221,
        LeftBrace = 219,
        Apostrophe = 222
    }
    */
   /*
    public static class DirectInputVKMap
    {
        private static VirtualKey[] map = new VirtualKey[] {
            VirtualKey.Unknown,
            VirtualKey.Escape,
            VirtualKey.D1,
            VirtualKey.D2,
            VirtualKey.D3,
            VirtualKey.D4,
            VirtualKey.D5,
            VirtualKey.D6,
            VirtualKey.D7,
            VirtualKey.D8,
            VirtualKey.D9,
            VirtualKey.D0,
            VirtualKey.Underscore,
            VirtualKey.Equals,
            VirtualKey.BackSpace,
            VirtualKey.Tab,
            VirtualKey.Q,
            VirtualKey.W,
            VirtualKey.E,
            VirtualKey.R,
            VirtualKey.T,
            VirtualKey.Y,
            VirtualKey.U,
            VirtualKey.I,
            VirtualKey.O,
            VirtualKey.P,
            VirtualKey.LeftBrace,
            VirtualKey.RightBrace,
            VirtualKey.Enter,
            VirtualKey.LeftControl,
            VirtualKey.A,
            VirtualKey.S, 
            VirtualKey.D,
            VirtualKey.F,
            VirtualKey.G,
            VirtualKey.H,
            VirtualKey.J,
            VirtualKey.K,
            VirtualKey.L,
            VirtualKey.Semicolon,
            VirtualKey.Apostrophe,
            VirtualKey.Tilde,
            VirtualKey.LeftShift,
            VirtualKey.BackSlash,
            VirtualKey.Z,
            VirtualKey.X,
            VirtualKey.V,
            VirtualKey.B,
            VirtualKey.N,
            VirtualKey.M,
            VirtualKey.Comma,
            VirtualKey.Period,
            VirtualKey.ForwardSlash,
            VirtualKey.RightShift,
            VirtualKey.Multiply,
            VirtualKey.LeftAlt,
            VirtualKey.Space,
            VirtualKey.CapsLock,
            VirtualKey.F1,
            VirtualKey.F2,
            VirtualKey.F3,
            VirtualKey.F4,
            VirtualKey.F5,
            VirtualKey.F6,
            VirtualKey.F7,
            VirtualKey.F8,
            VirtualKey.F9,
            VirtualKey.F10,
            VirtualKey.NumLock,
            VirtualKey.ScrollLock,
            VirtualKey.NumPad7,
            VirtualKey.NumPad8,
            VirtualKey.NumPad9,
            VirtualKey.Subtract,
            VirtualKey.NumPad4,
            VirtualKey.NumPad5,
            VirtualKey.NumPad6,
            VirtualKey.Add,
            VirtualKey.NumPad1,
            VirtualKey.NumPad2,
            VirtualKey.NumPad3,
            VirtualKey.NumPad0,
            VirtualKey.Decimal,
            VirtualKey.F11,
            VirtualKey.F12,
            VirtualKey.Unknown, //DIK_F13
            VirtualKey.Unknown, //DIK_F14
            VirtualKey.Unknown, //DIK_F15
            VirtualKey.Unknown, //DIK_KANA (Japenese Keyboard)
            VirtualKey.Unknown, //KID_CONVERT (Japenese Keyboard)
            VirtualKey.Unknown, //DIK_NOCONVERT (Japenese Keyboard)
            VirtualKey.Unknown, //DIK_YEN (Japenese Keyboard)
            VirtualKey.Unknown, //DIK_NAMPADEQUALS ( = on numeric keypad)
            VirtualKey.Unknown, //DIK_CIRCUMFLEX
            VirtualKey.Unknown, //DIK_AT
            VirtualKey.Unknown, //DIK_COLON
            VirtualKey.Unknown, //DIK_UNDERLINE
            VirtualKey.Unknown, //DIK_KANJI
            VirtualKey.Unknown, //DIK_STOP
            VirtualKey.Unknown, //DIK_AX
            VirtualKey.Unknown, //DIK_UNLABELED
            VirtualKey.Unknown, //DIK_NUMPADENTER
            VirtualKey.Unknown, //DIK_RCONTROL
            VirtualKey.Unknown, //DIK_NUMPADCOMMA
            VirtualKey.Divide, 
            VirtualKey.Unknown,
            VirtualKey.RightAlt,
            VirtualKey.Home,
            VirtualKey.Up,
            VirtualKey.PageUp,
            VirtualKey.Left,
            VirtualKey.RightAlt,
            VirtualKey.End,
            VirtualKey.Down,
            VirtualKey.PageDown,
            VirtualKey.Insert,
            VirtualKey.Delete,
            VirtualKey.LWin,
            VirtualKey.RWin,
            VirtualKey.Apps
        };

        public static VirtualKey ToVirtualKey(Key dik)
        {
            return map[(int)dik];
        }

        public static VirtualKey[] ToVirtualKeys( IEnumerable<Key> diks)
        {
            VirtualKey[] vkeys = new VirtualKey[diks.Count()];
            int i = 0;
            foreach (Key dik in diks)
            {
                vkeys[i] = map[(int)dik];
                i++;
            }
            return vkeys;
        }

        public static Key ToDirectInputKey(VirtualKey key)
        {
            return (Key)Array.IndexOf(map, key);
        }
    }
    */
}

   