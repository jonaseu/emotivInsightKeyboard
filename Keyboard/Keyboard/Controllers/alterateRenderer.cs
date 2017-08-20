using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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

    class RoundButton : Button
    {
        protected override void OnResize(EventArgs e)
        {
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(new Rectangle(2, 2, this.Width - 5, this.Height - 5));
                this.Region = new Region(path);
            }
            base.OnResize(e);
        }
    }
}
