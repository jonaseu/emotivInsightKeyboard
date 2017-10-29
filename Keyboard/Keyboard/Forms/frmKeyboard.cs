using System;
using System.Drawing;
using System.Windows.Forms;
using Keyboard.Business_Rules;
using Keyboard.Rules;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Keyboard.Controllers
{
    public partial class frmKeyboard : Form
    {
        public string IpToConnect { get; set; }
        public string ClickMode { get; set; }
        public int ClickSpeed { get; set; }
        public int Interval { get; set; }
        public int PortToConnect { get; set; }
        public string Language { get; set; }

        private readonly rulKeyboard _rule;
        private bool _isCapsOn;
        private bool _connected;
        private SpeechSynthesizer _synth;
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
            Language = "pt_BR";
            //Language = "en";
            IpToConnect = "127.0.0.1";
            ClickSpeed = 1200;
            ClickMode = "Mental Push";
            Interval = 1700;
            PortToConnect = 1900;

            InitializeComponent();
            _rule = new rulKeyboard(this);
            _isCapsOn = true;
            _synth = new SpeechSynthesizer();
            _synth.SetOutputToDefaultAudioDevice();
        }

        private void ctrKeyboard_Load(object sender, EventArgs e)
        {
            changeCase();
            menuStrip1.Renderer = new alterateRenderer();
            label1.BackColor = colorsPalette.FontColor;
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
                btn.FlatAppearance.BorderColor = colorsPalette.ButtonBorder;
                if (control.Parent.Name == "keyboardLine0")
                {
                    btn.BackColor = colorsPalette.AplicationBackground;
                }
                else
                {
                    btn.Margin = new Padding(3, 4, 3, 4);
                    btn.BackColor = colorsPalette.ButtonColor;
                }
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
            if (!_connected)
            {
                connectToolStripMenuItem.Text = @"Connecting";
                Refresh();

                if (_rule.ConnectEmotiv(IpToConnect, PortToConnect, ClickMode, Interval, ClickSpeed))
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

            //If button text is just a character, than it truly is a keyboard character. Else is a special key
            if (btn.Text.Length == 1)
                if (_isCapsOn)
                    AlterTextOnControl(label1.Text + btn.Text.ToUpper());
                else
                    AlterTextOnControl(label1.Text + btn.Text.ToLower());
            else
            {
                interactWithButton(btn.Name,(Button)btn);
            }
        }
        private void interactWithButton(String buttonName, Button btn)
        {
            switch (buttonName)
            {
                case "keyBackSpace":
                    if (label1.Text != "")
                        AlterTextOnControl(label1.Text.Remove(label1.Text.Length - 1));
                    break;
                case "keyCaps":
                    changeCase();
                    break;
                case "KeyEnter":
                    AlterTextOnControl(label1.Text + '\n');
                    break;

                case "keySpeak":
                    Task.Run(() => _synth.Speak(label1.Text));
                    //SendKeys.Send(label1.Text);
                    AlterTextOnControl("");
                    break;

                case "keyUndo":
                    _rule.UndoClick();
                    break;

                case "keyUrgent":
                    Task.Run(() => _synth.Speak("I need Help"));
                    break;

                case "keyThirsty":
                    Task.Run(() => _synth.Speak("I'm thirsty"));
                    break;

                case "keyHungry":
                    Task.Run(() => _synth.Speak("I'm hungry"));
                    break;

                case "keyTired":
                    Task.Run(() => _synth.Speak("I'm feeling tired"));
                    break;

                case "keyClear":
                    AlterTextOnControl("");
                    break;

                case "keySuggestion0":
                case "keySuggestion1":
                case "keySuggestion2":
                case "keySuggestion3":
                case "keySuggestion4":
                    string lastWord = label1.Text.Split(' ')[label1.Text.Split(' ').Length - 1];
                    string newLine = "";
                    for (int i = 0; i < label1.Text.Split(' ').Length - 1; i++)
                        newLine += label1.Text.Split(' ')[i]+' ';
                    newLine += btn.Text+' ';
                    AlterTextOnControl(newLine);
                    break;
            }
        }

        public String getText()
        {
            return label1.Text;
        }

        private void changeCase()
        {
            var buttons = misc.GetAllTypeControls(this, typeof(Button));
            _isCapsOn = !_isCapsOn;
            SwitchColumnColor(4, 0);
            foreach (var control in buttons)
            {
                var btn = (Button)control;
                if (btn.Text.Length == 1)
                {
                    if (_isCapsOn)
                        Invoke((Action)delegate
                        {
                            btn.Text = btn.Text.ToUpper();
                        });

                    else
                        Invoke((Action)delegate
                        {
                            btn.Text = btn.Text.ToLower();
                        });

                }
            }
        }


        //public void AlterTextOnControl(Control ctrl, string text)
        public void AlterTextOnControl(string text)
        {

            Invoke((Action)delegate
            {
                label1.Text = text;
            });
            string lastWord = text.Split(' ')[text.Split(' ').Length - 1];
            Task.Run(() =>
            {
                List<string> suggestions = textCompletion.suggestWordsSimple(lastWord, Language);
                for (int i = 0; i < 5; i++)
                {
                    var line = (TableLayoutPanel)keyboardKeys.GetControlFromPosition(0, 0);
                    var btn = line.GetControlFromPosition(i, 0);
                    Invoke((Action)delegate
                    {
                        if (suggestions.Count - 1 >= i)
                            btn.Text = suggestions[i];
                        else
                            btn.Text = "";
                    });
                }
            });
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new frmOptions(this)).ShowDialog();
        }

        public void LostConnection()
        {
            Invoke((Action)delegate
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
