using System;
using System.Collections.Generic;
using System.Linq;

namespace CipherPark.AngelJacket.Core.Utils
{
    public class ControlInputState
    {
        public const long DelayTimeAfterPress = 300;
        public const long DelayTimeAfterHold = 80;
        private List<Keys> _delayedScannedKeysDown = new List<Keys>();
        private InputState _inputState = null; 
        private Dictionary<Keys, long> _inputDelayExpirationTimes = new Dictionary<Keys, long>();

        public ControlInputState(InputState inputState)
        {
            _inputState = inputState;
        }

        public InputState InputStateManager
        {
            get { return _inputState; }
        }

        public bool IsKeyUp(Keys key)
        {
            return _inputState.IsKeyUp(key);             
        }

        public bool IsKeyDown(Keys key, bool scanRealTime = false)
        { 
            if (scanRealTime)
                return _inputState.IsKeyDown(key);
            else
                return _delayedScannedKeysDown.Contains(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            return _inputState.IsKeyReleased(key);
        }

        public Keys[] GetKeysDown(bool scanRealTime = false)
        {                       
            Keys[] keysDown = (scanRealTime) ? _inputState.GetKeysDown() : _delayedScannedKeysDown.ToArray();
            return keysDown;
        }

        public Keys[] GetKeysReleased()
        {
            return _inputState.GetKeysReleased();
        }

        public void UpdateState()
        {
            List<Keys> keysToRemove = new List<Keys>();
            foreach(Keys key in _inputDelayExpirationTimes.Keys)
            {
                if(_inputState.IsKeyUp(key))
                    keysToRemove.Add(key);
            }
            foreach(Keys key in keysToRemove)                
                _inputDelayExpirationTimes.Remove(key);

            _delayedScannedKeysDown.Clear();

            Keys[] keysDown = _inputState.GetKeysDown();
            for(int i = 0; i < keysDown.Length; i++ )
            {
                if (IsKeyDelayable(keysDown[i]))
                {
                    if (_inputDelayExpirationTimes.ContainsKey(keysDown[i]))
                    {
                        if (_inputState.StateUpdateTime >= _inputDelayExpirationTimes[keysDown[i]])
                        {
                            _inputDelayExpirationTimes[keysDown[i]] = _inputState.StateUpdateTime + DelayTimeAfterHold;
                            _delayedScannedKeysDown.Add(keysDown[i]);
                        }
                    }
                    else
                    {
                        _inputDelayExpirationTimes.Add(keysDown[i], _inputState.StateUpdateTime + DelayTimeAfterPress);
                        _delayedScannedKeysDown.Add(keysDown[i]);
                    }
                }
                else
                    _delayedScannedKeysDown.Add(keysDown[i]);
            }
        }

        private bool IsKeyDelayable(Keys key)
        {
            return !(key == Keys.RightShift ||
                   key == Keys.LeftShift ||
                   key == Keys.RightAlt ||
                   key == Keys.LeftAlt ||
                   key == Keys.RightControl ||
                   key == Keys.LeftControl ||
                   key == Keys.CapsLock);
        }

        public static WritableInput[] ConvertToWritableInput(Keys[] keys, bool isEnterSpecialKey)
        {
            List<WritableInput> input = new List<WritableInput>();
            bool applyShift = keys.Any(k => k == Keys.LeftShift || k == Keys.RightShift);
            bool applyCaps = (applyShift && keys.All(k => k != Keys.CapsLock)) ||
                              (!applyShift && keys.Any(k => k == Keys.CapsLock));

            foreach (Keys key in keys)
            {
                int xnaKeyCode = (int)key;
                char ascii = char.MinValue;
                              //A-Z or a-z
                if (xnaKeyCode >= 65 && xnaKeyCode <= 90)
                {
                    //A-Z
                    if (applyShift)
                        ascii = (char)xnaKeyCode;
                    //a-z
                    else
                        ascii = (char)(xnaKeyCode + 32);
                }
                //D_0 - D_9 or their respective shift-symbols
                else if (xnaKeyCode >= 48 && xnaKeyCode <= 57)
                {
                    //D_0 - D_9
                    if (applyCaps)
                    {
                        if (xnaKeyCode == 48)
                            ascii = (char)41; //)
                        else if (xnaKeyCode == 49)
                            ascii = (char)33; //!
                        else if (xnaKeyCode == 50)
                            ascii = (char)64; //@
                        else if (xnaKeyCode == 51)
                            ascii = (char)35; //#
                        else if (xnaKeyCode == 52)
                            ascii = (char)36; //$
                        else if (xnaKeyCode == 53)
                            ascii = (char)37; //%
                        else if (xnaKeyCode == 54)
                            ascii = (char)94; //^
                        else if (xnaKeyCode == 55)
                            ascii = (char)38; //&
                        else if (xnaKeyCode == 56)
                            ascii = (char)42; //*
                        else //if (xnaKeyCode == 57)
                            ascii = (char)40; //(
                    }
                    else
                        ascii = (char)xnaKeyCode;
                }
                //NUM_PAD_0 - NUM_PAD_9
                else if (xnaKeyCode >= 96 && xnaKeyCode <= 105)
                    ascii = (char)(xnaKeyCode - 48);
                else if (key == Keys.Space)
                    ascii = (char)xnaKeyCode;
                else if (key == Keys.Enter)
                {
                    if (!isEnterSpecialKey)
                        ascii = '\n';
                }
                else if (key == Keys.Tab)
                    ascii = '\t';
                else if (key == Keys.OemSemicolon)
                    ascii = applyShift ? (char)58 : (char)59;
                else if (key == Keys.OemPlus)
                    ascii = applyShift ? (char)43 : (char)61;
                else if (key == Keys.OemMinus)
                    ascii = applyShift ? (char)159 : (char)45;
                else if (key == Keys.OemPeriod)
                    ascii = applyShift ? (char)62 : (char)46;
                else if (key == Keys.OemQuestion)
                    ascii = applyShift ? (char)63 : (char)47;
                else if (key == Keys.OemTilde)
                    ascii = applyShift ? (char)126 : (char)96;
                else if (key == Keys.OemOpenBrackets)
                    ascii = applyShift ? (char)91 : (char)123;
                else if (key == Keys.OemPipe)
                    ascii = applyShift ? (char)124 : (char)92;
                else if (key == Keys.OemCloseBrackets)
                    ascii = applyShift ? (char)93 : (char)125;
                else if (key == Keys.OemQuotes)
                    ascii = applyShift ? (char)34 : (char)39;
                else if (key == Keys.OemBackslash)
                    ascii = (char)92;

                WritableInput wi = new WritableInput();
                wi.Ascii = ascii;
                wi.KeyType = (ascii == char.MinValue) ? WritableInputType.Special : WritableInputType.Printable;
                wi.IsAlt = keys.Any(k=> k == Keys.LeftAlt || k == Keys.RightAlt);
                wi.IsCtrl = keys.Any(k=> k == Keys.LeftControl || k == Keys.RightControl);
                wi.Key = key;
                input.Add(wi);
            }
            return input.ToArray();
        }
    }

    public struct WritableInput
    {
        public Keys Key;
        public WritableInputType KeyType;
        public char Ascii;
        public bool IsAlt;
        public bool IsCtrl;
    }

    public enum WritableInputType
    {
        Printable,
        Special
    }
}
