﻿using System;
using SharpDX;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.World;
using CipherPark.Aurora.Core.World.Scene;
using CipherPark.Aurora.Core.Extensions;
using Aurora.Core.Editor.Util;

namespace CipherPark.Aurora.Core.Services
{
    public class SceneModifierService
    {
        private IGameApp gameApp;
        private bool isMouseTracking;
        private Point mouseMoveFrom;
        private GameObjectSceneNode currentAdornmentRoot;
        private GameObjectSceneNode currentPickedNode;
        private bool isActive;

        public SceneModifierService(IGameApp gameApp, bool changeActivationOnDoubleTap)
        {
            this.gameApp = gameApp;
            ChangeActivationOnDoubleTap = changeActivationOnDoubleTap;
        }

        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (!value)
                {                    
                    EndMouseTracking();
                }
            }
        }

        public bool ChangeActivationOnDoubleTap { get; set; }

        public ModifierMode ModifierMode { get; set; }

        public TransformSpace SelectedObjectTransformSpace { get; set; }

        public GameObjectSceneNode PickedNode { get => currentPickedNode; }

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

        internal void NotifyMouseDoubleTap(Point location)
        {
            var pickedNode = MousePickNode(location.X, location.Y);
            if (!IsActive && pickedNode != null)
            {
                IsActive = true;
                UpdatePick(pickedNode);
                OnActivatedFromDoubleTap(true);
            }

            if (IsActive && pickedNode == null)
            {
                IsActive = false;
                UpdatePick(null);
                OnActivatedFromDoubleTap(false);
            }
        }

        public void ClearPick()
        {
            if (currentPickedNode != null)
            {
                RemoveModifierNodes(currentPickedNode);                
                currentPickedNode = null;
            }
        }

        public void UpdatePick(GameObjectSceneNode pickedNode)
        {
            if (pickedNode == currentPickedNode)
            {
                return;
            }

            if (currentPickedNode != null)
            {
                ClearPick();
            }

            if (pickedNode != null)
            {
                AttachAdornmentNodes(pickedNode);
            }

            currentPickedNode = pickedNode;

            OnPickedNodeChanged(pickedNode);
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
                
                var pickedNode = MousePickNode(mouseMoveFrom.X, mouseMoveFrom.Y);

                if (pickedNode != currentPickedNode)
                {
                    UpdatePick(pickedNode);
                }
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
            if (IsActive && buttonDown && currentPickedNode != null)
            {
                var cameraNode = gameApp.GetActiveCamera();        

                var pickFromRay = ScenePicker.GetPickRay(gameApp, mouseMoveFrom.X, mouseMoveFrom.Y);
                
                var pickToRay = ScenePicker.GetPickRay(gameApp, location.X, location.Y);

                var pickedNodeWorldPosition = currentPickedNode.WorldPosition();
                
                switch (ModifierMode)
                {
                    case ModifierMode.TransformObject:

                        var mouseOffsetXAbs = Math.Abs(location.X - mouseMoveFrom.X);
                        var mouseOffsetYAbs = Math.Abs(location.Y - mouseMoveFrom.Y);
                        var rotationDelta = 0f;                        
                        if (mouseOffsetXAbs > mouseOffsetYAbs)                        
                        {
                            rotationDelta = mouseOffsetXAbs * (location.X > mouseMoveFrom.X ? 1 : -1);
                        }
                        else
                        {
                            rotationDelta = mouseOffsetYAbs * (location.Y > mouseMoveFrom.Y ? 1 : -1);
                        }

                        switch (SelectedObjectTransformSpace)
                        {
                            case TransformSpace.ViewSpaceTranslateXZ:
                                var pickedNodeWorldAlignedYPlane = new Plane(pickedNodeWorldPosition, Vector3.Up);
                                bool fromRayIntersectsPlane = pickFromRay.Intersects(ref pickedNodeWorldAlignedYPlane, out Vector3 fromIntersectionPoint);
                                var toRayIntersectsPlane = pickToRay.Intersects(ref pickedNodeWorldAlignedYPlane, out Vector3 toIntersectionPoint);
                                if (fromRayIntersectsPlane && toRayIntersectsPlane)
                                {
                                    var nodeTranslationVector = currentPickedNode.WorldToParentCoordinate(toIntersectionPoint) -
                                                                currentPickedNode.WorldToParentCoordinate(fromIntersectionPoint);
                                    currentPickedNode.Translate(nodeTranslationVector);
                                    OnNodeTransformed(currentPickedNode);
                                }
                                break;
                            case TransformSpace.ViewSpaceTranslateY:
                                var cameraAlignedZPlaneNormal = Vector3.Normalize(cameraNode.WorldPosition() - pickedNodeWorldPosition);
                                var xAxis = Vector3.Normalize(Vector3.Cross(cameraAlignedZPlaneNormal, Vector3.Up));
                                var objectAlignedCameraSpaceZPlaneNormal = Vector3.Normalize(Vector3.Cross(Vector3.Up, xAxis));
                                var pickedNodeCameraAlignedZPlane = new Plane(pickedNodeWorldPosition, objectAlignedCameraSpaceZPlaneNormal);
                                fromRayIntersectsPlane = pickFromRay.Intersects(ref pickedNodeCameraAlignedZPlane, out fromIntersectionPoint);
                                toRayIntersectsPlane = pickToRay.Intersects(ref pickedNodeCameraAlignedZPlane, out toIntersectionPoint);
                                if (fromRayIntersectsPlane && toRayIntersectsPlane)
                                {
                                    var nodeTranslationVector = currentPickedNode.WorldToParentCoordinate(toIntersectionPoint) -
                                                                currentPickedNode.WorldToParentCoordinate(fromIntersectionPoint);
                                    currentPickedNode.Translate(new Vector3(0, nodeTranslationVector.Y, 0));
                                    OnNodeTransformed(currentPickedNode);
                                }
                                break;
                            case TransformSpace.ParentSpaceRevolveX:
                                if (currentPickedNode.IsSatelliteNode())
                                {                                   
                                    var axis = Vector3.Transform(
                                        currentPickedNode.WorldToParentNormal(
                                            currentPickedNode.ParentToWorldNormal(
                                                Vector3.Normalize(currentPickedNode.Parent.Transform.ToMatrix().Right))),
                                        currentPickedNode.Transform.Rotation);                                                                                                        
                                    
                                    var rotation = Quaternion.RotationAxis(axis, MathUtil.DegreesToRadians(rotationDelta));
                                    currentPickedNode.Rotate(rotation);

                                    var translation = Vector3.Transform(currentPickedNode.Transform.Translation, rotation);
                                    currentPickedNode.TranslateTo(translation);                                    
                                }
                                break;
                            case TransformSpace.ParentSpaceRevolveY:
                                if (currentPickedNode.IsSatelliteNode())
                                {
                                    var axis = currentPickedNode.WorldToParentNormal(
                                        currentPickedNode.ParentToWorldNormal(
                                            Vector3.Normalize(currentPickedNode.Parent.Transform.ToMatrix().Up)));

                                    var positionOrientation = Quaternion.RotationAxis(axis, MathUtil.DegreesToRadians(rotationDelta));
                                    var newPosition = Vector3.Transform(currentPickedNode.Transform.Translation, positionOrientation);
                                    currentPickedNode.TranslateTo(newPosition);                                    
                                    var orientationRotation = Quaternion.RotationAxis(axis, MathUtil.DegreesToRadians(rotationDelta));
                                    currentPickedNode.Rotate(orientationRotation);
                                }
                                break;
                            case TransformSpace.ParentSpaceRevolveZ:
                                if (currentPickedNode.IsSatelliteNode())
                                {
                                    var axis = Vector3.Normalize(currentPickedNode.Transform.ToMatrix().Forward);
                                    var rotation = Quaternion.RotationAxis(axis, MathUtil.DegreesToRadians(rotationDelta));
                                    currentPickedNode.Rotate(rotation);
                                }
                                break;                           
                            case TransformSpace.LocalSpaceRotateX:                                
                                currentPickedNode.Rotate(currentPickedNode.Transform.ToMatrix().Right, MathUtil.DegreesToRadians(rotationDelta));
                                break;
                            case TransformSpace.LocalSpaceRotateY:
                                currentPickedNode.Rotate(currentPickedNode.Transform.ToMatrix().Up, MathUtil.DegreesToRadians(rotationDelta));
                                break;
                            case TransformSpace.LocalSpaceRotateZ:
                                currentPickedNode.Rotate(currentPickedNode.Transform.ToMatrix().Forward, MathUtil.DegreesToRadians(rotationDelta));                                
                                break;
                            case TransformSpace.OrbitDistanceTranslate:
                                if (currentPickedNode.IsSatelliteNode())
                                {
                                    //TODO: Move orbital distance of node.
                                }
                                break;
                        }
                        break;        
                }
            }            

            mouseMoveFrom = location;
        }       

        private GameObjectSceneNode MousePickNode(int mouseX, int mouseY)
        {
            var cameraNode = gameApp.GetActiveCamera();
            var camera = cameraNode.Camera;

            return ScenePicker.PickNodes(
                        gameApp,
                        mouseX,
                        mouseY,
                        n => n.GameObject.IsPickableObject())
                        .GetClosest(camera.Location)
                        ?.Node;
        }

        private void AttachAdornmentNodes(GameObjectSceneNode targetNode)
        {
            if (currentAdornmentRoot != null)
            {
                throw new InvalidOperationException("Modifier root node already exists.");
            }            
            
            //Add modifier root node to target node...
            currentAdornmentRoot = new GameObjectSceneNode(gameApp)
            {
                GameObject = new GameObject(gameApp, new object[] {  
                    new EditorObjectContext
                    {
                        IsAdornmentRoot = true,
                        TargetNode = targetNode,
                    }
                })
            };
            targetNode.Children.Add(currentAdornmentRoot);

            //Add selection node to modifier root node...
            currentAdornmentRoot.Children.Add(new GameObjectSceneNode(gameApp)
            {
                GameObject = new GameObject(gameApp, new object[] 
                {
                    new EditorObjectContext
                    {
                        IsSelectionAdornment = true,
                        TargetNode = targetNode,
                    }
                })
                {
                    Renderer = new GameObjectSelectionRenderer(gameApp, targetNode.GameObject),
                },               
            });

            //Add shadow modifier
            currentAdornmentRoot.Children.Add(new GameObjectSceneNode(gameApp)
            {
                GameObject = new GameObject(gameApp, new object[]
                {
                    new EditorObjectContext
                    {
                        IsShadowAdronment = true,
                        TargetNode = targetNode,
                    }
                })
                {
                    Renderer = new GameObjectShadowRenderer(gameApp, targetNode),
                },
            });
        }

        private void RemoveModifierNodes(GameObjectSceneNode targetNode)            
        {
            if (currentAdornmentRoot != null)
            {
                targetNode.Children.Remove(currentAdornmentRoot);
                currentAdornmentRoot.Dispose(true);                
                currentAdornmentRoot = null;              
            }
        }

        private void OnNodeTransformed(GameObjectSceneNode transformedNode)
        {
            NodeTransformed?.Invoke(this, new NodeTransformedArgs(transformedNode));
        }

        private void OnPickedNodeChanged(GameObjectSceneNode pickedNode)
        {
            PickedNodeChanged?.Invoke(this, new PickedNodeChangedArgs(pickedNode));
        }

        private void OnActivatedFromDoubleTap(bool activated)
        {
            ActivatedOnDoubleTap?.Invoke(this, new ActivatedOnDoubleTapArgs(activated));
        }

        public event Action<object, NodeTransformedArgs> NodeTransformed;

        public event Action<object, PickedNodeChangedArgs> PickedNodeChanged;

        public event Action<object, ActivatedOnDoubleTapArgs> ActivatedOnDoubleTap;
    }

    public class NodeTransformedArgs : EventArgs
    {
        public NodeTransformedArgs(GameObjectSceneNode transformedNode)
        {
            TransfromedNode = transformedNode;
        }

        public GameObjectSceneNode TransfromedNode { get; }
    }

    public class PickedNodeChangedArgs : EventArgs
    {
        public PickedNodeChangedArgs(GameObjectSceneNode pickedNode)
        {
            PickedNode = pickedNode;
        }

        public GameObjectSceneNode PickedNode { get; }
    }

    public class ActivatedOnDoubleTapArgs : EventArgs
    {
        public ActivatedOnDoubleTapArgs(bool activated)
        {
            Activated = activated;
        }

        public bool Activated { get; }
    }

    public enum TransformSpace
    {
        None,
        ViewSpaceTranslateXZ,
        ViewSpaceTranslateY,
        ParentSpaceRevolveX,
        ParentSpaceRevolveY,
        ParentSpaceRevolveZ,
        LocalSpaceRotateX,
        LocalSpaceRotateY,
        LocalSpaceRotateZ,
        OrbitDistanceTranslate,
    }

    public enum ModifierMode
    {
        TransformObject,
        Rotate,
        Scale
    }
}
