using System.Drawing;
using System.Windows.Forms;

namespace Keyboard.Controllers
{
        class alterateRenderer : ToolStripProfessionalRenderer
        {
            public alterateRenderer() : base(new MyColors()) { }
        }
        class MyColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected
            {
                get { return Color.DarkCyan; }
            }
            public override Color MenuItemSelectedGradientBegin
            {
                get { return Color.FromArgb(98, 98, 98); }
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return Color.FromArgb(78, 78, 78); }
            }
            public override Color MenuItemBorder
            {
                get { return Color.Transparent; }
            }

    }
}
