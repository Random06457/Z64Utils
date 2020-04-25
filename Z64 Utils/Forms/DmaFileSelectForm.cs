using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Z64.Forms
{
    public partial class DmaFileSelectForm : Form
    {
        public Z64File SelectedFile { get; private set; }

        Z64Game _game;

        public DmaFileSelectForm(Z64Game game)
        {
            InitializeComponent();
            _game = game;
            UpdateFileList();
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            UpdateFileList();
        }

        private void UpdateFileList()
        {
            fileListView.BeginUpdate();
            fileListView.Items.Clear();
            for (int i = 0; i < _game.GetFileCount(); i++)
            {
                var file = _game.GetFileFromIndex(i);
                if (!file.Valid())
                    continue;

                string name = _game.GetFileName(file.VRomStart);
                string vrom = $"{file.VRomStart:X8}-{file.VRomEnd:X8}";
                string rom = $"{file.RomStart:X8}-{file.RomEnd:X8}";
                string type = "Unknow";

                if (name.ToLower().Contains(searchBox.Text.ToLower()) ||
                    vrom.ToLower().Contains(searchBox.Text.ToLower()) ||
                    rom.ToLower().Contains(searchBox.Text.ToLower()) ||
                    type.ToLower().Contains(searchBox.Text.ToLower()))
                {

                    var item = fileListView.Items.Add(name);
                    item.Tag = file.VRomStart;
                    item.SubItems.AddRange(new string[] { vrom, rom, type });
                }

            }
            fileListView.EndUpdate();
        }

        private void listView_files_DoubleClick(object sender, EventArgs e)
        {
            if (fileListView.SelectedItems.Count == 1)
            {
                SelectedFile = _game.GetFile((int)fileListView.SelectedItems[0].Tag);
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
