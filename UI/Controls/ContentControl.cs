using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class ContentControl : UIControl
    {        
        private UIContent _content = null;
        
        public ContentControl(Components.IUIRoot visualRoot, UIContent content)
            : base(visualRoot)
        {
            if( content == null)
                throw new ArgumentNullException("content");
            _content = content;
            _content.Container = this;
        }

        public override void Draw(long gameTime)
        {
            _content.Draw(gameTime);
            base.Draw(gameTime);
        }

        public UIContent Content { get { return _content; } }
    }
}
