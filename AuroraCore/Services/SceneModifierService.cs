using System;
using SharpDX;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Scene;

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
                
            }
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
    }

    public enum GameObjectModifierMode
    {
        None,
        Rotate,
        Translate,
        Scale
    }
}
