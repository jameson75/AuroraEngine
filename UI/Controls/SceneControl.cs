﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherPark.AngelJacket.Core.Services;
using CipherPark.AngelJacket.Core;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.World;
using CipherPark.AngelJacket.Core.UI.Components;
using CipherPark.AngelJacket.Core.World.Scene;
using SharpDX;
using SharpDX.Direct3D11;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

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
        //private Camera _defaultCamera = null;
        //private Camera _currentCamera = null;
        private EditorMode EditorMode = EditorMode.None;
        private NavigationMode NavigationMode = NavigationMode.None;
        private RectangleF rotationRectangle = RectangleF.Empty;
        private float rotationEllipseDiameter = 0f;
        private DrawingPoint mouseMoveFrom = new DrawingPoint(0, 0);
        private bool rotateAboutZ = false;
        private SceneNode _selectedNode = null;

        public SceneControl(IUIRoot visualRoot) : base(visualRoot)
        {
           // _defaultCamera = new Camera(visualRoot.Game,
           //                             Matrix.LookAtLH(new Vector3(0, 50, 100), new Vector3(0, 0, 0), Vector3.UnitY),
           //                             Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(45), this.DeviceAspectRatio, 1.0f, 1000.0f));
            //_currentCamera = _defaultCamera;
            this.EditorMode = EditorMode.Navigate;
            this.NavigationMode = Controls.NavigationMode.Rotate;
        }

        public Scene Scene { get; set; }

        public SceneNode SelectedNode { get { return _selectedNode; } }

        public Camera CurrentCamera
        {
            get { return Scene.Camera; }
        }

        //private float DeviceAspectRatio
        //{
        //    get { return (float)Game.GraphicsDeviceContext.Rasterizer.GetViewports()[0].Width / (float)Game.GraphicsDeviceContext.Rasterizer.GetViewports()[0].Height; }
        //}

        public override void Draw(long gameTime)
        {
            Scene.Draw(gameTime);
            base.Draw(gameTime);
        }        

        protected override void OnSizeChanged()
        {
            rotationEllipseDiameter = this.Bounds.Width / 3.0f;
            rotationRectangle = RectangleFExtension.CreateLTWH((this.Bounds.Width - rotationEllipseDiameter) / 2, (this.Bounds.Height - rotationEllipseDiameter) / 2, rotationEllipseDiameter, rotationEllipseDiameter);
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
                        System.Diagnostics.Trace.WriteLine(mousePointFar);
                        _selectedNode = SceneControl.PickNode(Scene.Nodes, mousePointNear, mousePointFar);
                    }
                    break;
            }
        }

        private static SceneNode PickNode(SceneNodes nodes, Vector3 near, Vector3 far)
        {           
            foreach(SceneNode node in nodes )
            {
                if (SceneControl.HitTestNode(node, near, far))
                    return node;
                else
                {
                    SceneNode pickedChild = PickNode(node.Children, near, far);
                    if (pickedChild != null)
                        return pickedChild;
                }
            }
            return null;
        }     

        private static bool HitTestNode(SceneNode node, Vector3 near, Vector3 far)
        {
            //TODO: Modify this condition when I have suitable design-time entities
            //for scene graph. The design-time node should lend it's self to hit testing
            //or, at least, indicate it's hit testable.            
            if (node is ModelSceneNode)
            {          
                Vector3 dir = far - near;
                Ray ray = new Ray(near, dir);
                World.Geometry.BasicModel model = ((ModelSceneNode)node).Model;
                BoundingBox wBoundingBox = BoundingBoxExtension.Transform(model.Mesh.BoundingBox, node.LocalToWorld(node.Transform).ToMatrix());
                if (wBoundingBox.Intersects(ref ray))
                    return true;                
            }

            return false;
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

        protected void OnMouseMove(InputState.MouseButton[] mouseButtonsDown, DrawingPoint location)
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
                                //get the offset between the last location we captured mouse movement and the current mouse location.
                                Vector2 offset = -Vector2.Subtract(new Vector2(location.X, location.Y), new Vector2(mouseMoveFrom.X, mouseMoveFrom.Y));
                                //negate the view matrix's translation vector and multiply it against the view matrix so that the view matrix becomes a rotation matrix about the origin.
                                Vector3 negatedTranslation = Vector3.Negate(CurrentCamera.ViewMatrix.TranslationVector);                               
                                CurrentCamera.ViewMatrix = CurrentCamera.ViewMatrix * Matrix.Translation(negatedTranslation);
                                //if the verticle mouse movement results in a rotation about the z-axis, we rotate the camera around it's own z-axis (view-space z-axis).
                                //We accomplish this by right multiplying the rotation about z before we re-apply the translations.
                                if (rotateAboutZ)
                                {      
#if !DISABLE_SCENE_CONTROL_ROTATION_RAY
                                    Vector2 zRotationOrigin = new Vector2(this.Bounds.Width / 2, this.Bounds.Height / 2);
                                    Vector2 zLocationVector = new Vector2(location.X, location.Y) - zRotationOrigin;
                                    Vector2 zMouseMoveFromVector = new Vector2(mouseMoveFrom.X, mouseMoveFrom.Y) - zRotationOrigin;
                                    zLocationVector.Normalize();
                                    zMouseMoveFromVector.Normalize();
                                    double zRotation = Math.Acos(MathUtil.Clamp(Vector2.Dot(zLocationVector, zMouseMoveFromVector), -1.0f, 1.0f));
                                    double diff = Math.Atan2(zMouseMoveFromVector.Y, zMouseMoveFromVector.X) - Math.Atan2(zLocationVector.Y, zLocationVector.X);
                                    float factor = (diff > 0) ? 1 : -1;
#else
                                    float zRotation = MathHelper.ToRadians(-offset.Y);
#endif                                    
                                    CurrentCamera.ViewMatrix = CurrentCamera.ViewMatrix * Matrix.RotationZ((float)zRotation * factor);                                    
                                }
                                //otherwise, the verticle mouse movement results in a revolution of the camera about an axis, v, which passes through
                                //the world origin and is parallel with the camera's (view-space) x-axis. We accomplish this 
                                //by right multiplying the rotation about x before we re-apply the translations.
                                else                            
                                    CurrentCamera.ViewMatrix = CurrentCamera.ViewMatrix * Matrix.RotationX(MathUtil.DegreesToRadians(offset.Y));
                                //Reapply the translations we negated earlier.
                                CurrentCamera.ViewMatrix = CurrentCamera.ViewMatrix * Matrix.Translation(Vector3.Negate(negatedTranslation));
                                //if verticle mouse movement doesn't result in a rotation about the z-axis, we revolve the camera about the world's (world space) y-axis by
                                //left multiplying a rotation about y, after reapplying the translations.
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
