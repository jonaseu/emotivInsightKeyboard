using System;
using System.Windows.Forms;

namespace Keyboard.Business_Rules
{
    class rulKeyboard
    {
        public int TimeInterval { get; set; }
        public int Sensibility { get; set; }
        public int IPtoConnect { get; set; }
        public int ClickMode { get; set; }
        public bool ShouldBlink { get; set; }

        private ctrKeyboard _form = null;
        private int _currentLine = 0;
        private int _currentColumn = 0;
        private Timer _blinkTimer = null;
        private bool blinkLine = true;

        public rulKeyboard(ctrKeyboard frm)
        {
            _form = frm;
        }

        public void BeginAlternateLines(int interval)
        {
            ShouldBlink = true;
            _blinkTimer = new Timer { Interval = interval };
            _blinkTimer.Tick += TimerTick;
            _form.SwitchLineColor(0);
            _blinkTimer.Start();
        }

        public void StopAlternateLines()
        {
            _blinkTimer.Stop();
            ShouldBlink = false;

            if(blinkLine)
                _form.SwitchLineColor(_currentLine);
            else
                _form.SwitchColumnColor(_currentLine,_currentColumn);
            
            _currentLine = 0;
            _currentColumn = 0;
            blinkLine = true;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (ShouldBlink)
            {
                //If it's supposed to blink line switches its color, if not, switch columns color
                if (blinkLine)
                {
                    _form.SwitchLineColor(_currentLine);
                    _currentLine = (_currentLine + 1) % 5;
                    _form.SwitchLineColor(_currentLine);
                }
                else
                {
                    _form.SwitchColumnColor(_currentLine,_currentColumn);
                    _currentColumn = (_currentColumn + 1) % 12;
                    _form.SwitchColumnColor(_currentLine, _currentColumn);
                }
            }
        }

        public void activateKey()
        {
            //If was blinking lines change to blink columns, if was blinking columns press the current button
            if (blinkLine)
            {
                blinkLine = false;
                _form.SwitchColumnColor(_currentLine,0);
                _form.SwitchLineColor(_currentLine);
                //Resets timer
                _blinkTimer.Stop();
                _blinkTimer.Start();
            }
            else
            {
                blinkLine = true;
                _form.SwitchColumnColor(_currentLine, _currentColumn);
                _form.ButtonClicked(_currentLine, _currentColumn);
                
                _currentLine = _currentColumn = 0;
                _form.SwitchLineColor(0);
                _blinkTimer.Stop();
                _blinkTimer.Start();            
            }
        }
    }
}
