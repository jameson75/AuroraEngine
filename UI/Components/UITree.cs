using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using CipherPark.AngelJacket.Core.UI.Design;
using CipherPark.AngelJacket.Core.UI.Controls;

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public class UITree : IUIRoot
    {
        private Game _game = null;
        private string _markUpFileName = null;
        private UIControlCollection controls = null;
        private UIStyleCollection styles = null;
        private UIResourceCollection resources = null;
        private FocusManager focusManager = null;

        private Dictionary<string, UIControlParser> _controlParsers = new Dictionary<string, UIControlParser>();
        private Dictionary<string, UIStyleParser> _styleParsers = new Dictionary<string, UIStyleParser>();
        private Dictionary<string, UIResourceParser> _resourceParsers = new Dictionary<string, UIResourceParser>();

        public UITree(Game game)
        {
            _game = game;
            controls = new UIControlCollection();
            styles = new UIStyleCollection();
            resources = new UIResourceCollection();
            focusManager = new FocusManager(this);
        }

        public Game Game { get { return _game; } }

        public UIControlCollection Controls { get { return controls; } }

        public UIStyleCollection Styles { get { return styles; } }

        public UIResourceCollection Resources { get { return resources; } }

        public FocusManager FocusManager { get { return focusManager; } }

        public void Initialize(string markupFileName)
        {
            _markUpFileName = markupFileName;
            RegisterStandardParsers();
        }

        public void LoadContent()
        {
            //Load control tree from markup file.
            if (_markUpFileName == null)
                throw new InvalidOperationException("Markup file not specified. Must call Initialize([markupFileName]), first.");
            Stream stream = TitleContainer.OpenStream(_markUpFileName);
            XDocument doc = XDocument.Load(stream);

            XElement resourcesElement = doc.Root.Elements("Resources").First();
            foreach (XElement element in resourcesElement.Elements())
            {
                UIResourceParser parser = this.GetRegisteredResourceParser(element.Name.LocalName);
                if (parser == null)
                    throw new System.IO.InvalidDataException("Resource parser not found.");
                UIResource resource = parser.CreateResource(this.Game);
                parser.Parse(this, element, resource);
                resources.Add(resource);
            }
            
            XElement stylesElement = doc.Root.Elements("Styles").First();                        
            foreach (XElement element in stylesElement.Elements())
            {
                UIStyleParser parser = this.GetRegisteredStyleParser(element.Name.LocalName);
                if (parser == null)
                    throw new System.IO.InvalidDataException("Style parser not found.");
                UIStyle style = parser.CreateStyle(this.Game);
                parser.Parse(this, element, style);
                styles.Add(style);
            }

            XElement controlsElement = doc.Root.Elements("Controls").First();
            foreach(XElement element in controlsElement.Elements())
            {               
                UIControlParser parser = this.GetRegisteredControlParser(element.Name.LocalName);
                if (parser == null)
                    throw new System.IO.InvalidDataException("Control parser not found.");
                UIControl newControl = parser.CreateControl(this);
                parser.Parse(this, element, newControl);
                controls.Add(newControl);
            }

            OnLoadComplete();
        }     

        public UIStyleParser GetRegisteredStyleParser(string elementName)
        {
            if (_styleParsers.ContainsKey(elementName) == false)
                return null;
            else
                return _styleParsers[elementName];
        }

        public UIControlParser GetRegisteredControlParser(string elementName)
        {
            if (_controlParsers.ContainsKey(elementName) == false)
                return null;
            else
                return _controlParsers[elementName];
        }

        public UIResourceParser GetRegisteredResourceParser(string elementName)
        {
            if( _resourceParsers.ContainsKey(elementName) == false )
                return null;
            else
                return _resourceParsers[elementName];
        }

        public void Update(GameTime gameTime)
        {
            focusManager.Update();

            foreach (UIControl control in this.controls)
                control.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            foreach (UIControl control in this.controls)
                control.Draw(gameTime);                
        }      

        private void RegisterStandardParsers()
        {
            RegisterStyleParser(new TextStyleParser(), "TextStyle");
            RegisterStyleParser(new ColorStyleParser(), "ColorStyle");

            RegisterResourceParser(new PathResourceParser(), "PathResource");

            RegisterControlParser(new MultiScreenControlParser(), "MultiScreen");
            RegisterControlParser(new MenuControlParser(), "Menu");
            RegisterControlParser(new ImageControlParser(), "Image");            
        }

        public void RegisterStyleParser(UIStyleParser parser, string elementName)
        {
            _styleParsers.Add(elementName, parser);
        }

        public void RegisterResourceParser(UIResourceParser parser, string elementName)
        {
            _resourceParsers.Add(elementName, parser);
        }

        public void RegisterControlParser(UIControlParser parser, string elementName)
        {
            _controlParsers.Add(elementName, parser);
        }

        public void OnLoadComplete()
        {
            EventHandler handler = LoadComplete;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public event EventHandler LoadComplete;
    }   
   
}
  