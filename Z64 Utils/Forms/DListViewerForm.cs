using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using OpenTK;
using RDP;
using System.IO;
using System.Globalization;
using Syroot.BinaryData;
using Z64;
using Common;

namespace Z64.Forms
{
    public partial class DListViewerForm : MicrosoftFontForm
    {
        public static DListViewerForm Instance { get; set; }

        Z64Game _game;
        RDPRenderer _renderer;
        SegmentEditorForm _segForm;
        DisasmForm _disasForm;
        RenderSettingsForm _settingsForm;
        List<F3DZEX.CommandInfo> _dlist = new List<F3DZEX.CommandInfo>();
        RDPRenderer.Config _rendererCfg;
        ModelViewerControl.Config _controlCfg;
        uint _vaddr;

        private DListViewerForm(Z64Game game)
        {
            _game = game;
            _rendererCfg = new RDPRenderer.Config();
            _controlCfg = new ModelViewerControl.Config();

            InitializeComponent();
            Toolkit.Init();

            _renderer = new RDPRenderer(game, _rendererCfg);
            modelViewer.CurrentConfig = _controlCfg;
            modelViewer.RenderCallback = _renderer.Render;

            StartRender();
        }

        public static void OpenInstance(Z64Game game)
        {
            if (Instance == null)
            {
                Instance = new DListViewerForm(game);
                Instance.Show();
            }
            else
            {
                Instance.Activate();
            }
        }
        
        public void SetSegment(int index, RDPRenderer.Segment segment)
        {
            if (index >= 0 && index < RDPRenderer.Segment.COUNT)
            {
                _renderer.Segments[index] = segment;
            }
        }
        public void SetAddress(uint vaddr)
        {
            toolStripTextBoxEntrypoint.Text = vaddr.ToString("X8");
            StartRender();
        }


        private void UpdateRender(object sender = null, EventArgs e = null)
        {
            _renderer.UpdateErrors();
            toolStripStatusErrorLabel.Text = _renderer.RenderFailed()
                ? $"RENDER ERROR AT 0x{_renderer.RenderErrorAddr:X8}! ({_renderer.ErrorMsg})"
                : "";

            modelViewer.Render();
        }

        //Address
        private void toolStripTextBoxEntrypoint_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !uint.TryParse(toolStripTextBoxEntrypoint.Text, NumberStyles.HexNumber, new CultureInfo("en-US"), out uint result);
        }

        private void StartRender(object sender = null, EventArgs e = null)
        {
            toolStripStatusErrorLabel.Text = "";
            _vaddr = uint.Parse(toolStripTextBoxEntrypoint.Text, NumberStyles.HexNumber);

            _dlist = _renderer.GetDlist(_vaddr);
            if (_dlist == null)
                _dlist = new List<F3DZEX.CommandInfo>();
            _renderer.Start(_vaddr);
            UpdateRender();
            _disasForm?.Update(_vaddr, _dlist);
        }

        private void toolStripTextBoxEntrypoint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                modelViewer.Focus();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (_segForm != null)
            {
                _segForm.Activate();
            }
            else
            {
                _segForm = new SegmentEditorForm(_game, _renderer);
                _segForm.SegmentsChanged += SegForm_SegmentsChanged;
                _segForm.FormClosed += SegForm_FormClosed;
                _segForm.Show();
            }
        }

        private void SegForm_SegmentsChanged(object sender, RDPRenderer.Segment e)
        {
            _renderer.Segments[(int)sender] = e;
            UpdateRender();
        }

        private void SegForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _segForm = null;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (_disasForm != null)
            {
                _disasForm.Activate();
            }
            else
            {
                _disasForm = new DisasmForm(_dlist, _vaddr);
                _disasForm.FormClosed += DisasForm_FormClosed;
                _disasForm.Show();
            }
        }

        private void DisasForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _disasForm = null;
        }

        private void DListViewerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Instance = null;
            _disasForm?.Close();
            _segForm?.Close();
            _settingsForm?.Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (_settingsForm != null)
            {
                _settingsForm.Activate();
            }
            else
            {
                _settingsForm = new RenderSettingsForm(_rendererCfg, _controlCfg);
                _settingsForm.FormClosed += SettingsForm_FormClosed;
                _settingsForm.SettingsChanged += UpdateRender;
                _settingsForm.Show();
            }
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _settingsForm = null;
        }

        private void saveScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "";
            saveFileDialog1.Filter = Filters.PNG;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var bmp = modelViewer.CaptureScreen();
                bmp.Save(saveFileDialog1.FileName);
            }
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bmp = modelViewer.CaptureScreen();
            Clipboard.SetImage(bmp);
        }

        private void modelViewer_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(modelViewer.PointToScreen(e.Location));
            }
        }
    }
}
