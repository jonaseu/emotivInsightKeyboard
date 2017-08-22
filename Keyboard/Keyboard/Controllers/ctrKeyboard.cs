using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Keyboard.Business_Rules;
using Keyboard.Controllers;

namespace Keyboard
{
    public partial class ctrKeyboard : Form
    {
        
        rulKeyboard _rule = null;

        public ctrKeyboard()
        {
            InitializeComponent();
            _rule = new rulKeyboard(this);
            _rule.TimeInterval = 5000;
            _rule.BeginAlternateLines();
        }

        private void ctrKeyboard_Load(object sender, EventArgs e)
        {
            menuStrip1.Renderer = new alterateRenderer();
            label1.BackColor = colorsPalette.FontColor;
            label2.ForeColor = colorsPalette.FontColor;          
            BackColor = colorsPalette.AplicationBackground;
            menuStrip1.BackColor = colorsPalette.AplicationBackground;
            connectToolStripMenuItem.ForeColor = colorsPalette.FontColor;
            optionsToolStripMenuItem.ForeColor = colorsPalette.FontColor; 
            
            //For each button in form alter its colors
            var buttons = GetAllTypeControls(this, typeof(Button));
            foreach ( Button btn in buttons)
            {
                btn.ForeColor = colorsPalette.FontColor;
                btn.BackColor = colorsPalette.ButtonColor;
                btn.FlatAppearance.BorderColor = colorsPalette.ButtonBorder;
            }
        }

        //Returns all controls of a given type
        private IEnumerable<Control> GetAllTypeControls(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllTypeControls(ctrl, type))
                .Concat(controls)
                .Where(c => c.GetType() == type);
        }

        public void changeLineColor(int lineNumber)
        {
            switch (lineNumber)
            {
                case 1:
                    if (keyboardLine1.BackColor != colorsPalette.HighlightColor)
                        keyboardLine1.BackColor = colorsPalette.HighlightColor;
                    else
                        keyboardLine1.BackColor = colorsPalette.AplicationBackground;
                    return;
                case 2:
                    if (keyboardLine2.BackColor != colorsPalette.HighlightColor)
                        keyboardLine2.BackColor = colorsPalette.HighlightColor;
                    else
                        keyboardLine2.BackColor = colorsPalette.AplicationBackground;
                    return;
                case 3:
                    if (keyboardLine3.BackColor != colorsPalette.HighlightColor)
                        keyboardLine3.BackColor = colorsPalette.HighlightColor;
                    else
                        keyboardLine3.BackColor = colorsPalette.AplicationBackground;
                    return;
                case 4:
                    if (keyboardLine4.BackColor != colorsPalette.HighlightColor)
                        keyboardLine4.BackColor = colorsPalette.HighlightColor;
                    else
                        keyboardLine4.BackColor = colorsPalette.AplicationBackground;
                    return;
                case 5:
                    if (keyboardLine5.BackColor != colorsPalette.HighlightColor)
                        keyboardLine5.BackColor = colorsPalette.HighlightColor;
                    else
                        keyboardLine5.BackColor = colorsPalette.AplicationBackground;
                    return;
            }
        }

        public void changeColumnColor(int lineNumber, int columnNumber)
        {
            var btn = keyboardLine1.GetControlFromPosition(columnNumber-1,0);
            if (btn.BackColor != colorsPalette.HighlightColor)
            {
                btn.BackColor = colorsPalette.HighlightColor;
                btn.ForeColor = colorsPalette.ButtonColor;
            }
            else
            {
                btn.BackColor = colorsPalette.ButtonColor;
                btn.ForeColor = colorsPalette.FontColor;
            }
        }
        
        private void ctrKeyboard_Resize(object sender, EventArgs e)
        {
            var buttons = GetAllTypeControls(this, typeof(Button));
            foreach (Button btn in buttons)
            {
                btn.Font = new Font(btn.Font.FontFamily, Width/60);
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _rule.ShouldBlink = !_rule.ShouldBlink;
        }
    }
}
