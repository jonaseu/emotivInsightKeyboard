using System;
using System.Windows.Forms;
using Keyboard.Controllers;
using Keyboard.Rules;

namespace Keyboard
{
    static class Initializer
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmKeyboard());
        }
    }
}
