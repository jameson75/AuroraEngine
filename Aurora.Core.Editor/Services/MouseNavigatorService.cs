using System;
using System.Linq;
using SharpDX;
using CipherPark.Aurora.Core.Animation;
using CipherPark.Aurora.Core.Utils;

namespace CipherPark.Aurora.Core.Services
{
    public class MouseNavigatorService
    {        
        private RectangleF rotationRectangle = RectangleF.Empty;        
        private Point mouseMoveFrom = new Point(0, 0);       
        private bool isMouseTracking = false;
        private bool rotateAboutZ = false;        
        private IGameApp gameApp = null;
        private Size2 navigationAreaSize = Size2.Empty;
        private int yRotationDirection = 0;

        public const int RotationEllipseDiameter = 400;

        public MouseNavigatorService(IGameApp gameApp)
        {            
            this.gameApp = gameApp;
            this.gameApp.ViewportSizeChanged += GameApp_ViewportSizeChanged;
        }

        private void GameApp_ViewportSizeChanged()
        {
            var td = gameApp.RenderTargetView.GetTextureDescription();
            navigationAreaSize = new Size2(td.Width, td.Height);           
            rotationRectangle = new RectangleF((navigationAreaSize.Width - RotationEllipseDiameter) / 2, (navigationAreaSize.Height - RotationEllipseDiameter) / 2, RotationEllipseDiameter, RotationEllipseDiameter);
        }

        public IGameApp Game => gameApp;       

        public NavigationMode Mode { get; set; } = NavigationMode.None;

        public bool IsMouseTracking { get => isMouseTracking; }

        public void NotifyMouseDown(Point location)
        {            
            this.isMouseTracking = true;
            this.mouseMoveFrom = location;
            Vector2 vectorFrom = new Vector2(mouseMoveFrom.X, mouseMoveFrom.Y);
            Vector2 vectorEllipseCenter = new Vector2(rotationRectangle.X + (rotationRectangle.Width / 2), rotationRectangle.Y + (rotationRectangle.Height / 2));
            float distance = Vector2.Distance(vectorEllipseCenter, vectorFrom);
            rotateAboutZ = (distance > RotationEllipseDiameter / 2);
        }

        public void NotifyMouseUp(Point location)
        {
            this.isMouseTracking = false;
            this.mouseMoveFrom = new Point(0, 0);
            this.yRotationDirection = 0;
        }

        public void NotifyMouseWheelChange(float mouseWheelDelta)
        {
            OnMouseWheelChange(mouseWheelDelta);            
        }
  
        public void OnMouseWheelChange(float mouseWheelDelta)
        { 
            var camera = Game.GetActiveScene().CameraNode.Camera;
            camera.ViewMatrix = camera.ViewMatrix * Matrix.Translation(0, 0, -mouseWheelDelta / 5);
        }

        public void NotifyMouseMove(bool buttonDown, Point location)
        {
            OnMouseMove(buttonDown, location);
        }       
        
