using System;
using System.Collections.Generic;
using CipherPark.Aurora;
using CipherPark.Aurora.Core.Services;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
//
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core
{
    public class GameTime
    {
        private long _startTime = 0;
        private long _updateTime = 0;
        private long _totalPauseTime = 0;
        private long? _pauseTime = null;
        private long _lastFrameStartTime = 0;
        private long _currentFrameOrdinal = 0;
        private double _averageFrameRate = 0;

        public GameTime(bool intialize = true)
        {
            Initialize();
        }

        public void Initialize()
        {
            _startTime = System.Environment.TickCount;
        }

        public void Update()
        {
            _updateTime = System.Environment.TickCount;
        }

        public long GetTotalRealtime(bool immediate = false)
        {
            return (immediate) ? System.Environment.TickCount - _startTime : _updateTime - _startTime;
        }

        public long GetTotalSimtime(bool immediate = false)
        {
            return GetTotalRealtime(immediate) - _totalPauseTime;
        }

        public void PauseSimulationClock()
        {
            if (_pauseTime == null)
                _pauseTime = _updateTime;
        }

        public void ResumeSimulationClock()
        {
            if (_pauseTime != null)
            {
                _totalPauseTime += _updateTime - _pauseTime.Value;
                _pauseTime = null;
            }
        }

        public double FrameRate
        { get; set; }

        public bool IsNextFrameReady
        {
            get
            {
                if (FrameRate <= 0)
                    throw new InvalidOperationException("FrameRate is not a positive, non-zero real number");
                double frameLength = 1000.0 / FrameRate;
                return (_lastFrameStartTime - _startTime > frameLength || _lastFrameStartTime == 0);
            }
        }

        public long NextFrame()
        {            
            long frameStartTime = _updateTime;    
            
            //Calculate the average time between calls to BeginFrame()
            //NOTE: If this frame is the first (where _lastFrameTime == 0) then we consider the rate of the previous frame "0"
            if (_lastFrameStartTime != 0)
            {
                double rateCurrentFrame = (frameStartTime - _lastFrameStartTime) / 1000.0;
                //Calculate and store the average frame rate of all previous frames since the start of game time.
                _averageFrameRate = ((_averageFrameRate * (_currentFrameOrdinal - 1)) + rateCurrentFrame) / _currentFrameOrdinal;
            }        
            
            _lastFrameStartTime = _updateTime;     
            
            //Set the ordinal of the current frame. (Ordinal start at "1").
            _currentFrameOrdinal++;

            return _currentFrameOrdinal;                     
        }
       
        public double AverageFrameRate
        {
            get { return _averageFrameRate; }
        }        
    }
}