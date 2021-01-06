using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RDP;
using Common;
using System.IO;
using System.Globalization;

namespace Z64.Forms
{
    public partial class SegmentEditForm : MicrosoftFontForm
    {
        public RDPRenderer.Segment ResultSegment { get; set; }

        private string _dmaFileName = null;
        private string _fileName = null;
        private Z64Game _game;

        public SegmentEditForm(Z64Game game)
        {
            InitializeComponent();
            _game = game;
            comboBox1.SelectedIndex = 0;
            DialogResult = DialogResult.Cancel;


            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.Appearance = TabAppearance.FlatButtons;
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0: // Address
                    tabControl1.SelectedIndex = 0;
                    okBtn.Enabled = uint.TryParse(addressValue.Text, NumberStyles.HexNumber, new CultureInfo("en-US"), out uint result);
                    break;
                case 1: // DMA File
                    tabControl1.SelectedIndex = 1;
                    okBtn.Enabled = _dmaFileName != null;
                    button1.ForeColor = _dmaFileName == null ? Color.Black : Color.Green;
                    break;
                case 2: // File
                    tabControl1.SelectedIndex = 1;
                    okBtn.Enabled = _fileName != null;
                    button1.ForeColor = _fileName == null ? Color.Black : Color.Green;
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1) // DMA File
            {
                DmaFileSelectForm form = new DmaFileSelectForm(_game);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _dmaFileName = _game.GetFileName(form.SelectedFile.VRomStart);
                    ResultSegment = RDPRenderer.Segment.FromBytes(form.SelectedFile.Data, _dmaFileName);
                    button1.ForeColor = Color.Green;
                    okBtn.Enabled = _dmaFileName != null;
                }
            }
            else
            {
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = Filters.ALL;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    _fileName = openFileDialog1.FileName;
                    ResultSegment = RDPRenderer.Segment.FromBytes(File.ReadAllBytes(_fileName), Path.GetFileName(_fileName));
                    button1.ForeColor = Color.Green;
                    okBtn.Enabled = _fileName != null;
                }
            }
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            ResultSegment = ResultSegment;
            if (comboBox1.SelectedIndex == 0)
            {
                uint addr = uint.Parse(addressValue.Text, NumberStyles.HexNumber);
                ResultSegment = RDPRenderer.Segment.FromVram(addr, addr == 0 ? "[NULL]" : $"{addr:X8}");
            }
            
            DialogResult = DialogResult.OK;
            Close();
        }

        private void addressValue_TextChanged(object sender, EventArgs e)
        {
            okBtn.Enabled = uint.TryParse(addressValue.Text, NumberStyles.HexNumber, new CultureInfo("en-US"), out uint result);
        }
    }
}
