using System;
using System.Collections.Generic;
using SharpDX;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public abstract class UIContent
    {
        public UIContent()
        {
            
        }

        public virtual void Draw(long gameTime)
        { 

        }

        //TODO: Deprecate this method.
        public virtual void Load(string path)
        {

        }

        public UIControl Container { get; set; }

        public abstract Rectangle CalculateSmallestBoundingRect();
    }
}
