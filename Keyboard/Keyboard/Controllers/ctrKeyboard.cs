using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Keyboard.Controllers;

namespace Keyboard
{
    public partial class ctrKeyboard : Form
    {
        
        public ctrKeyboard()
        {
            InitializeComponent();
        }

        private void ctrKeyboard_Load(object sender, EventArgs e)
        {
            colorsPalette keyboardColors = new colorsPalette();

            this.menuStrip1.Renderer = new alterateRenderer();
            this.label1.BackColor = keyboardColors.FontColor;
            this.label2.ForeColor = keyboardColors.FontColor;          
            this.BackColor = keyboardColors.AplicationBackground;
            this.menuStrip1.BackColor = keyboardColors.AplicationBackground;

            //For each button in form alter its colors
            var buttons = GetAllTypeControls(this, typeof(Button));
            foreach ( Button btn in buttons)
            {
                btn.ForeColor = keyboardColors.FontColor;
                btn.BackColor = keyboardColors.ButtonColor;
                btn.FlatAppearance.BorderColor = keyboardColors.ButtonBorder;
            }
        }

        public IEnumerable<Control> GetAllTypeControls(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllTypeControls(ctrl, type))
                .Concat(controls)
                .Where(c => c.GetType() == type);
        }
    }
}
