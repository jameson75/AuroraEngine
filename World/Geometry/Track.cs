using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Kinetics;
using CipherPark.AngelJacket.Core.World.Renderers;
using CipherPark.AngelJacket.Core.Utils;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Content;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    /// <summary>
    /// 
    /// </summary>
    public class TrackSection : ITransformable
    {
        public TrackSection()
        {
            _items.CollectionChanged += Items_CollectionChanged;
        }        

        private ObservableCollection<Form> _items = new ObservableCollection<Form>();

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<Form> Items { get { return _items; } }

        /// <summary>
        /// 
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ITransformable TransformableParent { get; set; }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {            
            foreach (Form item in _items)
                item.Draw(gameTime);          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (Form item in e.OldItems)
                    item.TransformableParent = null;

            if (e.NewItems != null)
                foreach (Form item in e.NewItems)
                    item.TransformableParent = this;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TrackSectionInstance : ITransformable
    {
        /// <summary>
        /// 
        /// </summary>
        public TrackSection Section { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ITransformable TransformableParent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            //Save the original state of the section's spatial data.
            Transform originalTransform = Section.Transform;
            ITransformable originalParent = Section.TransformableParent;
            //Set section's spatial data to this instance.
            Section.Transform = this.Transform;
            Section.TransformableParent = this.TransformableParent;
            //Render section.
            Section.Draw(gameTime);
            //Set section spatial data back to its orignal state.
            Section.Transform = originalTransform;
            Section.TransformableParent = originalParent;            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Track : ITransformable
    {
        private IGameApp _game = null;
        private List<TrackSectionInstance> _trackLayout = new List<TrackSectionInstance>();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        public Track(IGameApp game)
        {
            _game = game;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public IGameApp Game { get { return _game; } }

        /// <summary>
        /// 
        /// </summary>
        public List<TrackSectionInstance> TrackLayout { get { return _trackLayout; } }

        /// <summary>
        /// 
        /// </summary>
        public Path PathOfAction { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ITransformable TransformableParent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ITransformable FrameOfAction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sections"></param>
        /// <param name="sequence"></param>
        public void GenerateLayout(IEnumerable<TrackSection> sections, IEnumerable<int> sequence)
        {
            if(PathOfAction == null)
                throw new InvalidOperationException("Path of action property not initialized.");

            _trackLayout.Clear();
            for (int i = 0; i < sequence.Count(); i++)
            {
                float SectionLength = 1000.0f; //TODO: Figure out a way to parameterize this value.
                PathNode pathNode = PathOfAction.EvaluateNodeAtDistance(i * SectionLength);
                Transform instanceTransform = pathNode.Transform;
                _trackLayout.Add(new TrackSectionInstance()
                {
                    Section = sections.ElementAt(sequence.ElementAt(i)),
                    TransformableParent = this,
                    //TODO: Use action path to calculate transform.
                    Transform = instanceTransform
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            foreach (TrackSectionInstance instance in _trackLayout)
                instance.Draw(gameTime);
        }       
    }

    /// <summary>
    /// 
    /// </summary>
    public class TrackNode : Scene.SceneNode
    {
        Track _track = null;

        public TrackNode(Track track, string name = null)
            : base(track.Game, name)
        {
            _track = track;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            //TODO: Change design so that Form.Draw() implementations aquire the Camera from a IViewportService
            //and set the view and projecton matrices themselves.

            foreach( TrackSectionInstance sectionInstance in _track.TrackLayout)
                foreach(Form item in sectionInstance.Section.Items)
                {
                    item.ElementEffect.View = Camera.TransformToViewMatrix(Scene.CameraNode.ParentToWorld(Scene.CameraNode.Transform)); //ViewMatrix;
                    item.ElementEffect.Projection = Scene.CameraNode.Camera.ProjectionMatrix;                        
                }
            _track.Draw(gameTime);

            base.Draw(gameTime);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class EnumerableExtension
    {
        public static void RemoveAll<T>(this IList<T> data, System.Func<T, bool> selector)
        {
            var itemsToDelete = data.Where(selector);
            foreach (var d in data)
                data.Remove(d);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ObservableCollectionExtension
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> data)
        {
            foreach (T d in data)
                collection.Add(d);
        }
    }
}
