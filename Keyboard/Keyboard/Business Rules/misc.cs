using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Keyboard.Controllers
{
    class alterateRenderer : ToolStripProfessionalRenderer
    {
        public alterateRenderer() : base(new menuColors()) { }
    }

    class misc
    {
        public static IEnumerable<Control> GetAllTypeControls(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllTypeControls(ctrl, type))
                .Concat(controls)
                .Where(c => c.GetType() == type);
        }

        //Got from: https://stackoverflow.com/questions/11412956/what-is-the-best-way-of-validating-an-ip-address
        //All credits to https://stackoverflow.com/users/961113/habib
        public static bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }
    }
}
