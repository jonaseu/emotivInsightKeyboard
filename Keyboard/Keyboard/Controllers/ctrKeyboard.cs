using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using Keyboard.Business_Rules;
using Keyboard.Controllers;

namespace Keyboard
{
    public partial class ctrKeyboard : Form
    {
        public string IpToConnect { get; set; }
        public int Sensibility { get; set; }
        public string ClickMode { get; set; }

        private rulKeyboard _rule = null;
        
        public ctrKeyboard()
        {
            IpToConnect = "127.9.0.1";
            Sensibility = 3;
            ClickMode = "Blink";

            InitializeComponent();
            _rule = new rulKeyboard(this);
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
            var buttons = misc.GetAllTypeControls(this, typeof(Button));
            foreach (var control in buttons)
            {
                var btn = (Button)control;
                btn.ForeColor = colorsPalette.FontColor;
                btn.BackColor = colorsPalette.ButtonColor;
                btn.FlatAppearance.BorderColor = colorsPalette.ButtonBorder;
                btn.Margin = new Padding(3,4,3,4);
            }
            _rule.BeginAlternateLines(1000);
        }

        //Returns all controls of a given type
        
        private void ctrKeyboard_Resize(object sender, EventArgs e)
        {
            var buttons = misc.GetAllTypeControls(this, typeof(Button));
            foreach (var control in buttons)
            {
                var btn = (Button)control;
                btn.Font = new Font(btn.Font.FontFamily, Width / 60);
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _rule.activateKey();
        }

        public void SwitchLineColor(int lineNumber)
        {
            var line = (TableLayoutPanel)keyboardKeys.GetControlFromPosition(0, lineNumber);
            line.BackColor = (line.BackColor != colorsPalette.HighlightColor ? 
                colorsPalette.HighlightColor :
                colorsPalette.AplicationBackground);
        }

        public void SwitchColumnColor(int lineNumber, int columnNumber)
        {

            var line = (TableLayoutPanel)keyboardKeys.GetControlFromPosition(0, lineNumber);
            var btn = line.GetControlFromPosition(columnNumber, 0);
            
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

        public void ButtonClicked(int row, int coloumn)
        {
            var line = (TableLayoutPanel)keyboardKeys.GetControlFromPosition(0, row);
            var btn = line.GetControlFromPosition(coloumn, 0);
            label1.Text += btn.Text;
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new ctrOptions(this)).ShowDialog();
        }
    }
}
