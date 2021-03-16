using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using F3DZEX;
using F3DZEX.Render;

namespace Z64.Forms
{
    public partial class RenderSettingsForm : MicrosoftFontForm
    {
        public event EventHandler SettingsChanged;

        Renderer.Config _rendererCfg;

        public RenderSettingsForm(Renderer.Config cfg)
        {
            InitializeComponent();

            _rendererCfg = cfg;

            propertyGrid1.SelectedObject = _rendererCfg;
        }

        private void UpdateSettings(object sender, EventArgs e)
        {
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
