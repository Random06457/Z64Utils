using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using Z64;
using N64;
using Syroot.BinaryData;
using System.Text.RegularExpressions;
using RDP;
using Common;
using System.Diagnostics;

namespace Z64.Forms
{
    public partial class MainForm : MicrosoftFontForm
    {
        Z64Game _game = null;
        string[] _fileItemsText;
        string _lastSearch = null;

        public MainForm()
        {
            InitializeComponent();

            toolStrip1.DataBindings.Clear();
            tabControl1.DataBindings.Clear();

            UpdateControls();
        }

        private void StartTask(Action action)
        {
            new Thread(() =>
            {
                Invoke(new Action(() => UpdateControls(true)));

                action.Invoke();

                Invoke(new Action(() => {
                    label_loadProgress.Text = "...";
                    progressBar1.Value = 0;
                    SystemSounds.Asterisk.Play();
                    UpdateControls(false);
                }));

            })
            {
                IsBackground = true,
            }.Start();
        }

        public void ProcessCallback(float progress, string text)
        {
            Invoke(new Action(() => {
                progressBar1.Value = (int)(progress * progressBar1.Maximum);
                label_loadProgress.Text = text;
            }));
        }

        private void UpdateControls(bool inTask = false)
        {
            toolStrip1.Enabled = !inTask;
            tabControl1.Enabled = !inTask;

            tabControl1.Enabled = _game != null;
            openObjectToolStripMenuItem.Enabled = _game != null;
            exportFSToolStripMenuItem.Enabled = _game != null;
            saveToolStripMenuItem.Enabled = _game != null;
            ROMRAMConversionsToolStripMenuItem.Enabled = _game != null;
            textureViewerToolStripMenuItem.Enabled = _game != null;
        }

        private void UpdateFileList()
        {
            listView_files.BeginUpdate();
            string search = textBox_fileFilter.Text.ToLower();
            if (_lastSearch != null && search.Contains(_lastSearch))
            {
                for (int i = 0; i < listView_files.Items.Count; i++)
                    if (!_fileItemsText[(int)listView_files.Items[i].Tag].Contains(search))
                        listView_files.Items.RemoveAt(i--);
            }
            else
            {
                listView_files.Items.Clear();
                for (int i = 0; i < _game.GetFileCount(); i++)
                {
                    var file = _game.GetFileFromIndex(i);
                    if (!file.Valid())
                        continue;

                    if (_fileItemsText[i].Contains(search))
                    {
                        string name = _game.GetFileName(file.VRomStart);
                        string vrom = $"{file.VRomStart:X8}-{file.VRomEnd:X8}";
                        string rom = $"{file.RomStart:X8}-{file.RomEnd:X8}";
                        string type = "Unknow";

                        var item = listView_files.Items.Add(name);
                        item.SubItems.AddRange(new string[] { vrom, rom, type });
                        item.Tag = i;
                    }
                }
            }
            _lastSearch = search;
            listView_files.EndUpdate();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = $"{Filters.N64}|{Filters.ALL}";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _game = null;
                Application.OpenForms.OfType<Form>().ToList().ForEach(f =>
                {
                    if (f != this)
                        f.Close();
                });

                StartTask(() => {
                    try
                    {
                        _game = new Z64Game(openFileDialog1.FileName, ProcessCallback);
                        if (!Z64Version.ContainsConfig(_game.Version))
                        {
                            Invoke(new Action(() =>
                            {
                                MessageBox.Show($"No config file found for this version!\r\n(should be versions/{_game.Version}.json)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));
                        }
                    }
                    catch(Exception ex)
                    {
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }

                    Invoke(new Action(() =>
                    {
                        if (_game != null)
                        {
                            Text = $"Z64 Utils - {Path.GetFileName(openFileDialog1.FileName)} [ver. {_game.Version} ({_game.BuildID})]";

                            _fileItemsText = new string[_game.GetFileCount()];
                            for (int i = 0; i < _game.GetFileCount(); i++)
                            {
                                var file = _game.GetFileFromIndex(i);
                                if (!file.Valid())
                                    continue;

                                _fileItemsText[i] = ($"{_game.GetFileName(file.VRomStart).ToLower()} {file.VRomStart:x8} {file.VRomEnd:x8}");
                            }

                            _lastSearch = null;
                            UpdateFileList();
                        }
                        UpdateControls();
                        GC.Collect();
                    }));
                });
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "";
            saveFileDialog1.Filter = $"{Filters.N64}|{Filters.ALL}";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _game.FixRom();
                File.WriteAllBytes(saveFileDialog1.FileName, _game.Rom.RawRom);
                SystemSounds.Asterisk.Play();
            }
        }


        private void ExportFSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = "";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                StartTask(() => {
                    for (int i = 0; i < _game.GetFileCount(); i++)
                    {
                        var file = _game.GetFileFromIndex(i);
                        ProcessCallback((float)i/_game.GetFileCount(), $"Writing Files... {i}/{_game.GetFileCount()}");
                        if (file.Data != null)
                        {
                            string name = $"{_game.GetFileName(file.VRomStart):X8}";
                            if (string.IsNullOrEmpty(name) || Utils.IsValidFileName(name))
                                name = $"{file.VRomStart:X8}-{file.VRomEnd:X8}";
                            File.WriteAllBytes($"{folderBrowserDialog1.SelectedPath}/{name}.bin", file.Data);
                        }
                    }
                });
            }
        }

        private void InjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView_files.SelectedIndices.Count != 1)
                return;
            var item = listView_files.SelectedItems[0];
            int vrom = _game.GetFileFromIndex((int)item.Tag).VRomStart;

