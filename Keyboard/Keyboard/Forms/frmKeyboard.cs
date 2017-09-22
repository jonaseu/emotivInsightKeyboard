using System;
using System.Drawing;
using System.Windows.Forms;
using Keyboard.Business_Rules;


namespace Keyboard.Controllers
{
    public partial class frmKeyboard : Form
    {
        public string IpToConnect { get; set; }
        public string ClickMode { get; set; }
        public int Sensitivity { get; set; }
        public int Interval { get; set; }
        public int PortToConnect { get; set; }

        private readonly rulKeyboard _rule;
        private bool _connected;

        //Override to disable form as activable
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ExStyle |= 0x08000000;
                return param;
            }
        }

        public frmKeyboard()
        {
            IpToConnect = "127.0.0.1";
            Sensitivity = 1;
            ClickMode = "Blink";
            Interval = 1000;
            PortToConnect = 1900;

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
            clearToolStripMenuItem.ForeColor = colorsPalette.FontColor;

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
        }
        
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
            if(!_connected)
            {
                connectToolStripMenuItem.Text = @"Connecting";
                Refresh();
                if (_rule.ConnectEmotiv(IpToConnect, PortToConnect, ClickMode, Interval, Sensitivity))
                {
                    connectToolStripMenuItem.Text = @"Disconnect";
                    _connected = true;
                }
                else
                {
                    MessageBox.Show(@"Unable to connect to " + IpToConnect + @" on port " + PortToConnect, @"Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    connectToolStripMenuItem.Text = @"Connect";
                }
            }
            else
            {
                connectToolStripMenuItem.Text = @"Connect";
                _rule.DisconnectEmotiv();
                _connected = false;
            }

        }

        public void Disconnect()
        {
            if (_connected)
            {
                connectToolStripMenuItem.Text = @"Connect";
                _rule.DisconnectEmotiv();
                _connected = false;
            }
        }

        public void SwitchLineColor(int lineNumber)
        {
            var line = (TableLayoutPanel)keyboardKeys.GetControlFromPosition(0, lineNumber);
            line.BackColor = (line.BackColor != colorsPalette.HighlightColor ? 
                colorsPalette.HighlightColor :
                colorsPalette.AplicationBackground);
        }

        public int SwitchColumnColor(int lineNumber, int columnNumber)
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

            return line.ColumnCount;
        }

        public void ButtonClicked(int row, int coloumn)
        {
            var line = (TableLayoutPanel)keyboardKeys.GetControlFromPosition(0, row);
            var btn = line.GetControlFromPosition(coloumn, 0);

            //If button text is just a charachter, than it truly is a keyboard character. Else is a special key
            if (btn.Text.Length == 1)
                AlterTextOnControl(label1.Text+btn.Text.ToLower());
            else
            {
                if (btn.Name.Equals("keyBackSpace"))
                {
                    if (label1.Text != "")
                        AlterTextOnControl(label1.Text.Remove(label1.Text.Length - 1));
                }
                else if (btn.Name.Equals("KeyEnter"))
                    label1.Text = label1.Text + '\n';
                else if (btn.Name.Equals("KeySend"))
                    SendKeys.Send(label1.Text);
                else if (btn.Name.Equals("keyUndo"))
                    _rule.UndoClick();
                if (btn.Name.Equals("keyClear") || btn.Name.Equals("KeySend"))
                    AlterTextOnControl("");
            }


        }

        public String getText()
        {
            return label1.Text;
        }

        //public void AlterTextOnControl(Control ctrl, string text)
        public void AlterTextOnControl(string text)
        {
            Invoke((Action)delegate
            {
                label1.Text = text;
            });
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new frmOptions(this)).ShowDialog();
        }

        public void LostConnection()
        {
            Invoke((Action) delegate
            {
                connectToolStripMenuItem.Text = @"Connect";
                _connected = false;
            });
            MessageBox.Show(@"Lost connection to " + IpToConnect + @" on port " + PortToConnect, @"Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Text = "";
        }
    }
}
