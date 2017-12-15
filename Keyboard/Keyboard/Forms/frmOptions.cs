using System;
using System.Diagnostics;
using System.Windows.Forms;
using Keyboard.Properties;

namespace Keyboard.Controllers
{
    public partial class frmOptions : Form
    {
        private frmKeyboard _form = null;

        public frmOptions(frmKeyboard form)
        {
            _form = form;
            InitializeComponent();
        }

        private void ctrOptions_Load(object sender, EventArgs e)
        {

            if(_form.ClickMode == "Raise Eyebrows")
                comboBox1.SelectedIndex = 0;
            else
                comboBox1.SelectedIndex = 1;
            
            richTextBox1.Text = _form.IpToConnect;
            numericUpDown1.Value = _form.ClickSpeed;
            numericUpDown2.Value = _form.Interval;
            numericUpDown3.Text = _form.PortToConnect.ToString();

            BackColor = colorsPalette.AplicationBackground;
            var labels = misc.GetAllTypeControls(this, typeof(Label));
            foreach (var control in labels)
            {
                var btn = (Label)control;
                btn.ForeColor = colorsPalette.FontColor;
                btn.Margin = new Padding(10,0,20,0);
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
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (misc.ValidateIPv4(richTextBox1.Text))
            {
                _form.Interval = (int)numericUpDown2.Value;
                _form.ClickSpeed = (int)numericUpDown1.Value;
                _form.ClickMode = comboBox1.Text;
                _form.IpToConnect = richTextBox1.Text;
                _form.PortToConnect = Int32.Parse(numericUpDown3.Text);
                Close();
                _form.Disconnect();
            }
            else
            {
                MessageBox.Show(@"Invalid IP value", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(
                    @"C:\Program Files (x86)\Emotiv Xavier ControlPanel v3.3.3\Applications\EmotivXavierControlpanel.exe");

            }
            catch (Exception ex)
            {
            }
        }
    }
}
