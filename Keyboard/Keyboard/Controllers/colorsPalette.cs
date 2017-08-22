using System.Drawing;
using System.Windows.Forms;

namespace Keyboard.Controllers
{
    class colorsPalette
    {
        public Color AplicationBackground 
        {
            get { return System.Drawing.Color.FromArgb(28, 28, 28); }
        }
        public Color ButtonColor
        {
            get { return System.Drawing.Color.FromArgb(38, 38, 38); }
        }
        public Color ButtonBorder
        {
            get { return System.Drawing.Color.FromArgb(58, 58, 58); }
        } 

        public Color FontColor
        {
            get { return System.Drawing.Color.FromArgb(197, 199, 199); }
        }

    }

    class menuColors : ProfessionalColorTable
    {
        public override Color MenuItemSelected
        {
            get { return Color.DarkCyan; }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get { return Color.FromArgb(68, 68, 68); }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.Transparent; }
        }
        public override Color MenuItemBorder
        {
            get { return Color.Black; }
        }
    }
}
