using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Keyboard.Controllers;

namespace Keyboard
{
    public partial class ctrKeyboard : Form
    {
        
        public ctrKeyboard()
        {
            InitializeComponent();
            menuStrip1.Renderer = new alterateRenderer();
        }
    }
}
