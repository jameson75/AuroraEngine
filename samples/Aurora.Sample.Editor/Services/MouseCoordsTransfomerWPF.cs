using System.Windows;
using CipherPark.Aurora.Core.Utils;

namespace Aurora.Sample.Editor.Services
{
    public class MouseCoordsTransfomerWPF : IMouseCoordsTransfomer
    {
        private readonly UIElement imageHost;

        public MouseCoordsTransfomerWPF(UIElement imageHost)
        {
            this.imageHost = imageHost;
        }

        public SharpDX.Point Transform(SharpDX.Point point)
        {
            var tPoint = imageHost.PointFromScreen(new Point(point.X, point.Y));
            return new SharpDX.Point((int)tPoint.X, (int)tPoint.Y);
        }
    }
}
