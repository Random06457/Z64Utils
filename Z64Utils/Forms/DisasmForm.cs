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
using Common;
using DList = System.Collections.Generic.List<System.Tuple<uint, F3DZEX.Command.CommandInfo>>;

namespace Z64.Forms
{
    public partial class DisasmForm : MicrosoftFontForm
    {
        uint? _vaddr;
        DList _dlist;
        List<F3DZEX.Command.CommandInfo> _cmds;

        public DisasmForm(List<F3DZEX.Command.CommandInfo> cmds, uint vaddr = 0)
        {
            InitializeComponent();

            _dlist = null;
            _cmds = cmds;
            _vaddr = vaddr;
            if (_cmds != null)
            {
                textBox_bytes.Visible = label_bytes.Visible = label_disas.Visible = false;
                textBox_disassembly.Location = new Point(10, 30);
                textBox_disassembly.Size = new Size(Width - 40, Height - 80);
            }
        }

        public DisasmForm(DList dlist)
        {
            InitializeComponent();

            _dlist = dlist;
            _cmds = null;
            _vaddr = null;
            if (_dlist != null)
            {
                textBox_bytes.Visible = label_bytes.Visible = label_disas.Visible = false;
                textBox_disassembly.Location = new Point(10, 30);
                textBox_disassembly.Size = new Size(Width-40, Height-80);
            }
            UpdateDisassembly();
        }

        public void Update(DList dlist)
        {
            _cmds = null;
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
                    _cmds = F3DZEX.Command.DecodeDList(data, 0);
                }
                catch
                {
                    _cmds = new List<F3DZEX.Command.CommandInfo>();
                }
            }
            else
            {
                _cmds = new List<F3DZEX.Command.CommandInfo>();
            }

            UpdateDisassembly();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DisasmSettingsForm.OpenInstance();
        }

        public void UpdateDisassembly()
        {
            F3DZEX.Disassembler disas = _vaddr != null ? new F3DZEX.Disassembler(_cmds, _vaddr.Value) : new F3DZEX.Disassembler(_dlist);
            var lines = disas.Disassemble();
            StringWriter sw = new StringWriter();
            foreach (var line in lines)
                sw.Write($"{line}\r\n");

            textBox_disassembly.Text = sw.ToString();
        }
    }
}
