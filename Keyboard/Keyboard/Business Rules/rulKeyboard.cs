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
        private int _currentLine = 1;
        private int _currentColumn = 1;

        public rulKeyboard(ctrKeyboard frm)
        {
            _form = frm;
        }

        public void BeginAlternateLines()
        {
            ShouldBlink = true;
            Timer blinkTime = new Timer { Interval = TimeInterval };
            blinkTime.Tick += TimerTick;
            _form.changeColumnColor(1, 1);
            blinkTime.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (ShouldBlink)
            {
                _form.changeColumnColor(_currentLine,_currentColumn);
                _currentColumn++; 
                _form.changeColumnColor(_currentLine, _currentColumn);
                
            }
        }
    }
}
