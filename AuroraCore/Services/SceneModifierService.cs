using System;
using SharpDX;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Animation;
using System.Linq;

namespace CipherPark.Aurora.Core.Services
{
    public class SceneModifierService
    {
        private IGameApp gameApp;
        private bool isMouseTracking;
        private Point mouseMoveFrom;
        private GameObjectSceneNode currentPickedNode;
        private GameObjectSceneNode currentModifierNode;
        private bool isPickingModeActive;

        public SceneModifierService(IGameApp gameApp)
        {
            this.gameApp = gameApp;
        }

        public bool IsActive
        {
            get { return isPickingModeActive; }
            set
            {
                isPickingModeActive = value;
                if (!value)
                {                    
                    EndMouseTracking();
                }
            }
        }

        public TransformMode TransformMode { get; set; }

        public TranslationPlane TranslationPlane { get; set; }

        public void NotifyMouseDown(Point location)
        {
            OnMouseDown(location);
        }

        public void NotifyMouseUp(Point location)
        {
            OnMouseUp(location);
        }

        public void NotifyMouseMove(bool buttonDown, Point location)
        {
            OnMouseMove(buttonDown, location);
        }

        public void ClearPick()
        {
            if (currentPickedNode != null)
            {
                RemoveModifierNode(currentPickedNode);                
                currentPickedNode = null;
            }
        }

        private void BeginMouseTracking(Point location)
        {
            isMouseTracking = true;
            mouseMoveFrom = location;
        }

        private void EndMouseTracking()
        {
            isMouseTracking = false;
            mouseMoveFrom = new Point(0, 0);
        }

        private void OnMouseDown(Point location)
        {
            if (IsActive)
            {
                if (!isMouseTracking)
                {
                    BeginMouseTracking(location);
                }
                
                UpdatePick();
            }
        }

        private void OnMouseUp(Point location)
        {           
            if (isMouseTracking)
            {
                EndMouseTracking();
            }
        }

        private void OnMouseMove(bool buttonDown, Point location)
        {
            if (currentPickedNode != null)
            {
                if (buttonDown)
                {
                    var cameraNode = gameApp.GetActiveScene().CameraNode;

                    var pickFromInfo = ScenePicker.PickNodes(
                                            gameApp,
                                            mouseMoveFrom.X,
                                            mouseMoveFrom.Y,
                                            n => n == currentPickedNode).FirstOrDefault();

                    var pickToInfo = ScenePicker.PickNodes(
                                            gameApp,
                                            location.X,
                                            location.Y,
                                            n => n == currentPickedNode).FirstOrDefault();

                    var pickedNodeWorldPosition = currentPickedNode.WorldPosition();

                    if (pickFromInfo != null && pickToInfo != null)
                    {
                        switch (TransformMode)
                        {
                            case TransformMode.Translate:
                                switch (TranslationPlane)
                                {
                                    case TranslationPlane.XZ:
                                        var pickedNodeWorldAlignedYPlane = new Plane(pickedNodeWorldPosition, Vector3.Up);
                                        bool fromRayIntersectsPlane = pickFromInfo.Ray.Intersects(ref pickedNodeWorldAlignedYPlane, out Vector3 fromIntersectionPoint);
                                        bool toRayIntersectsPlane = pickToInfo.Ray.Intersects(ref pickedNodeWorldAlignedYPlane, out Vector3 toIntersectionPoint);
                                        if (fromRayIntersectsPlane && toRayIntersectsPlane)
                                        {
                                            var nodeTranslationVector = currentPickedNode.WorldToLocalCoordinate(toIntersectionPoint) - 
                                                                        currentPickedNode.WorldToLocalCoordinate(fromIntersectionPoint);
                                            currentPickedNode.Translate(nodeTranslationVector);
                                            OnNodeTransformed(currentPickedNode);
                                        }
                                        break;
                                    case TranslationPlane.XY:
                                        var cameraAlignedZPlaneNormal = Vector3.Normalize(cameraNode.WorldPosition() - pickedNodeWorldPosition);
                                        var pickedNodeCameraAlignedZPlane = new Plane(pickedNodeWorldPosition, cameraAlignedZPlaneNormal);
                                        fromRayIntersectsPlane = pickFromInfo.Ray.Intersects(ref pickedNodeCameraAlignedZPlane, out fromIntersectionPoint);
                                        toRayIntersectsPlane = pickToInfo.Ray.Intersects(ref pickedNodeCameraAlignedZPlane, out toIntersectionPoint);
                                        if (fromRayIntersectsPlane && toRayIntersectsPlane)
                                        {
                                            var nodeTranslationVector = currentPickedNode.WorldToLocalCoordinate(toIntersectionPoint) -
                                                                        currentPickedNode.WorldToLocalCoordinate(fromIntersectionPoint);
                                            currentPickedNode.Translate(nodeTranslationVector);
                                            OnNodeTransformed(currentPickedNode);
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                }
            }

            mouseMoveFrom = location;
        }        

        private void UpdatePick()
        {
            var cameraNode = gameApp.GetActiveScene().CameraNode;
            var camera = cameraNode.Camera;

            var pickedNode = ScenePicker.PickNodes(
                        gameApp,
                        mouseMoveFrom.X,
                        mouseMoveFrom.Y,
                        n => n.GameObject.IsEditorObject() == false)
                        .GetClosest(camera.Location)
                        ?.Node;

            if (pickedNode != currentPickedNode)
            {
                if (currentPickedNode != null)
                {
                    ClearPick();
                }

                if (pickedNode != null)
                {
                    AttachModifierNode(pickedNode);
                }

                currentPickedNode = pickedNode;
            }
        }

        private void AttachModifierNode(GameObjectSceneNode targetNode)
        {
            if (currentModifierNode != null)
            {
                throw new InvalidOperationException("Modifier node already exists.");
            }

            var modifierContext = new EditorObjectContext
            {
                IsModifier = true,
                ModifierTargetNode = targetNode,
            };

            var modifierRenderer = new GameObjectModifierRenderer(gameApp, modifierContext);
            
            currentModifierNode = new GameObjectSceneNode(gameApp)
            {
                GameObject = new GameObject(gameApp, new object[] { modifierContext })
                {
                    Renderer = modifierRenderer,
                },               
            };

            targetNode.Children.Add(currentModifierNode);
        }

        private void RemoveModifierNode(GameObjectSceneNode targetNode)            
        {
            if (currentModifierNode != null)
            {
                targetNode.Children.Remove(currentModifierNode);
                currentModifierNode.Dispose();
                currentModifierNode = null;
            }
        }

        private void OnNodeTransformed(GameObjectSceneNode transformedNode)
        {
            NodeTransformed?.Invoke(this, new NodeTransformedArgs(transformedNode));
        }

        public event Action<object, NodeTransformedArgs> NodeTransformed;
    }

    public class NodeTransformedArgs : EventArgs
    {
        public NodeTransformedArgs(SceneNode transformedNode)
        {
            TransfromedNode = transformedNode;
        }

        public SceneNode TransfromedNode { get; }
    }

    public enum TranslationPlane
    {        
        XZ,
        XY
    }

    public enum TransformMode
    {
        Translate,
        Rotate,
        Scale
    }
}
