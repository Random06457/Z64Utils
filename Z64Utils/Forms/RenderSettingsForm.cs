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

        public RenderSettingsForm(Renderer.Config rendererCfg)
        {
            InitializeComponent();
            _rendererCfg = rendererCfg;

            checkBox_renderTextures.Checked = _rendererCfg.RenderTextures;
            value_gridScale.Value = (decimal)_rendererCfg.GridScale;
            checkBox_showAxis.Checked = _rendererCfg.ShowAxis;
            checkBox_showGrid.Checked = _rendererCfg.ShowGrid;
        }

        private void UpdateSettings(object sender, EventArgs e)
        {
            _rendererCfg.RenderTextures = checkBox_renderTextures.Checked;
            _rendererCfg.GridScale = (float)value_gridScale.Value;
            _rendererCfg.ShowAxis = checkBox_showAxis.Checked;
            _rendererCfg.ShowGrid = checkBox_showGrid.Checked;

            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
