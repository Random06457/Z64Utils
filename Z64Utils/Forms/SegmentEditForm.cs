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
using F3DZEX;

namespace Z64.Forms
{
    public partial class SegmentEditForm : MicrosoftFontForm
    {
        const string SRC_EMPTY = "Empty";
        const string SRC_ADDR = "Address";
        const string SRC_ROM_FS = "ROM FS";
        const string SRC_FILE = "File";
        const string SRC_IDENT_MTX = "Ident Matrices";
        const string SRC_NULL = "Null Bytes";

        public Memory.Segment ResultSegment { get; set; }

        private string _dmaFileName = null;
        private string _fileName = null;
        private Z64Game _game;

        public SegmentEditForm(Z64Game game)
        {
            InitializeComponent();
            _game = game;

            comboBox1.Items.Clear();
            comboBox1.Items.Add(SRC_EMPTY);
            comboBox1.Items.Add(SRC_ADDR);
            comboBox1.Items.Add(SRC_ROM_FS);
            comboBox1.Items.Add(SRC_FILE);
            comboBox1.Items.Add(SRC_IDENT_MTX);
            comboBox1.Items.Add(SRC_NULL);

            comboBox1.SelectedItem = SRC_EMPTY;
            DialogResult = DialogResult.Cancel;


            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.Appearance = TabAppearance.FlatButtons;
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedItem)
            {
                case SRC_ADDR: // Address
                    tabControl1.SelectedTab = tabPage_address;
                    okBtn.Enabled = uint.TryParse(addressValue.Text, NumberStyles.HexNumber, new CultureInfo("en-US"), out uint result);
                    break;
                case SRC_ROM_FS: // ROM FS
                    tabControl1.SelectedTab = tabPage_file;
                    okBtn.Enabled = _dmaFileName != null;
                    button1.ForeColor = _dmaFileName == null ? Color.Black : Color.Green;
                    break;
                case SRC_FILE: // File
                    tabControl1.SelectedTab = tabPage_file;
                    okBtn.Enabled = _fileName != null;
                    button1.ForeColor = _fileName == null ? Color.Black : Color.Green;
                    break;
                case SRC_EMPTY: // Empty
                case SRC_IDENT_MTX: // Ident Matrices
                case SRC_NULL: // Null Bytes
                    tabControl1.SelectedTab = tabPage_empty;
                    okBtn.Enabled = true;
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == (object)SRC_ROM_FS) // ROM FS
            {
                DmaFileSelectForm form = new DmaFileSelectForm(_game);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _dmaFileName = _game.GetFileName(form.SelectedFile.VRomStart);
                    ResultSegment = Memory.Segment.FromBytes(_dmaFileName, form.SelectedFile.Data);
                    button1.ForeColor = Color.Green;
                    okBtn.Enabled = _dmaFileName != null;
                }
            }
            else if (comboBox1.SelectedItem == (object)SRC_FILE)// File
            {
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = Filters.ALL;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    _fileName = openFileDialog1.FileName;
                    ResultSegment = Memory.Segment.FromBytes(Path.GetFileName(_fileName), File.ReadAllBytes(_fileName));
                    button1.ForeColor = Color.Green;
                    okBtn.Enabled = _fileName != null;
                }
            }
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedItem)
            {
                case SRC_ADDR:
                    uint addr = SegmentedAddress.Parse(addressValue.Text).VAddr;
                    ResultSegment = Memory.Segment.FromVram($"{addr:X8}", addr);
                    break;

                case SRC_IDENT_MTX:
                    ResultSegment = Memory.Segment.FromFill("Ident Matrices", new byte[] {
                        0,1,   0,0,   0,0,   0,0,
                        0,0,   0,1,   0,0,   0,0,
                        0,0,   0,0,   0,1,   0,0,
                        0,0,   0,0,   0,0,   0,1,

                        0,0,   0,0,   0,0,   0,0,
                        0,0,   0,0,   0,0,   0,0,
                        0,0,   0,0,   0,0,   0,0,
                        0,0,   0,0,   0,0,   0,0,
                    });
                    break;
                case SRC_NULL:
                    ResultSegment = Memory.Segment.FromFill("Null Bytes");
                    break;
                case SRC_EMPTY:
                    ResultSegment = Memory.Segment.Empty();
                    break;

                default:
                    break;
            }
            
            DialogResult = DialogResult.OK;
            Close();
        }

        private void addressValue_TextChanged(object sender, EventArgs e)
        {
            okBtn.Enabled = SegmentedAddress.TryParse(addressValue.Text, true, out var _);
        }
    }
}
