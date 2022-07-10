using SharpDX;

namespace CipherPark.Aurora.Core.Services
{
    public class MouseTrackingService
    {
        private Point? mouseLocation;

        public MouseTrackingService(IGameApp gameApp)
        {
            GameApp = gameApp;
        }

        private IGameApp GameApp { get; }

        public void UpdateMouse(Point location)
        {
            mouseLocation = location;
        }

        public Point? GetMouseLocation()
        {
            return mouseLocation;
        }
    }
}
