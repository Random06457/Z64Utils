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

namespace Z64.Forms
{
    public partial class SegmentEditorForm : Form
    {
        public event EventHandler<RDPRenderer.Segment> SegmentsChanged;
        public SegmentEditorForm(Z64Game game, RDPRenderer renderer)
        {
            InitializeComponent();
            for (int i = 0; i < 16; i++)
            {
                SegmentControl seg = (SegmentControl)Controls[$"segmentControl{i}"];
                seg.SetGame(game);
                seg.SetSegment(renderer.Segments[i]);
                seg.SegmentChanged += Seg_SegmentChanged;
            }
        }

        private void Seg_SegmentChanged(object sender, RDPRenderer.Segment e)
        {
            SegmentsChanged?.Invoke(((SegmentControl)sender).SegmentID, e);
        }
    }
}
