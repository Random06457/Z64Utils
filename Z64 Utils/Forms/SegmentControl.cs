using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RDP;
using System.IO;
using Common;
using System.Globalization;

namespace Z64.Forms
{
    public partial class SegmentControl : UserControl
    {
        public event EventHandler<RDPRenderer.Segment> SegmentChanged;
        public int SegmentID {
            get {
                return _segmentId;
            }
            set {
                if (value < 0)
                    _segmentId = 0;
                else if (value >= 15)
                    _segmentId = 15;
                else
                    _segmentId = value;

                indexLabel.Text = $"{SegmentID:D2} :";
            } }

        private int _segmentId;
        Z64Game _game;

        public SegmentControl()
        {
            InitializeComponent();
            indexLabel.Text = $"{SegmentID:D2} :";
        }

        public void SetGame(Z64Game game)
        {
            _game = game;
        }
        public void SetSegment(RDPRenderer.Segment seg)
        {
            addressValue.ForeColor = Color.Black;
            importFileButton.ForeColor = Color.Black;
            dmaFileButton.ForeColor = Color.Black;

            if (seg.IsVram())
            {
                addressValue.Text = seg.Address.ToString("X8");
                addressValue.ForeColor = Color.Green;
            }
            else
            {
                importFileButton.ForeColor = Color.Green;
                dmaFileButton.ForeColor = Color.Green;
            }
        }

        private void importFileButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = Filters.ALL;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SegmentChanged?.Invoke(this, RDPRenderer.Segment.FromBytes(File.ReadAllBytes(openFileDialog1.FileName)));

                addressValue.ForeColor = Color.Black;
                importFileButton.ForeColor = Color.Green;
                dmaFileButton.ForeColor = Color.Black;
            }
        }


        private void dmaFileButton_Click(object sender, EventArgs e)
        {
            DmaFileSelectForm form = new DmaFileSelectForm(_game);
            if (form.ShowDialog() == DialogResult.OK)
            {
                SegmentChanged?.Invoke(this, RDPRenderer.Segment.FromBytes(form.SelectedFile.Data));

                addressValue.ForeColor = Color.Black;
                importFileButton.ForeColor = Color.Black;
                dmaFileButton.ForeColor = Color.Green;
            }
        }

        private void addressValue_Validated(object sender, EventArgs e)
        {
            SegmentChanged?.Invoke(this, RDPRenderer.Segment.FromVram(uint.Parse(addressValue.Text, NumberStyles.HexNumber)));

            addressValue.ForeColor = Color.Green;
            importFileButton.ForeColor = Color.Black;
            dmaFileButton.ForeColor = Color.Black;
        }

        private void addressValue_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !uint.TryParse(addressValue.Text, NumberStyles.HexNumber, new CultureInfo("en-US"), out uint result);
        }
    }
}
