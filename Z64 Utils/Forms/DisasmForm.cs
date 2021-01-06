using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RDP;
using Common;

namespace Z64.Forms
{
    public partial class DisasmForm : MicrosoftFontForm
    {
        List<F3DZEX.CommandInfo> _dlist;
        uint _vaddr;

        public DisasmForm(List<F3DZEX.CommandInfo> dlist = null, uint vaddr = 0)
        {
            InitializeComponent();

            _dlist = dlist;
            _vaddr = vaddr;
            if (_dlist != null)
            {
                textBox_bytes.Visible = label_bytes.Visible = label_disas.Visible = false;
                textBox_disassembly.Location = new Point(10, 30);
                textBox_disassembly.Size = new Size(Width-40, Height-80);
            }
            else
            {
                _dlist = new List<F3DZEX.CommandInfo>();
            }
            UpdateDisassembly();
        }

        public void Update(uint vaddr, List<F3DZEX.CommandInfo> dlist)
        {
            _vaddr = vaddr;
            _dlist = dlist;
            UpdateDisassembly();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            bool valid = Utils.IsValidHex(textBox_bytes.Text);
            label_bytes.ForeColor = valid ? Color.Green : Color.Red;
            textBox_disassembly.Text = "";

            if (valid)
            {
                byte[] data = Utils.HexToBytes(textBox_bytes.Text);
                try
                {
                    _dlist = F3DZEX.DecodeDList(data, 0);
                }
                catch
                {
                    _dlist = new List<F3DZEX.CommandInfo>();
                }
            }
            else
            {
                _dlist = new List<F3DZEX.CommandInfo>();
            }

            UpdateDisassembly();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DisasmSettingsForm.OpenInstance();
        }

        public void UpdateDisassembly()
        {
            RDPDisassembler disas = new RDPDisassembler(_dlist, _vaddr);
            var lines = disas.Disassemble();
            StringWriter sw = new StringWriter();
            foreach (var line in lines)
                sw.Write($"{line}\r\n");

            textBox_disassembly.Text = sw.ToString();
        }
    }
}
