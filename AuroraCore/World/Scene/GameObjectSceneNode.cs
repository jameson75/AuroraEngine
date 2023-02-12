namespace CipherPark.Aurora.Core.World.Scene
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
                }
                else
                {
                    _gameObject = value;

                    if (_gameObject.ContainerNode != this)                    
                        _gameObject.ContainerNode = this;
                }
            }
        }        

        public GameObjectSceneNode(IGameApp game, string name = null) : base(game, name)
        { }
     

        public override void Update(GameTime gameTime)
        {
            GameObject.Update(gameTime);
            base.Update(gameTime);            
        }

        public override void Draw()
        {
            GameObject.Draw();
            base.Draw();            
        }

        protected override void OnDispose()
        {
            if (GameObject != null)
                GameObject.Dispose();
            base.OnDispose();
        }
    }
}