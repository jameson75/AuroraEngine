using CipherPark.Aurora.Core.Utils;
using System;
using System.Collections.Generic;

namespace CipherPark.Aurora.Core.Toolkit
{
    public class DirectWriteTextProcessor: IDisposable, IProceduralTextureProcessor
    {
        public DirectWriteTextProcessor(DirectWriteSurface11 surface)
        {
            Surface = surface;
            DisplayTextMap = new Dictionary<string, DirectText>();
        }
        
        public Dictionary<string, DirectText> DisplayTextMap { get; }

        public DirectWriteSurface11 Surface { get; }

        public void Dispose()
        {
            DisplayTextMap.Clear();
        }

        public void Render()
        {
            Surface.DrawText(DisplayTextMap.Values);
        }
    }
}
