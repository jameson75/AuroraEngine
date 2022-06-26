using System;
using System.Collections.Generic;
using SharpDX.Direct3D11;
using SharpDX;
using CipherPark.Aurora.Core.UI.Controls;
using CipherPark.Aurora.Core.Animation;
using System.Linq;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.UI.Components
{
    public class UITree : IUIRoot
    {
        private IGameApp _game = null;        
        private UIControlCollection controls = null;
        private UIStyleCollection styles = null;
        private UIResourceCollection resources = null;
        private FocusManager focusManager = null;
        private List<ISimulatorController> _animationControllers = null;    
       
        public UITree(IGameApp game)
        {
            _game = game;
            controls = new UIControlCollection();
            styles = new UIStyleCollection();
            resources = new UIResourceCollection();
            focusManager = new FocusManager(this);
            _animationControllers = new List<ISimulatorController>();            
        }     

        public IGameApp Game { get { return _game; } }

        public UIControlCollection Controls { get { return controls; } }

        public UIStyleCollection Styles { get { return styles; } }

        public UIResourceCollection Resources { get { return resources; } }

        public FocusManager FocusManager { get { return focusManager; } }

        public List<ISimulatorController> Animations { get { return _animationControllers; } }

        public IUITheme Theme { get; set; }

        public Size2F ScreenSize
        {
            get
            {
                return new Size2F(_game.RenderTargetView.GetTextureDescription().Width,
                                  _game.RenderTargetView.GetTextureDescription().Height);                                                       
            }
        }      

        public void Update(GameTime gameTime)
        {
            focusManager.Update();

            UpdateAnimations(gameTime);

            foreach (UIControl control in this.controls.ToList())
                control.Update(gameTime);

            focusManager.PostUpdate();
        }

        public void Draw()
        {
            //******************************************************************************
            // NOTE: When using the SpriteBatch, the DepthStencilState and RasterizerState
            // resets so we cache them before drawing the control tree, then use it later.
            //******************************************************************************
            DepthStencilState oldState = Game.GraphicsDeviceContext.OutputMerger.DepthStencilState;
            RasterizerState oldRasterizerState = Game.GraphicsDeviceContext.Rasterizer.State;
            
            foreach (UIControl control in this.controls)
                control.Draw();
            
            Game.GraphicsDeviceContext.OutputMerger.DepthStencilState = oldState;
            Game.GraphicsDeviceContext.Rasterizer.State = oldRasterizerState;
        }        
        
        public void OnLoadComplete()
        {
            LoadComplete?.Invoke(this, EventArgs.Empty);
        }        

        private void UpdateAnimations(GameTime gameTime)
        {
            //Update animation controllers.
            //**********************************************************************************
            //NOTE: We use an auxilary controller collection to enumerate through, in 
            //the event that an updated controller alters this Simulator's Animation Controllers
            //collection.
            //**********************************************************************************
            List<ISimulatorController> auxAnimationControllers = new List<ISimulatorController>(_animationControllers);
            foreach (ISimulatorController controller in auxAnimationControllers)
            {
                controller.Update(gameTime);
                if (controller.IsSimulationFinal)
                    _animationControllers.Remove(controller);
            }
        }    

        public event EventHandler LoadComplete;        
    }   
   
}
  