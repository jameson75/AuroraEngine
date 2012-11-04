using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.Services;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.World;
using SharpDX;
using SharpDX.Direct3D11;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public enum EditorMode
    {
        None,
        Select,
        Navigate      
    }

    public enum NavigationMode
    {
        None,
        Rotate,
        Pan
    }

    public class SceneControl : UIControl
    {
        private Camera _defaultCamera = null;
        private Camera _currentCamera = null;
        private Scene _scene = null;
        private EditorMode EditorMode = EditorMode.None;
        private NavigationMode NavigationMode = NavigationMode.None;
        private Rectangle rotationRectangle = Rectangle.Empty;
        private int rotationEllipseDiameter = 0;
        private DrawingPoint mouseMoveFrom = new DrawingPoint(0, 0);
        private bool rotateAboutZ = false;

        public SceneControl(IUIRoot visualRoot) : base(visualRoot)
        {
            _defaultCamera = new Camera(Matrix.LookAtLH(new Vector3(0, 50, 100), new Vector3(0, 0, 0), Vector3.UnitY),
                                        Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(45), this.DeviceAspectRatio, 1.0f, 1000.0f));
            _currentCamera = _defaultCamera;
            this.EditorMode = EditorMode.Select;
            _scene = new Scene(visualRoot.Game);
        }

        public Scene Scene { get { return _scene; } }

        public Camera CurrentCamera
        {
            get { return _currentCamera; }
        }

        private float DeviceAspectRatio
        {
            get { return (float)Game.GraphicsDeviceContext.Rasterizer.GetViewports()[0].Width / (float)Game.GraphicsDeviceContext.Rasterizer.GetViewports()[0].Height; }
        }

        public override void Draw(long gameTime)
        {
            base.Draw(gameTime);
        }        

        protected override void OnSizeChanged()
        {
            rotationEllipseDiameter = Math.Min(this.Bounds.Height, this.Bounds.Width) - 80;
            rotationRectangle = RectangleExtension.CreateLTWH((this.Bounds.Width - rotationEllipseDiameter) / 2, (this.Bounds.Height - rotationEllipseDiameter) / 2, rotationEllipseDiameter, rotationEllipseDiameter);
            base.OnSizeChanged();
        }     

        public override void Update(long gameTime)
        {
            InputService inputServices = (InputService)Game.Services.GetService(typeof(InputService));
            InputState inputState = inputServices.GetInputState();
            InputState.MouseButton[] pressedMouseButtons = inputState.GetMouseButtonsPressed();
            for(int i = 0; i < pressedMouseButtons.Length; i++)            
                OnMouseDown(pressedMouseButtons[i], inputState.GetMouseLocation());

            InputState.MouseButton[] downMouseButtons = inputState.GetMouseButtonsDown();
            if (inputState.GetPreviousMouseLocation() != inputState.GetMouseLocation())
                OnMouseMove(downMouseButtons, inputState.GetMouseLocation());             

            InputState.MouseButton[] releasedMouseButtons = inputState.GetMouseButtonsReleased();
            for (int i = 0; i < releasedMouseButtons.Length; i++)
                OnMouseUp(releasedMouseButtons[i], inputState.GetMouseLocation());

            if (inputState.GetMouseWheelDelta() != 0)
                OnMouseWheel(inputState.GetMouseWheelDelta());
        }

        protected void OnMouseDown(InputState.MouseButton mouseButton, DrawingPoint location)
        {          
            switch (this.EditorMode)
            {
                case EditorMode.Navigate:
                    if (mouseButton == InputState.MouseButton.Left)
                    {                        
                        this.Capture = true;
                        this.mouseMoveFrom = location;
                        Vector2 vectorFrom = new Vector2(mouseMoveFrom.X, mouseMoveFrom.Y);
                        Vector2 vectorEllipseCenter = new Vector2(rotationRectangle.X + (rotationRectangle.Width / 2), rotationRectangle.Y + (rotationRectangle.Height / 2));
                        float distance = Vector2.Distance(vectorEllipseCenter, vectorFrom);
                        rotateAboutZ = (distance > rotationEllipseDiameter / 2);
                    }
                    break;
                case EditorMode.Select:
                    if (mouseButton == InputState.MouseButton.Left)
                    {
                        Viewport vp = Game.GraphicsDeviceContext.Rasterizer.GetViewports()[0];
                        //Vector3 mousePointNear = vp.Unproject(new Vector3(location.X, location.Y, vp.MinDepth), CurrentCamera.ProjectionMatrix, CurrentCamera.ViewMatrix, Matrix.Identity);
                        //Vector3 mousePointFar = vp.Unproject(new Vector3(location.X, location.Y, vp.MaxDepth), CurrentCamera.ProjectionMatrix, CurrentCamera.ViewMatrix, Matrix.Identity);
                        Vector3 mousePointNear = Vector3.Unproject(new Vector3(location.X, location.Y, vp.MinDepth), vp.TopLeftX, vp.TopLeftX, vp.Width, vp.Height, vp.MinDepth, vp.MaxDepth, CurrentCamera.ViewMatrix * CurrentCamera.ProjectionMatrix);
                        Vector3 mousePointFar = Vector3.Unproject(new Vector3(location.X, location.Y, vp.MaxDepth), vp.TopLeftX, vp.TopLeftX, vp.Width, vp.Height, vp.MinDepth, vp.MaxDepth, CurrentCamera.ViewMatrix * CurrentCamera.ProjectionMatrix);
                        Vector3 mouseRay = mousePointFar - mousePointNear;
                        mouseRay.Normalize();
                        System.Diagnostics.Trace.WriteLine(mousePointNear);
                    }
                    break;
            }
        }

        protected void OnMouseUp(InputState.MouseButton mouseButton, DrawingPoint location)
        {          
            if (this.Capture)
            {
                this.Capture = false;
                this.mouseMoveFrom = new DrawingPoint(0,0);
            }
        }

        protected void OnMouseWheel(float mouseWheelDelta)
        {
            CurrentCamera.ViewMatrix = CurrentCamera.ViewMatrix * Matrix.Translation(0, 0, mouseWheelDelta / 5);          
        }

        protected void OnMouseMove(InputState.MouseButton[] mouseButtonsDown,DrawingPoint location)
        {         
            switch (EditorMode)
            {
                case EditorMode.Navigate:
                    switch (NavigationMode)
                    {
                        case NavigationMode.Rotate:
                            //this.Cursor = Cursors.Cross;
                            if (mouseButtonsDown.Contains(InputState.MouseButton.Left))
                            {
                                Vector2 offset = Vector2.Subtract(new Vector2(location.X, location.Y), new Vector2(mouseMoveFrom.X, mouseMoveFrom.Y));
                                Vector3 negatedTranslation = Vector3.Negate(CurrentCamera.ViewMatrix.TranslationVector);
                                CurrentCamera.ViewMatrix = CurrentCamera.ViewMatrix * Matrix.Translation(negatedTranslation);
                                if (rotateAboutZ)
                                {      
                                    Vector2 zRotationOrigin = new Vector2(this.Bounds.Width / 2, this.Bounds.Height / 2);
                                    Vector2 zLocationVector = new Vector2(location.X, location.Y) - zRotationOrigin;
                                    Vector2 zMouseMoveFromVector = new Vector2(mouseMoveFrom.X, mouseMoveFrom.Y) - zRotationOrigin;
                                    zLocationVector.Normalize();
                                    zMouseMoveFromVector.Normalize();
                                    double zRotation = Math.Acos(MathUtil.Clamp(Vector2.Dot(zLocationVector, zMouseMoveFromVector), -1.0f, 1.0f));
                                    double diff = Math.Atan2(zMouseMoveFromVector.Y, zMouseMoveFromVector.X) - Math.Atan2(zLocationVector.Y, zLocationVector.X);
                                    float factor = (diff > 0) ? 1 : -1;
                                    //float zRotation(MathHelper.ToRadians(-offset.Y);
                                    CurrentCamera.ViewMatrix = CurrentCamera.ViewMatrix * Matrix.RotationZ((float)zRotation * factor);
                                }
                                else
                                    CurrentCamera.ViewMatrix = CurrentCamera.ViewMatrix * Matrix.RotationX(MathUtil.DegreesToRadians(offset.Y));
                                CurrentCamera.ViewMatrix = CurrentCamera.ViewMatrix * Matrix.Translation(Vector3.Negate(negatedTranslation));
                                if (!rotateAboutZ)
                                    CurrentCamera.ViewMatrix = Matrix.Multiply(Matrix.RotationY(MathUtil.DegreesToRadians(offset.X)), CurrentCamera.ViewMatrix);
                                mouseMoveFrom = location;                                
                            }
                            break;
                        case NavigationMode.Pan:
                            //this.Cursor = Cursors.Hand;
                            if (mouseButtonsDown.Contains(InputState.MouseButton.Left))
                            {
                                Vector2 offset = Vector2.Subtract(new Vector2(location.X, location.Y), new Vector2(mouseMoveFrom.X, mouseMoveFrom.Y));
                                //TODO: Unhardcode the translation factor using one derived from the z coordinate (in camera space) of
                                //the intersection point of the bounding box surrounding the visible scene graph object closest to
                                //the camera.
                                float translationFactor = 5.0f;
                                CurrentCamera.ViewMatrix = Matrix.Multiply(CurrentCamera.ViewMatrix, Matrix.Translation(offset.X * translationFactor, -offset.Y * translationFactor, 0));
                                mouseMoveFrom = location;                               
                            }
                            break;
                        case NavigationMode.None:
                            //this.Cursor = Cursors.Default;
                            break;
                    }
                    break;
                case EditorMode.Select:

                    break;
            }
        }     
    }
}