            openFileDialog1.FileName = "";
            openFileDialog1.Filter = Filters.ALL;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _game.InjectFile(vrom, File.ReadAllBytes(openFileDialog1.FileName));
                SystemSounds.Asterisk.Play();
            }
        }
        private void SaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView_files.SelectedIndices.Count != 1)
                return;
            var item = listView_files.SelectedItems[0];
            var file = _game.GetFileFromIndex((int)item.Tag);

            if (!file.Valid())
            {
                MessageBox.Show("Invalid File");
                return;
            }
            if (file.Deleted)
            {
                MessageBox.Show("Deleted File");
                return;
            }

            saveFileDialog1.FileName = $"{_game.GetFileName(file.VRomStart)}.bin";
            saveFileDialog1.Filter = Filters.ALL;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog1.FileName, file.Data);
                SystemSounds.Asterisk.Play();
            }
        }
        private void OpenObjectViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView_files.SelectedIndices.Count != 1)
                return;
            var item = listView_files.SelectedItems[0];
            var file = _game.GetFileFromIndex((int)item.Tag);

            if (!file.Valid())
            {
                MessageBox.Show("Invalid File");
                return;
            }
            if (file.Deleted)
            {
                MessageBox.Show("Deleted File");
                return;
            }


            string defaultValue = null;
            string fileName = _game.GetFileName(file.VRomStart).ToLower();

            if (fileName.StartsWith("object_"))
                defaultValue = "6";
            else if (fileName.Contains("_room_"))
                defaultValue = "3";
            else if (fileName.EndsWith("_scene"))
                defaultValue = "2";
            else if (fileName == "gameplay_keep")
                defaultValue = "4";
            else if (fileName.StartsWith("gameplay_"))
                defaultValue = "5";

            var valueForm = new EditValueForm("Choose Segment", "Plase enter a segment id", (v) =>
            {
                return (int.TryParse(v, out int ret) && ret >= 0 && ret < 16)
                ? null
                : "Segment ID must be a value between 0 and 15";
            }, defaultValue);
            if (valueForm.ShowDialog() == DialogResult.OK)
            {
                var form = new ObjectAnalyzerForm(_game, file.Data, int.Parse(valueForm.Result));
                form.Text += $" - \"{_game.GetFileName(file.VRomStart)}\" ({file.VRomStart:X8}-{file.VRomEnd:X8})";
                form.Show();
            }
        }

        private void TextBox_fileFilter_TextChanged(object sender, EventArgs e)
        {
            UpdateFileList();
        }

        private void OpenDlistViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DListViewerForm.OpenInstance(_game);
        }

        private void f3DEXDisassemblerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new DisasmForm(true).Show();
        }

        private void ROMRAMConversionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ConversionForm(_game).Show();
        }

        private void textureViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new TextureViewer(_game).Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void checkNewReleasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newTag = UpdateChecker.GetLatestTag();
            if (UpdateChecker.CurrentTag != newTag)
            {
                var res = MessageBox.Show($"A new release is available on github (tag {newTag}).\r\nWould you like to open the release page?", "New Release Available", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                    Process.Start(UpdateChecker.ReleaseURL);
            }
            else
                MessageBox.Show("No new release available.");
        }
    }
}
