﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using SharpDX.Direct3D11;
using SharpDX;
using CipherPark.AngelJacket.Core.UI.Design;
using CipherPark.AngelJacket.Core.UI.Controls;
using CipherPark.AngelJacket.Core.Module;
using CipherPark.AngelJacket.Core.UI.Animation;
using CipherPark.AngelJacket.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Components
{
    public class UITree : IUIRoot
    {
        private IGameApp _game = null;
        private string _markUpFileName = null;
        private UIControlCollection controls = null;
        private UIStyleCollection styles = null;
        private UIResourceCollection resources = null;
        private FocusManager focusManager = null;
        private List<IAnimationController> _animationControllers = null;
        //private IUITheme theme = null;       

        private Dictionary<string, UIControlParser> _controlParsers = new Dictionary<string, UIControlParser>();
        private Dictionary<string, UIStyleParser> _styleParsers = new Dictionary<string, UIStyleParser>();
        private Dictionary<string, UIResourceParser> _resourceParsers = new Dictionary<string, UIResourceParser>();
        
        public UITree(IGameApp game)
        {
            _game = game;
            controls = new UIControlCollection();
            styles = new UIStyleCollection();
            resources = new UIResourceCollection();
            focusManager = new FocusManager(this);
            _animationControllers = new List<IAnimationController>();
            //theme = new DefaultTheme();    
        }     

        public IGameApp Game { get { return _game; } }

        public UIControlCollection Controls { get { return controls; } }

        public UIStyleCollection Styles { get { return styles; } }

        public UIResourceCollection Resources { get { return resources; } }

        public FocusManager FocusManager { get { return focusManager; } }

        public List<IAnimationController> Animations { get { return _animationControllers; } }

        //public IUITheme Theme { get { return theme; } }

        public DrawingSizeF ScreenSize
        {
            get
            {
                return new DrawingSizeF(_game.RenderTarget.ResourceAs<Texture2D>().Description.Width,
                                        _game.RenderTarget.ResourceAs<Texture2D>().Description.Height);                                                       

            }
        }

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
            Stream stream = new System.IO.FileStream(_markUpFileName, FileMode.Open);
            XDocument doc = XDocument.Load(stream);
            stream.Close();

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
                UIStyle style = parser.CreateStyle();
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

            UpdateAnimations(gameTime);

            foreach (UIControl control in this.controls)
                control.Update(gameTime);

            focusManager.PostUpdate();
        }

        public void Draw(GameTime gameTime)
        {
            //******************************************************************************
            // NOTE: When using the SpriteBatch, the DepthStencilState and RasterizerState
            // resets so we cache them before drawing the control tree, then use it later.
            //******************************************************************************
            DepthStencilState oldState = Game.GraphicsDeviceContext.OutputMerger.DepthStencilState;
            RasterizerState oldRasterizerState = Game.GraphicsDeviceContext.Rasterizer.State;
            
            foreach (UIControl control in this.controls)
                control.Draw(gameTime);
            
            Game.GraphicsDeviceContext.OutputMerger.DepthStencilState = oldState;
            Game.GraphicsDeviceContext.Rasterizer.State = oldRasterizerState;
        }        

        public void OnLoadComplete()
        {
            EventHandler handler = LoadComplete;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void UpdateAnimations(GameTime gameTime)
        {
            //Update animation controllers.
            //**********************************************************************************
            //NOTE: We use an auxilary controller collection to enumerate through, in 
            //the event that an updated controller alters this Simulator's Animation Controllers
            //collection.
            //**********************************************************************************
            List<IAnimationController> auxAnimationControllers = new List<IAnimationController>(_animationControllers);
            foreach (IAnimationController controller in auxAnimationControllers)
            {
                controller.UpdateAnimation(gameTime);
                if (controller.IsAnimationComplete)
                    _animationControllers.Remove(controller);
            }
        }
        
        private void RegisterStandardParsers()
        {
            RegisterStyleParser(new TextStyleParser(), "TextStyle");
            RegisterStyleParser(new ColorStyleParser(), "ColorStyle");

            RegisterResourceParser(new PathResourceParser(), "PathResource");

            //RegisterControlParser(new MultiScreenControlParser(), "MultiScreen");
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

        public event EventHandler LoadComplete;
    }   
   
}
  