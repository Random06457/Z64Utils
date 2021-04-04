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

namespace Z64.Forms
{
    public partial class DisasmSettingsForm : MicrosoftFontForm
    {
        public static DisasmSettingsForm Instance { get; set; }

        F3DZEX.Disassembler _disas;

        public static void OpenInstance()
        {
            if (Instance == null)
            {
                Instance = new DisasmSettingsForm();
                Instance.Show();
            }
            else
            {
                Instance.Activate();
            }
        }

        private DisasmSettingsForm()
        {
            InitializeComponent();


            checkBoxShowAddr.Checked = F3DZEX.Disassembler.StaticConfig.ShowAddress;
            checkBoxRelativeAddr.Checked = F3DZEX.Disassembler.StaticConfig.RelativeAddress;
            checkBoxMultiCmdMacro.Checked = F3DZEX.Disassembler.StaticConfig.DisasMultiCmdMacro;
            checkBoxAddrLiteral.Checked = F3DZEX.Disassembler.StaticConfig.AddressLiteral;
            checkBoxStatic.Checked = F3DZEX.Disassembler.StaticConfig.Static;

            checkBoxShowAddr.CheckedChanged += new EventHandler(DisassemblyOptionForm_OnOptionUpdate);
            checkBoxRelativeAddr.CheckedChanged += new EventHandler(DisassemblyOptionForm_OnOptionUpdate);
            checkBoxMultiCmdMacro.CheckedChanged += new EventHandler(DisassemblyOptionForm_OnOptionUpdate);
            checkBoxAddrLiteral.CheckedChanged += new EventHandler(DisassemblyOptionForm_OnOptionUpdate);
            checkBoxStatic.CheckedChanged += new EventHandler(DisassemblyOptionForm_OnOptionUpdate);

            byte[] dlist = new byte[] { 0x01, 0x01, 0x20, 0x24, 0x06, 0x00, 0x0F, 0xC8 };
            _disas = new F3DZEX.Disassembler(new F3DZEX.Dlist(dlist, 0x060002C8));

            DisassemblyOptionForm_OnOptionUpdate(this, EventArgs.Empty);
        }

        private void DisassemblyOptionForm_OnOptionUpdate(object sender, EventArgs e)
        {
            F3DZEX.Disassembler.StaticConfig.ShowAddress = checkBoxShowAddr.Checked;
            F3DZEX.Disassembler.StaticConfig.RelativeAddress= checkBoxRelativeAddr.Checked;
            F3DZEX.Disassembler.StaticConfig.DisasMultiCmdMacro = checkBoxMultiCmdMacro.Checked;
            F3DZEX.Disassembler.StaticConfig.AddressLiteral = checkBoxAddrLiteral.Checked;
            F3DZEX.Disassembler.StaticConfig.Static = checkBoxStatic.Checked;

            Application.OpenForms.OfType<DisasmForm>().ToList().ForEach(f => f.UpdateDisassembly());
            Application.OpenForms.OfType<ObjectAnalyzerForm>().ToList().ForEach(f => f.UpdateDisassembly());

            var lines = _disas.Disassemble();

            StringWriter sw = new StringWriter();
            foreach (var line in lines)
                sw.WriteLine(line);

            previewTextBox.Text = sw.ToString();
        }

        private void DisasmSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Instance = null;
        }
    }
}