        private void OnMouseMove(bool buttonDown, Point location)
        { 
            var cameraNode = Game.GetActiveScene().CameraNode;
            var camera = cameraNode.Camera;
            var mode = Mode;            
            Vector2 mouseOffset = -Vector2.Subtract(new Vector2(location.X, location.Y), new Vector2(mouseMoveFrom.X, mouseMoveFrom.Y));
            Vector3 platformLocation = new Vector3(0, 0, 0);
            Matrix platformTranslation = Matrix.Translation(platformLocation);            

            switch (mode)
            {
                case NavigationMode.Pan:
                    if (buttonDown && isMouseTracking)
                    {
                        //TODO: Unhardcode the translation factor using one derived from the z coordinate (in camera space) of
                        //the intersection point of the bounding box surrounding the visible scene graph object closest to
                        //the camera.
                        float translationFactor = 5.0f;
                        var traverseVector = cameraNode.LocalToWorldNormal(new Vector3(-mouseOffset.X * translationFactor, mouseOffset.Y * translationFactor, 0));
                        traverseVector = cameraNode.WorldToParentNormal(traverseVector);
                        camera.ViewMatrix = Matrix.Translation(traverseVector) * camera.ViewMatrix;
                        mouseMoveFrom = location;
                        accumTraverseVector += traverseVector;
                    }
                    break;
                case NavigationMode.PlatformRotate:
                    if (buttonDown && isMouseTracking)
                    {
                        var accumTraverseTranslation = Matrix.Translation(accumTraverseVector);
                        camera.ViewMatrix = Matrix.Invert(accumTraverseTranslation) * camera.ViewMatrix;                                                                
                        //Capture translation of camera.
                        Matrix cameraTranslation = Matrix.Translation(camera.ViewMatrix.TranslationVector);
                        Matrix cameraTranslationInverse = Matrix.Invert(cameraTranslation);
                        Matrix platformTranslationLocal = Matrix.Translation(Vector3.TransformCoordinate(platformLocation, cameraNode.ViewMatrix * cameraTranslationInverse));
                        //if the mouse movement results in a rotation about the z-axis, we rotate the camera around it's own z-axis (view-space z-axis).
                        //We accomplish this by right multiplying the rotation about z before we re-apply the translations.
                        if (rotateAboutZ)
                        {
                            Vector2 zRotationOrigin = new Vector2(navigationAreaSize.Width / 2, navigationAreaSize.Height / 2);
                            Vector2 zLocationVector = new Vector2(location.X, location.Y) - zRotationOrigin;
                            Vector2 zMouseMoveFromVector = new Vector2(mouseMoveFrom.X, mouseMoveFrom.Y) - zRotationOrigin;
                            zLocationVector.Normalize();
                            zMouseMoveFromVector.Normalize();
                            double zRotation = Math.Acos(MathUtil.Clamp(Vector2.Dot(zLocationVector, zMouseMoveFromVector), -1.0f, 1.0f));
                            double diff = Math.Atan2(zMouseMoveFromVector.Y, zMouseMoveFromVector.X) - Math.Atan2(zLocationVector.Y, zLocationVector.X);
                            float spinDir = (diff > 0) ? 1 : -1;

                            camera.ViewMatrix = camera.ViewMatrix *
                                                cameraTranslationInverse *
                                                Matrix.Invert(platformTranslationLocal) *
                                                Matrix.RotationZ((float)zRotation * spinDir) *
                                                platformTranslationLocal *
                                                cameraTranslation;
                        }
                        //otherwise, the verticle mouse movement results in a revolution of the camera about an axis, v, which passes through
                        //the world origin and is parallel with the camera's (view-space) x-axis. We accomplish this 
                        //by right multiplying the rotation about x before we re-apply the translations.
                        else
                            camera.ViewMatrix = camera.ViewMatrix *
                                                cameraTranslationInverse *
                                                Matrix.Invert(platformTranslationLocal) *
                                                Matrix.RotationX(MathUtil.DegreesToRadians(mouseOffset.Y)) *
                                                platformTranslationLocal *
                                                cameraTranslation;

                        //if  mouse movement doesn't result in a rotation about the z-axis, we revolve the camera about the world's y-axis by
                        //left multiplying a rotation about y, after reapplying the translations above.
                        if (!rotateAboutZ)
                        {                            
                            if (yRotationDirection == 0)
                            {
                                var vectorEllipseCenterY = rotationRectangle.Y + (rotationRectangle.Height / 2);                            
                                //Moving the mouse left to right in the upper hemisphere of the rotation ellipse results in a clockwise rotation
                                //around world Y and counter-clockwise when moved right to left. Moving the mouse left to right in the lower
                                //hemisphere of the rotation ellipse results in a counter-clockwise rotation around world Y and clockwise rotation
                                //when moved right to left.
                                yRotationDirection = vectorEllipseCenterY < location.Y ? 1 : -1;
                                //We want to invert the direction of rotations when the camera is below the world's Y plane.                            
                                yRotationDirection*= camera.Location.Y > 0 ? 1 : -1;
                            }
                           
                            camera.ViewMatrix = Matrix.Invert(platformTranslation) * 
                                                Matrix.RotationY(MathUtil.DegreesToRadians(mouseOffset.X) * yRotationDirection) * 
                                                platformTranslation *
                                                camera.ViewMatrix;
                        }
                        
                        camera.ViewMatrix = accumTraverseTranslation * camera.ViewMatrix;
                        mouseMoveFrom = location;
                    }
                    break;
                case NavigationMode.PlaformTraverse:                    
                    if (buttonDown && isMouseTracking)
                    {                        
                        var platformPickFrom = ScenePicker.PickNodes(
                            Game, 
                            mouseMoveFrom.X, 
                            mouseMoveFrom.Y,
                            n => n.GameObject.SupportsCameraTraversing())
                            .GetClosest(camera.Location);
                        
                        Vector3 panVector = Vector3.Zero;
                        if (platformPickFrom != null)
                        {
                            var platfromPickTo = ScenePicker.PickNodes(Game, location.X, location.Y, n => n == platformPickFrom.Node).FirstOrDefault();
                            if (platfromPickTo != null)
                            {
                                var v = platfromPickTo.IntersectionPoint - platformPickFrom.IntersectionPoint;
                                panVector = v;
                                panVector = cameraNode.WorldToParentNormal(panVector);
                                camera.ViewMatrix = Matrix.Translation(panVector) * camera.ViewMatrix;
                                mouseMoveFrom = location;
                                accumTraverseVector += panVector;
                            }
                        }
                    }
                    break;                
                case NavigationMode.None:                    
                    break;
            }            
        }

        internal void NotifyReset()
        {
            accumTraverseVector = Vector3.Zero;            
        }

        private Vector3 accumTraverseVector = Vector3.Zero;

        public enum NavigationMode
        {
            None,
            PlatformRotate,           
            PlaformTraverse,
            Pan
        }
    }   
}
