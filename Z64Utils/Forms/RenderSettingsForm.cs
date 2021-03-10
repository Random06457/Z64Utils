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
        ModelViewerControl.Config _controlCfg;

        public RenderSettingsForm(Renderer.Config rendererCfg, ModelViewerControl.Config controlCfg)
        {
            InitializeComponent();
            _rendererCfg = rendererCfg;
            _controlCfg = controlCfg;

            checkBox_renderTextures.Checked = _rendererCfg.RenderTextures;
            value_gridScale.Value = (decimal)_controlCfg.GridScale;
            checkBox_showAxis.Checked = _controlCfg.ShowAxis;
            checkBox_showGrid.Checked = _controlCfg.ShowGrid;
        }

        private void UpdateSettings(object sender, EventArgs e)
        {
            _rendererCfg.RenderTextures = checkBox_renderTextures.Checked;
            _controlCfg.GridScale = (float)value_gridScale.Value;
            _controlCfg.ShowAxis = checkBox_showAxis.Checked;
            _controlCfg.ShowGrid = checkBox_showGrid.Checked;

            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
