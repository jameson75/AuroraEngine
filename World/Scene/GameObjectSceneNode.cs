namespace CipherPark.KillScript.Core.World.Scene
{
    public class GameObjectSceneNode : SceneNode
    {
        GameObject _gameObject = null;

        public GameObject GameObject
        {
            get { return _gameObject; }
            set
            {
                if (value == null)
                {
                    var g = _gameObject;
                    _gameObject = null;

                    if (g.ContainerNode == this)                    
                        g.ContainerNode = null;

                    /*
                    if (g.TransformableParent == this)
                        g.TransformableParent = null;                    
                    */
                }
                else
                {
                    _gameObject = value;

                    if (_gameObject.ContainerNode != this)                    
                        _gameObject.ContainerNode = this;

                    /*
                    if (_gameObject.TransformableParent != this)
                        _gameObject.TransformableParent = this;                    
                    */
                }
            }
        }

        public GameObjectSceneNode(IGameApp game, string name = null) : base(game, name)
        { }
     

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            GameObject.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
            GameObject.Draw();
        }

        protected override void OnDispose()
        {
            if (GameObject != null)
                GameObject.Dispose();
            base.OnDispose();
        }
    }
}