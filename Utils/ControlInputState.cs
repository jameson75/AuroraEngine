using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX.XInput;

namespace CipherPark.AngelJacket.Core.Utils
{
    public class ControlInputState
    {
        public const long DelayTimeAfterPress = 300;
        public const long DelayTimeAfterHold = 80;
        private List<VirtualKey> _delayedScannedKeysDown = new List<VirtualKey>();
        private InputState _inputState = null;
        private Dictionary<VirtualKey, long> _inputDelayExpirationTimes = new Dictionary<VirtualKey, long>();

        public ControlInputState(InputState inputState)
        {
            _inputState = inputState;
        }

        public InputState InputStateManager
        {
            get { return _inputState; }
        }

        public bool IsKeyUp(VirtualKey key)
        {
            return _inputState.IsKeyUp(key);             
        }

        public bool IsKeyDown(VirtualKey key, bool scanRealTime = false)
        { 
            if (scanRealTime)
                return _inputState.IsKeyDown(key);
            else
                return _delayedScannedKeysDown.Contains(key);
        }

        public bool IsKeyReleased(VirtualKey key)
        {
            return _inputState.IsKeyReleased(key);
        }

        public VirtualKey[] GetKeysDown(bool scanRealTime = false)
        {                       
            int[] keysDown = (scanRealTime) ? _inputState.GetKeysDown() : _delayedScannedKeysDown.ToArray();
            return keysDown;
        }

        public VirtualKey[] GetKeysReleased()
        {
            return _inputState.GetKeysReleased();
        }

        public void UpdateState()
        {
            List<VirtualKey> keysToRemove = new List<VirtualKey>();
            foreach (VirtualKey key in _inputDelayExpirationTimes.Keys)
            {
                if(_inputState.IsKeyUp(key))
                    keysToRemove.Add(key);
            }
            foreach (VirtualKey key in keysToRemove)                
                _inputDelayExpirationTimes.Remove(key);

            _delayedScannedKeysDown.Clear();

            VirtualKey[] keysDown = _inputState.GetKeysDown();
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

        private bool IsKeyDelayable(VirtualKey key)
        {
            return !(key == VirtualKey.RightShift ||
                   key == VirtualKey.LeftShift ||
                   key == VirtualKey.RightAlt ||
                   key == VirtualKey.LeftAlt ||
                   key == VirtualKey.RightControl ||
                   key == VirtualKey.LeftControl ||
                   key == VirtualKey.CapsLock);
        }

        public static WritableInput[] ConvertToWritableInput(VirtualKey[] keys, bool isEnterSpecialKey)
        {
            List<WritableInput> input = new List<WritableInput>();
            bool applyShift = keys.Any(k => k == VirtualKey.LeftShift || k == VirtualKey.RightShift);
            bool applyCaps = (applyShift && keys.All(k => k != VirtualKey.CapsLock)) ||
                              (!applyShift && keys.Any(k => k == VirtualKey.CapsLock));

            foreach (VirtualKey key in keys)
            {
                int virtualKeyInteger = (int)key;
                char ascii = char.MinValue;
                              //A-Z or a-z
                if (virtualKeyInteger >= 65 && virtualKeyInteger <= 90)
                {
                    //A-Z
                    if (applyShift)
                        ascii = (char)virtualKeyInteger;
                    //a-z
                    else
                        ascii = (char)(virtualKeyInteger + 32);
                }

                //D_0 - D_9 or their respective shift-symbols
                else if (virtualKeyInteger >= 48 && virtualKeyInteger <= 57)
                {
                    //D_0 - D_9
                    if (applyCaps)
                    {
                        if (virtualKeyInteger == 48)
                            ascii = (char)41; //)
                        else if (virtualKeyInteger == 49)
                            ascii = (char)33; //!
                        else if (virtualKeyInteger == 50)
                            ascii = (char)64; //@
                        else if (virtualKeyInteger == 51)
                            ascii = (char)35; //#
                        else if (virtualKeyInteger == 52)
                            ascii = (char)36; //$
                        else if (virtualKeyInteger == 53)
                            ascii = (char)37; //%
                        else if (virtualKeyInteger == 54)
                            ascii = (char)94; //^
                        else if (virtualKeyInteger == 55)
                            ascii = (char)38; //&
                        else if (virtualKeyInteger == 56)
                            ascii = (char)42; //*
                        else //if (xnaKeyCode == 57)
                            ascii = (char)40; //(
                    }
                    else
                        ascii = (char)virtualKeyInteger;
                }

                //NUM_PAD_0 - NUM_PAD_9
                else if (virtualKeyInteger >= 96 && virtualKeyInteger <= 105)
                    ascii = (char)(virtualKeyInteger - 48);

                //NUM_OPERATORS
                else if (key == VirtualKey.Divide)
                    ascii = (char)47; // /
                else if (key == VirtualKey.Add)
                    ascii = (char)43; //+
                else if (key == VirtualKey.Subtract)
                    ascii = (char)45; //-
                else if (key == VirtualKey.Multiply)
                    ascii = (char)42; //*

                else if (key == VirtualKey.Space)
                    ascii = (char)virtualKeyInteger;
                else if (key == VirtualKey.Enter)
                {
                    if (!isEnterSpecialKey)
                        ascii = '\n';
                }
                else if (key == VirtualKey.Tab)
                    ascii = '\t';
                else if (key == VirtualKey.Semicolon)
                    ascii = applyShift ? (char)58 : (char)59; // : or ;
                else if (key == VirtualKey.Equals)
                    ascii = applyShift ? (char)43 : (char)61; // + or =
                else if (key == VirtualKey.Underscore)
                    ascii = applyShift ? (char)159 : (char)45; // _ or -
                else if (key == VirtualKey.Period)
                    ascii = applyShift ? (char)62 : (char)46; // > or .
                else if (key == VirtualKey.ForwardSlash)
                    ascii = applyShift ? (char)63 : (char)47; // / or ?
                else if (key == VirtualKey.Tilde)
                    ascii = applyShift ? (char)126 : (char)96; // ~ or `
                else if (key == VirtualKey.LeftBrace)
                    ascii = applyShift ? (char)91 : (char)123; // { or [
                else if (key == VirtualKey.BackSlash)
                    ascii = applyShift ? (char)124 : (char)92; // | or \
                else if (key == VirtualKey.RightBrace)
                    ascii = applyShift ? (char)93 : (char)125; // } OR ]
                else if (key == VirtualKey.Apostrophe)
                    ascii = applyShift ? (char)34 : (char)39; // " or '
                else if (key == VirtualKey.Comma)
                    ascii = applyShift ? (char)60 : (char)44; // < or ,

                WritableInput wi = new WritableInput();
                wi.Ascii = ascii;
                wi.KeyType = (ascii == char.MinValue) ? WritableInputType.Special : WritableInputType.Printable;
                wi.IsAlt = keys.Any(k => k == VirtualKey.LeftAlt || k == VirtualKey.RightAlt);
                wi.IsCtrl = keys.Any(k => k == VirtualKey.LeftControl || k == VirtualKey.RightControl);
                wi.Key = key;
                input.Add(wi);
            }
            return input.ToArray();
        }
    }

    public struct WritableInput
    {
        public VirtualKey Key;
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
