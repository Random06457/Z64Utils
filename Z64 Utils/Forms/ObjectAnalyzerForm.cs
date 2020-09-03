using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RDP;
using Be.Windows.Forms;
using Common;

namespace Z64.Forms
{
    public partial class ObjectAnalyzerForm : Form
    {
        byte[] _data;
        Z64Object _obj;
        int _segment;
        Z64Game _game;

        public ObjectAnalyzerForm(Z64Game game, byte[] data, int segmentId)
        {
            InitializeComponent();

            if (segmentId > 15)
                segmentId = 15;
            if (segmentId < 0)
                segmentId = 0;

            _data = data;
            _obj = new Z64Object(_data);
            _segment = segmentId;
            _game = game;

            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.Appearance = TabAppearance.FlatButtons;

            MinimumSize = new Size(Width, Height);

            UpdateMap();
        }

        public void UpdateDisassembly()
        {
            var dlist = GetCurrentHolder<Z64Object.DListHolder>();
            if (dlist != null)
            {
                try
                {
                    RDPDisassembler disas = new RDPDisassembler(dlist.GetData(), new SegmentedAddress(_segment, _obj.OffsetOf(dlist)).VAddr);
                    var lines = disas.Disassemble();
                    StringWriter sw = new StringWriter();
                    lines.ForEach(s => sw.WriteLine(s));
                    ucodeTextBox.Text = sw.ToString();
                }
                catch (Exception ex)
                {
                    ucodeTextBox.Text = "ERROR";
                }
            }
        }


        private void UpdateMap()
        {
            listView_map.Items.Clear();
            listView_map.BeginUpdate();
            foreach (var entry in _obj.Entries)
            {
                var item = listView_map.Items.Add($"{new SegmentedAddress(_segment, _obj.OffsetOf(entry)).VAddr:X8}");
                item.SubItems.Add(entry.Name);
                item.SubItems.Add(entry.GetEntryType().ToString());
            }
            listView_map.EndUpdate();
        }

        private T GetCurrentHolder<T>()
            where T : Z64Object.ObjectHolder
        {
            if (listView_map.SelectedIndices.Count != 1)
                return null;
            int idx = listView_map.SelectedIndices[0];
            if (idx >= _obj.Entries.Count || idx < 0)
                return null;

            return (typeof(T) == typeof(Z64Object.ObjectHolder) || typeof(T) == _obj.Entries[idx].GetType())
                ? (T)_obj.Entries[idx]
                : null;
        }

        private void listView_map_SelectedIndexChanged(object sender, EventArgs e)
        {
            var holder = GetCurrentHolder<Z64Object.ObjectHolder>();
            if (holder == null)
            {
                tabControl1.SelectedIndex = 0;
                return;
            }

            switch (holder.GetEntryType())
            {
                case Z64Object.EntryType.DList:
                    {
                        tabControl1.SelectedIndex = 1;
                        UpdateDisassembly();
                        break;
                    }
                case Z64Object.EntryType.Vertex:
                    {
                        tabControl1.SelectedIndex = 3;
                        var vtx = (Z64Object.VertexHolder)holder;

                        listView_vtx.BeginUpdate();
                        listView_vtx.Items.Clear();
                        uint addr = new SegmentedAddress(_segment, _obj.OffsetOf(holder)).VAddr;
                        for (int i = 0; i < vtx.Vertices.Count; i++)
                        {
                            var item = listView_vtx.Items.Add($"{addr:X8}");
                            item.SubItems.Add($"{vtx.Vertices[i].X}, {vtx.Vertices[i].Y}, {vtx.Vertices[i].Z}");
                            item.SubItems.Add($"0x{vtx.Vertices[i].Flag:X8}");
                            item.SubItems.Add($"{vtx.Vertices[i].TexX}, {vtx.Vertices[i].TexY}");
                            item.SubItems.Add($"{vtx.Vertices[i].R}, {vtx.Vertices[i].G}, {vtx.Vertices[i].B}, {vtx.Vertices[i].A}");
                            addr += 0x10;
                        }
                        listView_vtx.EndUpdate();

                        break;
                    }
                case Z64Object.EntryType.Texture:
                    {
                        tabControl1.SelectedIndex = 2;
                        var tex = (Z64Object.TextureHolder)holder;
                        if ((tex.Format != N64.N64TexFormat.CI4 && tex.Format != N64.N64TexFormat.CI8) || tex.Tlut != null)
                            pic_texture.Image = tex.GetBitmap();
                        break;
                    }
                case Z64Object.EntryType.Unknown:
                    {
                        tabControl1.SelectedIndex = 4;

                        var provider = new Be.Windows.Forms.DynamicByteProvider(holder.GetData());;
                        hexBox1.ByteProvider = provider;
                        hexBox1.LineInfoOffset = new SegmentedAddress(_segment, _obj.OffsetOf(holder)).VAddr;
                        break;
                    }
                default: tabControl1.SelectedIndex = 0; break;
            }
            listView_map.Focus();
        }

