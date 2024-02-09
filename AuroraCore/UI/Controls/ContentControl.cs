using CipherPark.Aurora.Core.UI.Components;
using CipherPark.Aurora.Core.Extensions;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Controls
{
    public class ContentControl : UIControl
    {        
        private UIContent _content = null;

        public ContentControl(IUIRoot visualRoot) : base(visualRoot)
        {
            visualRoot.Theme?.Apply(this);
        }

        public ContentControl(IUIRoot visualRoot, UIContent content)
            : base(visualRoot)
        {            
            Content = content;
            visualRoot.Theme?.Apply(this);
        }     

        protected override void OnDraw()
        {
            if(_content != null )
                _content.Draw();
            base.OnDraw();
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

        public virtual void SizeToContent()
        {
            if (this.Content != null)
                this.Size = this.Content.CalculateSmallestBoundingRect().Size();           
        }

     
        public override void ApplyTemplate(UIControlTemplate template)
        {
            ContentControlTemplate contentControlTemplate = (ContentControlTemplate)template;
            if (contentControlTemplate.ContentStyle != null)
                Content = contentControlTemplate.ContentStyle.GenerateContent();
            base.ApplyTemplate(template);
        }

        public static ContentControl FromTemplate(IUIRoot visualRoot, ContentControlTemplate template)
        {
            ContentControl contentControl = new ContentControl(visualRoot);
            contentControl.ApplyTemplate(template);
            return contentControl;
        }    
  

        /*
        public static ContentControl CreateLabelControl(IUIRoot visualRoot, TextContent text, Color? backgroundColor = null)
        {
            Color backgroundColor_ = backgroundColor != null ? backgroundColor.Value : Color.Transparent;
            return new ContentControl(visualRoot, new LayeredContent(new UIContent[] { new ColorContent(backgroundColor_), text}));
        }

        public static ContentControl CreateLabelControl(IUIRoot visualRoot, string text, SpriteFont font, Color fontColor, Color? backgroundColor = null)
        {
            Color backgroundColor_ = backgroundColor != null ? backgroundColor.Value : Color.Transparent;
            return new ContentControl(visualRoot, new LayeredContent(new UIContent[] {  new ColorContent(backgroundColor_),
                                                                                        new TextContent(text, font, fontColor)                                                                                         }));
        }
        */
    }
}
