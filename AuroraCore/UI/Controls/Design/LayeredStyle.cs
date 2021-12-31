using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CipherPark.Aurora.Core.UI.Controls;

namespace CipherPark.Aurora.Core.UI.Components
{
    /*
    public class LayeredStyle : UIStyle
    {
        private List<UIStyle> _styles = new List<UIStyle>();

        public List<UIStyle> Styles { get { return _styles; } }

        public LayeredStyle()
        { }

        public LayeredStyle(IEnumerable<UIStyle> styles)
        {
            Styles.AddRange(styles);
        }

        public override Controls.UIContent GenerateContent()
        {
            UIContent[] childContent = Styles.Select(s => s.GenerateContent()).ToArray();
            return new LayeredContent(childContent);           
        }
    }
    */
}