        private void findDlistsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new AnalyzerSettingsForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                Z64ObjectAnalyzer.FindDlists(_obj, _data, _segment, form.Result);
                UpdateMap();
            }
        }
        private void analyzeDlistsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var errors = Z64ObjectAnalyzer.AnalyzeDlists(_obj, _data, _segment);
            if (errors.Count > 0)
            {
                StringWriter sw = new StringWriter();
                errors.ForEach(error => sw.WriteLine(error));
                TextForm form = new TextForm(SystemIcons.Warning, "Warning", sw.ToString());
                form.ShowDialog();
            }
            UpdateMap();
        }
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _obj.Entries.Clear();
            _obj.AddUnknow(_data.Length);
            _obj.SetData(_data);
            UpdateMap();
        }

        private void showInDisplayViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlist = GetCurrentHolder<Z64Object.DListHolder>();

            DListViewerForm.OpenInstance(_game);
            DListViewerForm.Instance.SetSegment(_segment, RDPRenderer.Segment.FromBytes(_data, "[Selected Dlist]"));
            DListViewerForm.Instance.SetAddress(new SegmentedAddress(_segment, _obj.OffsetOf(dlist)).VAddr);
        }

        private void disassemblySettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisasmSettingsForm.OpenInstance();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exportJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "";
            saveFileDialog1.Filter = $"{Filters.JSON}|{Filters.ALL}";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string json = _obj.GetJSON();
                File.WriteAllText(saveFileDialog1.FileName, json);
            }
        }

        private void importJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = $"{Filters.JSON}|{Filters.ALL}";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string json = File.ReadAllText(openFileDialog1.FileName);
                _obj = Z64Object.FromJson(json);
                _obj.SetData(_data);
                UpdateMap();
            }
        }

        private void exportCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "";
            saveFileDialog1.Filter = Filters.C;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StringWriter sw = new StringWriter();
                foreach (var entry in _obj.Entries)
                {
                    int entryOff = _obj.OffsetOf(entry);
                    switch (entry.GetEntryType())
                    {
                        case Z64Object.EntryType.DList:
                            {
                                sw.WriteLine($"Gfx dlist_{entryOff:X8}[] = \r\n{{");
                                bool oldStatic = RDPDisassembler.Configuration.Static;

                                RDPDisassembler.Configuration.Static = true;
                                RDPDisassembler dis = new RDPDisassembler(entry.GetData(), new SegmentedAddress(_segment, _obj.OffsetOf(entry)).VAddr);
                                dis.Disassemble().ForEach(l => sw.WriteLine($"    {l}")); ;

                                RDPDisassembler.Configuration.Static = oldStatic;
                                sw.WriteLine("};");
                                break;
                            }
                        case Z64Object.EntryType.Vertex:
                            {
                                sw.WriteLine($"Vtx_t vertices_{entryOff:X8}[] = \r\n{{");

                                var vtx = (Z64Object.VertexHolder)entry;
                                vtx.Vertices.ForEach(v => sw.WriteLine($"    {{ {v.X}, {v.Y}, {v.Z}, 0x{v.Flag:X4}, {v.TexX}, {v.TexY}, {v.R}, {v.G}, {v.B}, {v.A} }},"));

                                sw.WriteLine("};");
                                break;
                            }
                        case Z64Object.EntryType.Texture:
                            {
                                sw.WriteLine($"u8 tex_{entryOff:X8}[] = \r\n{{");

                                var tex = entry.GetData();
                                for (int i = 0; i < tex.Length; i+= 16)
                                {
                                    sw.Write("    ");
                                    for (int j = 0; j < 16 && i + j < tex.Length; j++)
                                        sw.Write($"0x{tex[i + j]:X2}, ");
                                    sw.Write("\r\n");
                                }

                                sw.WriteLine("};");
                                break;
                            }
                        case Z64Object.EntryType.Unknown:
                            {
                                sw.WriteLine($"u8 unk_{entryOff:X8}[] = \r\n{{");

                                var bytes = entry.GetData();
                                for (int i = 0; i < bytes.Length; i += 16)
                                {
                                    sw.Write("    ");
                                    for (int j = 0; j < 16 && i+j < bytes.Length; j++)
                                        sw.Write($"0x{bytes[i + j]:X2}, ");
                                    sw.Write("\r\n");
                                }

                                sw.WriteLine("};");
                                break;
                            }
                        default: throw new Exception("Invalid Entry Type");
                    }

                    sw.Write("\r\n");
                }

                File.WriteAllText(saveFileDialog1.FileName, sw.ToString());
            }
        }
    }
}
