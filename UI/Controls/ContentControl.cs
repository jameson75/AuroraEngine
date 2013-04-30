using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.UI.Components;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class ContentControl : UIControl
    {        
        private UIContent _content = null;

        public ContentControl(IUIRoot visualRoot) : base(visualRoot)
        {
            _content = visualRoot.Theme.ContentControl.ContentStyle.GenerateContent();
            Size = visualRoot.Theme.ContentControl.Size.Value;
        }

        public ContentControl(IUIRoot visualRoot, UIContent content)
            : base(visualRoot)
        {            
            Content = content;
            Size = visualRoot.Theme.ContentControl.Size.Value;
        }

        public override void Draw(long gameTime)
        {
            _content.Draw(gameTime);
            base.Draw(gameTime);
        }

        public UIContent Content
        { 
            get 
            { 
                return _content; 
            }
            set
            {
                if (_content != null)
                    _content.Container = null;
                
                _content = value;
                
                if (_content != null)
                    _content.Container = this;
            }
        }

        public override void ApplyTemplate(UIControlTemplate template)
        {
            ContentControlTemplate contentControlTemplate = (ContentControlTemplate)template;
            if (contentControlTemplate.ContentStyle != null)
                Content = contentControlTemplate.ContentStyle.GenerateContent();
            base.ApplyTemplate(template);
        }
    }
}
