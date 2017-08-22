using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyboard.Business_Rules
{
    class rulKeyboard
    {
        public int TimeInterval { get; set; }
        public int Sensibility { get; set; }
        public int IPtoConnect { get; set; }
        public int clickMode { get; set; }

        public void BeginAlternateLines()
        {
            System.Windows.Forms.Timer blinkTime = new System.Windows.Forms.Timer { Interval = TimeInterval };
            blinkTime.Tick += new EventHandler(timerTick);
            blinkTime.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {
            
        }
    }
}
