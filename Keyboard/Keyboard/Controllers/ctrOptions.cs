using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Keyboard.Business_Rules;

namespace Keyboard.Controllers
{
    public partial class ctrOptions : Form
    {
        private rulOptions _rul = null;
        private ctrKeyboard _form = null;

        public ctrOptions(ctrKeyboard form)
        {
            _form = form;
            InitializeComponent();
        }

        private void ctrOptions_Load(object sender, EventArgs e)
        {

            if(_form.ClickMode == "Blink")
                comboBox1.SelectedIndex = 0;
            else
                comboBox1.SelectedIndex = 1;
            richTextBox1.Text = _form.IpToConnect;
            numericUpDown1.Value = _form.Sensibility;

            BackColor = colorsPalette.AplicationBackground;
            var labels = misc.GetAllTypeControls(this, typeof(Label));
            foreach (var control in labels)
            {
                var btn = (Label)control;
                btn.ForeColor = colorsPalette.FontColor;
                btn.Margin = new Padding(20,0,20,0);
            }

            var buttons = misc.GetAllTypeControls(this, typeof(Button));
            foreach (var control in buttons)
            {
                var btn = (Button)control;
                btn.ForeColor = colorsPalette.FontColor;
                btn.BackColor = colorsPalette.ButtonColor;
                btn.FlatAppearance.BorderColor = colorsPalette.ButtonBorder;
                btn.Margin = new Padding(3, 4, 3, 4);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (misc.ValidateIPv4(richTextBox1.Text))
            {
                _form.ClickMode = comboBox1.Text;
                _form.IpToConnect = richTextBox1.Text;
                _form.Sensibility = (int)numericUpDown1.Value;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid IP value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}
