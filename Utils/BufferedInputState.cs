using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX.XInput;
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
    public class BufferedInputState
    {
        public const long DelayTimeAfterPress = 300;
        public const long DelayTimeAfterHold = 40;
        private InputState _inputState = null;
        private List<Key> _delayedScannedKeysDown = new List<Key>();        
        private Dictionary<Key, long> _inputDelayExpirationTimes = new Dictionary<Key, long>();

        public BufferedInputState(InputState inputState)
        {
            _inputState = inputState;
        }

        public InputState InputState
        {
            get { return _inputState; }
        }

        public bool IsKeyUp(Key key)
        {
            return _inputState.IsKeyUp(key);             
        }

        public bool IsKeyDown(Key key, bool scanRealTime = false)
        { 
            if (scanRealTime)
                return _inputState.IsKeyDown(key);
            else
                return _delayedScannedKeysDown.Contains(key);
        }
        
        public bool IsKeyHit(Key key)
        {
            return _inputState.IsKeyHit(key);
        }

        public bool IsKeyReleased(Key key)
        {
            return _inputState.IsKeyReleased(key);
        }

        public void DiscardKey(Key key)
        {
            _inputState.DiscardKey(key);
        }

        public Key[] GetKeysDown(bool scanRealTime = false)
        {                       
            Key[] keysDown = (scanRealTime) ? _inputState.GetKeysDown() : _delayedScannedKeysDown.ToArray();
            return keysDown;
        }

        public Key[] GetKeysReleased()
        {
            return _inputState.GetKeysReleased();
        }    

        /// <summary>
        /// This method only needs be called once per frame and must be called before any input is read.
        /// </summary>
        public void UpdateState()
        {
            List<Key> keysToRemove = new List<Key>();
            foreach (Key key in _inputDelayExpirationTimes.Keys)
            {
                if(_inputState.IsKeyUp(key))
                    keysToRemove.Add(key);
            }
            foreach (Key key in keysToRemove)                
                _inputDelayExpirationTimes.Remove(key);

            _delayedScannedKeysDown.Clear();

            Key[] keysDown = _inputState.GetKeysDown();
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

        private bool IsKeyDelayable(Key key)
        {
            return !(key == Key.RightShift ||
                   key == Key.LeftShift ||
                   key == Key.RightAlt ||
                   key == Key.LeftAlt ||
                   key == Key.RightControl ||
                   key == Key.LeftControl ||
                   key == Key.Capital);
        }

        public static AsciiCharacterInfo[] ConvertToAsciiCharacters(Key[] keys, AsciiCharacterConversionFlags flags)
        {
            List<AsciiCharacterInfo> input = new List<AsciiCharacterInfo>();
            bool applyShift = keys.Any(k => k == Key.LeftShift || k == Key.RightShift);
            bool applyCaps = (applyShift && keys.All(k => k != Key.Capital)) ||
                              (!applyShift && keys.Any(k => k == Key.Capital));

            foreach (Key key in keys)
            {
                int dik = (int)key;
                char ascii = char.MinValue;
                //A-Z or a-z
                if ((dik >= 0x10 && dik <= 0x19) ||
                    (dik >= 0x1E && dik <= 0x26) ||
                    (dik >= 0x2C && dik <= 0x32))
                {
                    byte[] qwertyuiopAsciiVals = new byte[] { 81, 87, 69, 82, 84, 89, 85, 73, 79, 80 };
                    byte[] asdefghjklAsciiValues = new byte[] { 65, 83, 68, 70, 71, 72, 74, 75, 76 };
                    byte[] zxcvbnmAsciiValues = new byte[] { 90, 88, 67, 86, 66, 78, 77 };
                    byte val = 0;
                    if (dik >= 0x10 && dik <= 0x19)
                        val = qwertyuiopAsciiVals[dik - 0x10];
                    else if (dik >= 0x1E && dik <= 0x26)
                        val = asdefghjklAsciiValues[dik - 0x1E];
                    else
                        val = zxcvbnmAsciiValues[dik - 0x2C];
                    //A-Z
                    if (applyCaps)
                        ascii = (char)val;
                    //a-z
                    else
                        ascii = (char)(val + 32);
                }

                //D_0 - D_9 or their respective shift-symbols
                else if (dik >= 0x02 && dik <= 0x0B)
                {
                    //D_0 - D_9
                    if (applyShift)
                    {
                        if (dik == 0x0B)
                            ascii = (char)41; //)
                        else if (dik == 0x02)
                            ascii = (char)33; //!
                        else if (dik == 0x03)
                            ascii = (char)64; //@
                        else if (dik == 0x04)
                            ascii = (char)35; //#
                        else if (dik == 0x05)
                            ascii = (char)36; //$
                        else if (dik == 0x06)
                            ascii = (char)37; //%
                        else if (dik == 0x07)
                            ascii = (char)94; //^
                        else if (dik == 0x08)
                            ascii = (char)38; //&
                        else if (dik == 0x09)
                            ascii = (char)42; //*
                        else //if (xnaKeyCode == 0x0A)
                            ascii = (char)40; //(
                    }
                    else
                    {
                        if (dik == 0x0B)
                            ascii = (char)48; //0
                        else
                            ascii = (char)(48 + dik - 1);
                    }
                }

                //NUM_PAD_0 - NUM_PAD_9
                else if (dik >= 96 && dik <= 105)
                    ascii = (char)(dik - 48);

                //NUM_OPERATORS
                else if (key == Key.Divide)
                    ascii = (char)47; // /
                else if (key == Key.Add)
                    ascii = (char)43; //+
                else if (key == Key.Subtract)
                    ascii = (char)45; //-
                else if (key == Key.Multiply)
                    ascii = (char)42; //*

                else if (key == Key.Space)
                    ascii = (char)32;
                else if (key == Key.Return)
                {
                    if (!flags.HasFlag(AsciiCharacterConversionFlags.IgnoreNewLine))
                        ascii = '\n';
                }
                else if (key == Key.Tab)
                {
                    if (!flags.HasFlag(AsciiCharacterConversionFlags.IgnoreTab))
                        ascii = '\t';
                }
                else if (key == Key.Semicolon)
                    ascii = applyShift ? (char)58 : (char)59; // : or ;
                else if (key == Key.Equals || key == Key.PreviousTrack)
                    ascii = applyShift ? (char)43 : (char)61; // + or =
                else if (key == Key.Minus)
                    ascii = applyShift ? (char)95 : (char)45; // _ or -
                else if (key == Key.Period)
                    ascii = applyShift ? (char)62 : (char)46; // > or .
                else if (key == Key.Slash)
                    ascii = applyShift ? (char)63 : (char)47; // / or ?
                else if (key == Key.Grave)
                    ascii = applyShift ? (char)126 : (char)96; // ~ or `
                else if (key == Key.LeftBracket)
                    ascii = applyShift ? (char)91 : (char)123; // { or [
                else if (key == Key.Backslash)
                    ascii = applyShift ? (char)124 : (char)92; // | or \
                else if (key == Key.RightBracket)
                    ascii = applyShift ? (char)93 : (char)125; // } OR ]
                else if (key == Key.Apostrophe)
                    ascii = applyShift ? (char)34 : (char)39; // " or '
                else if (key == Key.Comma)
                    ascii = applyShift ? (char)60 : (char)44; // < or ,

                AsciiCharacterInfo wi = new AsciiCharacterInfo();
                wi.Ascii = ascii;
                wi.KeyType = (ascii == char.MinValue) ? AsciiCharacterType.Special : AsciiCharacterType.Printable;
                wi.IsAlt = keys.Any(k => k == Key.LeftAlt || k == Key.RightAlt);
                wi.IsCtrl = keys.Any(k => k == Key.LeftControl || k == Key.RightControl);
                wi.Key = key;
                input.Add(wi);
            }
            return input.ToArray();
        }

        /* ConvertToWritableInput(...)
        //*****************************************************************************************
        //NOTE: The conversion code below was actually meant to take in a VirtualKey as a parameter,
        //not a DirectInput Key. So I'm commenting out the code below and rewriting it for DirectInput 
        //keys.
        //*****************************************************************************************        
        public static WritableInput[] ConvertToWritableInput(Key[] keys, bool isEnterSpecialKey)
        {
            List<WritableInput> input = new List<WritableInput>();
            bool applyShift = keys.Any(k => k == Key.LeftShift || k == Key.RightShift);
            bool applyCaps = (applyShift && keys.All(k => k != Key.Capital)) ||
                              (!applyShift && keys.Any(k => k == Key.Capital));

            foreach (Key key in keys)
            {
                int virtualKeyInteger = (int)key;
                char ascii = char.MinValue;
                              //A-Z or a-z
                if (virtualKeyInteger >= 65 && virtualKeyInteger <= 90)
                {
                    //A-Z
                    if (applyCaps)
                        ascii = (char)virtualKeyInteger;
                    //a-z
                    else
                        ascii = (char)(virtualKeyInteger + 32);
                }

                //D_0 - D_9 or their respective shift-symbols
                else if (virtualKeyInteger >= 48 && virtualKeyInteger <= 57)
                {
                    //D_0 - D_9
                    if (applyShift)
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
                else if (key == Key.Divide)
                    ascii = (char)47; // /
                else if (key == Key.Add)
                    ascii = (char)43; //+
                else if (key == Key.Subtract)
                    ascii = (char)45; //-
                else if (key == Key.Multiply)
                    ascii = (char)42; //*

                else if (key == Key.Space)
                    ascii = (char)virtualKeyInteger;
                else if (key == Key.Return)
                {
                    if (!isEnterSpecialKey)
                        ascii = '\n';
                }
                else if (key == Key.Tab)
                    ascii = '\t';
                else if (key == Key.Semicolon)
                    ascii = applyShift ? (char)58 : (char)59; // : or ;
                else if (key == Key.Equals)
                    ascii = applyShift ? (char)43 : (char)61; // + or =
                else if (key == Key.Underline)
                    ascii = applyShift ? (char)95 : (char)45; // _ or -
                else if (key == Key.Period)
                    ascii = applyShift ? (char)62 : (char)46; // > or .
                else if (key == Key.Slash)
                    ascii = applyShift ? (char)63 : (char)47; // / or ?
                else if (key == Key.Grave)
                    ascii = applyShift ? (char)126 : (char)96; // ~ or `
                else if (key == Key.LeftBracket)
                    ascii = applyShift ? (char)91 : (char)123; // { or [
                else if (key == Key.Backslash)
                    ascii = applyShift ? (char)124 : (char)92; // | or \
                else if (key == Key.RightBracket)
                    ascii = applyShift ? (char)93 : (char)125; // } OR ]
                else if (key == Key.Apostrophe)
                    ascii = applyShift ? (char)34 : (char)39; // " or '
                else if (key == Key.Comma)
                    ascii = applyShift ? (char)60 : (char)44; // < or ,

                WritableInput wi = new WritableInput();
                wi.Ascii = ascii;
                wi.KeyType = (ascii == char.MinValue) ? WritableInputType.Special : WritableInputType.Printable;
                wi.IsAlt = keys.Any(k => k == Key.LeftAlt || k == Key.RightAlt);
                wi.IsCtrl = keys.Any(k => k == Key.LeftControl || k == Key.RightControl);
                wi.Key = key;
                input.Add(wi);
            }
            return input.ToArray();
        }
        */
    }

    public struct AsciiCharacterInfo
    {
        public Key Key;
        public AsciiCharacterType KeyType;
        public char Ascii;
        public bool IsAlt;
        public bool IsCtrl;
    }

    public enum AsciiCharacterType
    {
        Printable,
        Special
    }

    [Flags]
    public enum AsciiCharacterConversionFlags
    {
        IgnoreNewLine,
        IgnoreTab
    }
}
