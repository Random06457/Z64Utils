using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Z64.Forms
{
    public partial class TextForm : MicrosoftFontForm
    {
        public TextForm(Icon icon, string title, string message)
        {
            InitializeComponent();
            Icon = icon;
            Text = title;
            textBox1.Text = message;
        }
    }
}
