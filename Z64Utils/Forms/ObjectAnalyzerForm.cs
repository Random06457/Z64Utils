﻿using System;
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
    public partial class ObjectAnalyzerForm : MicrosoftFontForm
    {
        byte[] _data;
        Z64Object _obj;
        int _segment;
        Z64Game _game;

        public ObjectAnalyzerForm(Z64Game game, Z64File file, int segmentId)
        {
            InitializeComponent();

            segmentId = Math.Clamp(segmentId, 0, 15);

            _game = game;
            _data = file.Data;
            _obj = new Z64Object(_game, file);
            _segment = segmentId;


            // setup page stuff
            tabPage_text.Text = tabPage_texture.Text = tabPage_vtx.Text = "Data";
            tabPage_unknow.Text = "Hex";
            _defaultTabItemSize = tabControl1.ItemSize;
            _defaultTabSizeMode = tabControl1.SizeMode;
            _defaultTabAppearance = tabControl1.Appearance;
            SetTabControlVisible(false);

            MinimumSize = new Size(Width, Height);

            UpdateMap();
        }



        Size _defaultTabItemSize = new Size(0, 0);
        TabSizeMode _defaultTabSizeMode;
        TabAppearance _defaultTabAppearance;
        void SetTabControlVisible(bool visible)
        {
            if (visible)
            {
                tabControl1.ItemSize = _defaultTabItemSize;
                tabControl1.SizeMode = _defaultTabSizeMode;
                tabControl1.Appearance = _defaultTabAppearance;
            }
            else
            {
                tabControl1.ItemSize = new Size(0, 1);
                tabControl1.SizeMode = TabSizeMode.Fixed;
                tabControl1.Appearance = TabAppearance.FlatButtons;
            }
        }

        void SelectTabPage(TabPage page)
        {
            SetTabControlVisible(page != tabPage_empty && page != tabPage_unknow);

            tabControl1.TabPages.Clear();
            if (page != tabPage_unknow)
                tabControl1.TabPages.Add(page);
            tabControl1.TabPages.Add(tabPage_unknow);

            tabControl1.SelectedTab = page;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Shift | Keys.F))
            {
                var form = new AnalyzerSettingsForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Z64ObjectAnalyzer.FindDlists(_obj, _data, _segment, form.Result);
                    UpdateMap();
                }
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Shift | Keys.A))
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
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void UpdateDisassembly()
        {
            var dlist = GetCurrentHolder<Z64Object.DListHolder>();
            if (dlist != null)
            {
                try
                {
                    F3DZEX.Disassembler disas = new F3DZEX.Disassembler(new F3DZEX.Command.Dlist(dlist.GetData(), new SegmentedAddress(_segment, _obj.OffsetOf(dlist)).VAddr));
                    var lines = disas.Disassemble();
                    StringWriter sw = new StringWriter();
                    lines.ForEach(s => sw.WriteLine(s));
                    textBox_holderInfo.Text = sw.ToString();
                }
                catch (Exception ex)
                {
                    textBox_holderInfo.Text = "ERROR";
                }
            }
        }


        private void UpdateMap()
        {
            listView_map.Items.Clear();
            listView_map.BeginUpdate();

            string compare = textBox_filter.Text.ToLower();

            for (int i = 0; i < _obj.Entries.Count; i++)
            {
                var entry = _obj.Entries[i];
                string addrStr = $"{new SegmentedAddress(_segment, _obj.OffsetOf(entry)).VAddr:X8}";
                string entryStr = $"{addrStr}{entry.Name}{entry.GetEntryType()}".ToLower();

                if (entryStr.Contains(compare))
                {
                    var item = listView_map.Items.Add(addrStr);
                    item.SubItems.Add(entry.Name);
                    item.SubItems.Add(entry.GetEntryType().ToString());
                    item.Tag = i;
                }
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

            idx = (int)listView_map.Items[idx].Tag;

            return (typeof(T) == typeof(Z64Object.ObjectHolder) || typeof(T) == _obj.Entries[idx].GetType())
                ? (T)_obj.Entries[idx]
                : null;
        }

        private void listView_map_SelectedIndexChanged(object sender, EventArgs e)
        {
            var holder = GetCurrentHolder<Z64Object.ObjectHolder>();
            if (holder == null)
            {
                tabControl1.SelectedTab = tabPage_empty;
                return;
            }

            openInDlistViewerMenuItem.Visible = 
            addToDlistViewerMenuItem.Visible =
            openSkeletonViewerMenuItem.Visible = false;


            var provider = new DynamicByteProvider(holder.GetData()); ;
            hexBox1.ByteProvider = provider;
            hexBox1.LineInfoOffset = new SegmentedAddress(_segment, _obj.OffsetOf(holder)).VAddr;


            switch (holder.GetEntryType())
            {
                case Z64Object.EntryType.DList:
                    {
                        openInDlistViewerMenuItem.Visible =
                        addToDlistViewerMenuItem.Visible = true;

                        SelectTabPage(tabPage_text);
                        UpdateDisassembly();
                        break;
                    }
                case Z64Object.EntryType.Vertex:
                    {
                        SelectTabPage(tabPage_vtx);
                        var vtx = (Z64Object.VertexHolder)holder;

                        listView_vtx.BeginUpdate();
                        listView_vtx.Items.Clear();
                        uint addr = new SegmentedAddress(_segment, _obj.OffsetOf(holder)).VAddr;
                        for (int i = 0; i < vtx.Vertices.Count; i++)
                        {
                            var item = listView_vtx.Items.Add($"{addr:X8}");
                            item.SubItems.Add($"{vtx.Vertices[i].X}, {vtx.Vertices[i].Y}, {vtx.Vertices[i].Z}");
                            item.SubItems.Add($"0x{vtx.Vertices[i].Flag:X8}");
                            item.SubItems.Add($"{vtx.Vertices[i].TexX >> 5}, {vtx.Vertices[i].TexY >> 5}");
                            item.SubItems.Add($"{vtx.Vertices[i].R}, {vtx.Vertices[i].G}, {vtx.Vertices[i].B}, {vtx.Vertices[i].A}");
                            addr += 0x10;
                        }
                        listView_vtx.EndUpdate();

                        break;
                    }
                case Z64Object.EntryType.Texture:
                    {
                        SelectTabPage(tabPage_texture);
                        var tex = (Z64Object.TextureHolder)holder;

                        label_textureInfo.Text = $"{tex.Width}x{tex.Height} {tex.Format}";

                        if ((tex.Format != N64.N64TexFormat.CI4 && tex.Format != N64.N64TexFormat.CI8) || tex.Tlut != null)
                            pic_texture.Image = tex.GetBitmap();

                        if (tex.Tlut != null)
                        {
                            uint tlutAddr = new SegmentedAddress(_segment, _obj.OffsetOf(tex.Tlut)).VAddr;
                            label_textureInfo.Text += $" (TLUT : 0x{tlutAddr:X8} {tex.Tlut.Width}x{tex.Tlut.Height} {tex.Tlut.Format})";
                        }
                        break;
                    }
                case Z64Object.EntryType.CollisionHeader:
                    {
                        tabControl1.SelectedTab = tabPage_text;
                        var colHdr = (Z64Object.ColHeaderHolder)holder;

                        StringWriter sw = new StringWriter();
                        sw.WriteLine($"Bounds: " +
                            $"( {colHdr.MinBounds.X}, {colHdr.MinBounds.Y}, {colHdr.MinBounds.Z} ) : " +
                            $"( {colHdr.MaxBounds.X}, {colHdr.MaxBounds.Y}, {colHdr.MaxBounds.Z} )");
                        sw.WriteLine($"Vertices Count: {colHdr.NbVertices}");
                        sw.WriteLine($"Vertices: 0x{colHdr.VertexListSeg.VAddr:X8}");
                        sw.WriteLine($"Polygons Count: {colHdr.NbPolygons}");
                        sw.WriteLine($"Polygons: 0x{colHdr.PolyListSeg.VAddr:X8}");
                        sw.WriteLine($"SurfaceTypes: 0x{colHdr.SurfaceTypeSeg.VAddr:X8}");
                        sw.WriteLine($"CamData: 0x{colHdr.CamDataSeg.VAddr:X8}");
                        sw.WriteLine($"WaterBoxes Count: {colHdr.NbWaterBoxes}");
                        sw.WriteLine($"WaterBoxes: 0x{colHdr.WaterBoxSeg.VAddr:X8}");
                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.CollisionVertices:
                    {
                        tabControl1.SelectedTab = tabPage_text;
                        var vertices = (Z64Object.CollisionVerticesHolder)holder;

                        StringWriter sw = new StringWriter();
                        sw.WriteLine($"Vertices:");
                        foreach (var vertex in vertices.Points)
                            sw.WriteLine($"{{ {vertex.X}, {vertex.Y}, {vertex.Z} }}");

                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.CollisionPolygons:
                    {
                        tabControl1.SelectedTab = tabPage_text;
                        var polygons = (Z64Object.CollisionPolygonsHolder)holder;

                        StringWriter sw = new StringWriter();
                        sw.WriteLine($"Polygons:");
                        foreach (var poly in polygons.CollisionPolys)
                            sw.WriteLine($"{{ {poly.Type:4}, " +
                                $"{{ 0x{poly.Data[0]:X04}, 0x{poly.Data[1]:X04}, 0x{poly.Data[2]:X04} }}, " +
                                $"{{ {poly.Normal.X}, {poly.Normal.Y}, {poly.Normal.Z} }}, " +
                                $"{poly.Dist} }}");

                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.CollisionSurfaceTypes:
                    {
                        tabControl1.SelectedTab = tabPage_text;
                        var surfaceTypes = (Z64Object.CollisionSurfaceTypesHolder)holder;

                        StringWriter sw = new StringWriter();
                        sw.WriteLine($"Surface Types:");
                        foreach (var surfType in surfaceTypes.SurfaceTypes)
                            sw.WriteLine($"{{ 0x{surfType[0]:X08} , 0x{surfType[1]:X08} }}");

                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.CollisionCamData:
                    {
                        tabControl1.SelectedTab = tabPage_text;
                        var camData = (Z64Object.CollisionCamDataHolder)holder;

                        StringWriter sw = new StringWriter();
                        sw.WriteLine($"Camera Data:");
                        foreach (var data in camData.CamData)
                        {
                            sw.WriteLine($"{{");
                            sw.WriteLine($"    Camera Type: 0x{data.CameraSType:X02}");
                            sw.WriteLine($"    Number of Cameras: {data.NumCameras}");
                            sw.WriteLine($"    Data: 0x{data.CamPosData.VAddr:X08}");
                            sw.WriteLine($"}}");
                        }
                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.WaterBox:
                    {
                        tabControl1.SelectedTab = tabPage_text;
                        var waterBoxes = (Z64Object.WaterBoxHolder)holder;

                        StringWriter sw = new StringWriter();
                        sw.WriteLine($"Waterboxes:");
                        foreach (var waterBox in waterBoxes.WaterBoxes)
                        {
                            sw.WriteLine($"{{");
                            sw.WriteLine($"    Dimensions: {waterBox.XLength}x{waterBox.ZLength}");
                            sw.WriteLine($"    Height: {waterBox.YSurface}");
                            sw.WriteLine($"    Points: " +
                                $"({waterBox.XMin}, {waterBox.ZMin}), " +
                                $"({waterBox.XMin + waterBox.XLength}, {waterBox.ZMin}), " +
                                $"({waterBox.XMin}, {waterBox.ZMin + waterBox.ZLength}), " +
                                $"({waterBox.XMin + waterBox.XLength}, {waterBox.ZMin + waterBox.ZLength})");
                            sw.WriteLine($"    Properties: {waterBox.Properties:X08}");
                            sw.WriteLine($"}}");
                        }
                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.MatAnimTextureIndexList:
                    {
                        tabControl1.SelectedTab = tabPage_text;
                        var textureIndexList = (Z64Object.MatAnimTextureIndexListHolder)holder;

                        StringWriter sw = new StringWriter();
                        sw.WriteLine("Texture Indices:");
                        foreach (var index in textureIndexList.TextureIndices)
                        {
                            sw.WriteLine($"  {index:X02}");
                        }
                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.MatAnimTextureList:
                    {
                        tabControl1.SelectedTab = tabPage_text;
                        var textureList = (Z64Object.MatAnimTextureListHolder)holder;

                        StringWriter sw = new StringWriter();
                        sw.WriteLine("Texture List:");
                        foreach (var segment in textureList.TextureSegments)
                        {
                            sw.WriteLine($"  0x{segment.VAddr:X08}");
                        }
                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.MatAnimHeader:
                    {
                        tabControl1.SelectedTab = tabPage_text;
                        var header = (Z64Object.MatAnimHeaderHolder)holder;
                        StringWriter sw = new StringWriter();

                        var signedSegId = (sbyte)header.SegmentId;
                        var trueSeg = ((signedSegId < 0) ? -signedSegId : signedSegId) + 7;
                        var typeStr = "INVALID";
                        switch (header.Type)
                        {
                            case 0: typeStr = "Single Tex Scroll"; break;
                            case 1: typeStr = "Two Tex Scroll"; break;
                            case 2: typeStr = "Color No Interpolation"; break;
                            case 3: typeStr = "Color Linear Interpolation"; break;
                            case 4: typeStr = "Color Lagrange Interpolation"; break;
                            case 5: typeStr = "Tex Cycle"; break;
                        }
                        
                        sw.WriteLine($"Segment: 0x{header.SegmentId:X2} (0x{trueSeg:X2})");
                        sw.WriteLine($"Type: {typeStr}");
                        sw.WriteLine($"Params Seg: 0x{header.ParamsSeg.VAddr:X8}");

                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.MatAnimTexScrollParams:
                    {
                        tabControl1.SelectedTab = tabPage_text;
                        var scrollParams = (Z64Object.MatAnimTexScrollParamsHolder)holder;
                        StringWriter sw = new StringWriter();

                        sw.WriteLine($"StepX  : 0x{scrollParams.StepX:X2}");
                        sw.WriteLine($"StepY  : 0x{scrollParams.StepY:X2}");
                        sw.WriteLine($"Width  : 0x{scrollParams.Width:X2}");
                        sw.WriteLine($"Height : 0x{scrollParams.Height:X2}");
                        
                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.MatAnimColorParams:
                    {
                        tabControl1.SelectedTab = tabPage_text;
                        var colorParams = (Z64Object.MatAnimColorParamsHolder)holder;
                        StringWriter sw = new StringWriter();

                        sw.WriteLine($"KeyFrame Length : 0x{colorParams.KeyFrameLength:X4}");
                        sw.WriteLine($"KeyFrame Count  : 0x{colorParams.KeyFrameCount:X4}");
                        sw.WriteLine($"Prim Colors Seg : 0x{colorParams.PrimColors.VAddr:X8}");
                        sw.WriteLine($"Env Colors Seg  : 0x{colorParams.EnvColors.VAddr:X8}");
                        sw.WriteLine($"KeyFrames Seg   : 0x{colorParams.KeyFrames.VAddr:X8}");

                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.MatAnimTexCycleParams:
                    {
                        tabControl1.SelectedTab = tabPage_text;
                        var cycleParams = (Z64Object.MatAnimTexCycleParamsHolder)holder;
                        StringWriter sw = new StringWriter();

                        sw.WriteLine($"KeyFrame Length  : 0x{cycleParams.KeyFrameLength:X4}");
                        sw.WriteLine($"Texture List Seg : 0x{cycleParams.TextureList.VAddr:X8}");
                        sw.WriteLine($"Texture Index List Seg : 0x{cycleParams.TextureIndexList.VAddr:X8}");

                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.Mtx:
                    {
                        SelectTabPage(tabPage_text);
                        var matrices = (Z64Object.MtxHolder)holder;
                        StringWriter sw = new StringWriter();
                        for (int n = 0; n < matrices.Matrices.Count; n++)
                        {
                            sw.WriteLine($" ┌                                                ┐ ");
                            for (int i = 0; i < 4; i++)
                            {
                                var values = "";
                                for (int j = 0; j < 4; j++)
                                {
                                    values += $"0x{ArrayUtil.ReadUint32BE(matrices.Matrices[n].GetBuffer(), 4*(4 * i + j)):X08}";
                                    if (j != 3)
                                        values += $"  ";
                                }
                                sw.WriteLine($" │ {values} │ ");
                            }
                            sw.WriteLine($" └                                                ┘ ");
                        }
                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.SkeletonHeader:
                    {
                        openSkeletonViewerMenuItem.Visible = true;
                        SelectTabPage(tabPage_text);
                        var skel = (Z64Object.SkeletonHolder)holder;
                        StringWriter sw = new StringWriter();
                        sw.WriteLine($"Limbs: 0x{skel.LimbsSeg.VAddr:X8}");
                        sw.WriteLine($"Limb Count: {skel.LimbCount}");
                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.FlexSkeletonHeader:
                    {
                        openSkeletonViewerMenuItem.Visible = true;
                        SelectTabPage(tabPage_text);
                        var skel = (Z64Object.FlexSkeletonHolder)holder;

                        StringWriter sw = new StringWriter();
                        sw.WriteLine($"Limbs: 0x{skel.LimbsSeg.VAddr:X8}");
                        sw.WriteLine($"Limb Count: {skel.LimbCount}");
                        sw.WriteLine($"DList Count: {skel.DListCount}");

                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.SkeletonLimbs:
                    {
                        SelectTabPage(tabPage_text);
                        var limbs = (Z64Object.SkeletonLimbsHolder)holder;

                        StringWriter sw = new StringWriter();
                        sw.WriteLine($"Limbs:");
                        foreach (var limb in limbs.LimbSegments)
                            sw.WriteLine($"0x{limb.VAddr:X8}");

                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.StandardLimb:
                case Z64Object.EntryType.LODLimb:
                case Z64Object.EntryType.SkinLimb:
                    {
                        SelectTabPage(tabPage_text);
                        var limb = (Z64Object.SkeletonLimbHolder)holder;

                        StringWriter sw = new StringWriter();
                        sw.WriteLine($"Position: {{ {limb.JointX}, {limb.JointY}, {limb.JointZ} }}");
                        sw.WriteLine($"Child: 0x{limb.Child:X2}");
                        sw.WriteLine($"Sibling: 0x{limb.Sibling:X2}");
                        if (limb.Type != Z64Object.EntryType.SkinLimb)
                            sw.WriteLine($"DList : 0x{limb.DListSeg.VAddr:X8}");
                        if (limb.Type == Z64Object.EntryType.LODLimb)
                            sw.WriteLine($"Far DList : 0x{limb.DListFarSeg.VAddr:X8}");
                        else if (limb.Type == Z64Object.EntryType.SkinLimb)
                        {
                            sw.WriteLine($"Data Type : {limb.SegmentType}"); // TODO describe data instead of printing number?
                            sw.WriteLine($"Data Segment : 0x{limb.SkinSeg.VAddr:X8}");
                        }
                        
                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.LinkAnimationHeader:
                    {
                        tabControl1.SelectedTab = tabPage_text;
                        var anim = (Z64Object.LinkAnimationHolder)holder;

                        StringWriter sw = new StringWriter();
                        sw.WriteLine($"Frame Count: {anim.FrameCount}");
                        sw.WriteLine($"Animation Data Segment: 0x{anim.LinkAnimationSegment.VAddr:X8}");

                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.AnimationHeader:
                    {
                        SelectTabPage(tabPage_text);
                        var anim = (Z64Object.AnimationHolder)holder;

                        StringWriter sw = new StringWriter();
                        sw.WriteLine($"Frame Count: {anim.FrameCount}");
                        sw.WriteLine($"Frame Data: 0x{anim.FrameData.VAddr:X8}");
                        sw.WriteLine($"Joint Indices: 0x{anim.JointIndices.VAddr:X8}");
                        sw.WriteLine($"Static Index Max: 0x{anim.StaticIndexMax}");

                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.JointIndices:
                    {
                        SelectTabPage(tabPage_text);
                        var joints = (Z64Object.AnimationJointIndicesHolder)holder;

                        StringWriter sw = new StringWriter();
                        sw.WriteLine($"Joints:");
                        foreach (var joint in joints.JointIndices)
                            sw.WriteLine($"{{ frameData[{joint.X}], frameData[{joint.Y}], frameData[{joint.Z}] }}");

                        textBox_holderInfo.Text = sw.ToString();
                        break;
                    }
                case Z64Object.EntryType.FrameData:
                case Z64Object.EntryType.Unknown:
                    {
                        SelectTabPage(tabPage_unknow);
                        break;
                    }
                default: SelectTabPage(tabPage_empty); break;
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

        

        private void openSkeletonViewerMenuItem_Click(object sender, EventArgs e)
        {
            var holder = GetCurrentHolder<Z64Object.ObjectHolder>();
            switch (holder.GetEntryType())
            {
                case Z64Object.EntryType.FlexSkeletonHeader:
                    {
                        var skel = GetCurrentHolder<Z64Object.FlexSkeletonHolder>();
                        List<Z64Object.AnimationHolder> anims = new List<Z64Object.AnimationHolder>();
                        _obj.Entries.ForEach(e =>
                        {
                            if (e is Z64Object.AnimationHolder)
                                anims.Add((Z64Object.AnimationHolder)e);
                        });

                        SkeletonViewerForm form = new SkeletonViewerForm(_game);
                        form.SetSegment(_segment, F3DZEX.Memory.Segment.FromBytes("[Current Object]", _data));
                        form.SetSkeleton(skel, anims);
                        form.Show();
                        break;
                    }
                case Z64Object.EntryType.SkeletonHeader:
                    {
                        var skel = GetCurrentHolder<Z64Object.SkeletonHolder>();
                        List<Z64Object.AnimationHolder> anims = new List<Z64Object.AnimationHolder>();
                        _obj.Entries.ForEach(e =>
                        {
                            if (e is Z64Object.AnimationHolder)
                                anims.Add((Z64Object.AnimationHolder)e);
                        });

                        SkeletonViewerForm form = new SkeletonViewerForm(_game);
                        form.SetSegment(_segment, F3DZEX.Memory.Segment.FromBytes("[Current Object]", _data));
                        form.SetSkeleton(skel, anims);
                        form.Show();
                        break;
                    }
            }
        }

        private void addToDisplayViewerMenuItem_Click(object sender, EventArgs e)
        {
            var holder = GetCurrentHolder<Z64Object.ObjectHolder>();
            if (holder.GetEntryType() == Z64Object.EntryType.DList)
            {
                DListViewerForm.OpenInstance(_game);
                DListViewerForm.Instance.SetSegment(_segment, F3DZEX.Memory.Segment.FromBytes("[Selected Dlist]", _data));

                var dlist = GetCurrentHolder<Z64Object.DListHolder>();
                DListViewerForm.Instance.AddDList(new SegmentedAddress(_segment, _obj.OffsetOf(dlist)).VAddr);
            }
        }

        private void openInDisplayViewerMenuItem_Click(object sender, EventArgs e)
        {
            var holder = GetCurrentHolder<Z64Object.ObjectHolder>();
            if (holder.GetEntryType() == Z64Object.EntryType.DList)
            {
                DListViewerForm.OpenInstance(_game);
                DListViewerForm.Instance.SetSegment(_segment, F3DZEX.Memory.Segment.FromBytes("[Selected Dlist]", _data));

                var dlist = GetCurrentHolder<Z64Object.DListHolder>();
                DListViewerForm.Instance.SetSingleDlist(new SegmentedAddress(_segment, _obj.OffsetOf(dlist)).VAddr);
            }

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
                                sw.WriteLine($"Gfx {entry.Name}[] = \r\n{{");
                                bool oldStatic = F3DZEX.Disassembler.StaticConfig.Static;

                                F3DZEX.Disassembler.StaticConfig.Static = true;
                                F3DZEX.Disassembler dis = new F3DZEX.Disassembler(new F3DZEX.Command.Dlist(entry.GetData(), new SegmentedAddress(_segment, _obj.OffsetOf(entry)).VAddr));
                                dis.Disassemble().ForEach(l => sw.WriteLine($"    {l}")); ;

                                F3DZEX.Disassembler.StaticConfig.Static = oldStatic;
                                sw.WriteLine("};");
                                break;
                            }
                        case Z64Object.EntryType.Vertex:
                            {
                                sw.WriteLine($"Vtx_t {entry.Name}[] = \r\n{{");

                                var vtx = (Z64Object.VertexHolder)entry;
                                vtx.Vertices.ForEach(v => sw.WriteLine($"    {{ {v.X}, {v.Y}, {v.Z}, 0x{v.Flag:X4}, {v.TexX}, {v.TexY}, {v.R}, {v.G}, {v.B}, {v.A} }},"));

                                sw.WriteLine("};");
                                break;
                            }
                        case Z64Object.EntryType.Texture:
                            {
                                sw.WriteLine($"u8 {entry.Name}[] = \r\n{{");

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
                                sw.WriteLine($"u8 {entry.Name}[] = \r\n{{");

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
                        case Z64Object.EntryType.LinkAnimationHeader:
                            {
                                var holder = (Z64Object.LinkAnimationHolder)entry;
                                sw.WriteLine($"LinkAnimationHeader {entry.Name} = {{ {{ {holder.FrameCount} }}, 0x{holder.LinkAnimationSegment} }};");
                                break;
                            }
                        case Z64Object.EntryType.AnimationHeader:
                            {
                                var holder = (Z64Object.AnimationHolder)entry;
                                sw.WriteLine($"AnimationHeader {entry.Name} = {{ {{ {holder.FrameCount} }}, 0x{holder.FrameData.VAddr}, 0x{holder.JointIndices.VAddr}, {holder.StaticIndexMax} }};");
                                break;
                            }
                        case Z64Object.EntryType.FrameData:
                            {
                                var holder = (Z64Object.AnimationFrameDataHolder)entry;
                                sw.WriteLine($"s16 {entry.Name}[] = {{");
                                for (int i = 0; i < 8; i += 8)
                                {
                                    sw.Write("    ");
                                    for (int j = 0; j < 8 && i+j < holder.FrameData.Length; j++)
                                        sw.Write($"0x{holder.FrameData[i + j]:X4}, ");
                                }
                                sw.WriteLine("};");
                                break;
                            }
                        case Z64Object.EntryType.JointIndices:
                            {
                                var holder = (Z64Object.AnimationJointIndicesHolder)entry;
                                sw.WriteLine($"JointIndex {entry.Name}[] = {{");
                                foreach (var joint in holder.JointIndices)
                                    sw.WriteLine($"    {{ {joint.X}, {joint.Y}, {joint.Z} }}, ");
                                sw.WriteLine("};");
                                break;
                            }
                        case Z64Object.EntryType.SkeletonHeader:
                            {
                                var holder = (Z64Object.SkeletonHolder)entry;
                                sw.WriteLine($"SkeletonHeader {entry.Name} = {{ 0x{holder.LimbsSeg.VAddr}, {holder.LimbCount} }};");
                                break;
                            }
                        case Z64Object.EntryType.FlexSkeletonHeader:
                            {
                                var holder = (Z64Object.FlexSkeletonHolder)entry;
                                sw.WriteLine($"FlexSkeletonHeader {entry.Name} = {{ {{ 0x{holder.LimbsSeg.VAddr}, {holder.LimbCount} }}, {holder.DListCount} }};");
                                break;
                            }
                        case Z64Object.EntryType.StandardLimb:
                            {
                                var holder = (Z64Object.SkeletonLimbHolder)entry;
                                sw.WriteLine($"StandardLimb {entry.Name} = {{ {{ {holder.JointX}, {holder.JointY}, {holder.JointZ} }}, {holder.Child}, {holder.Sibling}, 0x{holder.DListSeg.VAddr:X8} }};");
                                break;
                            }
                        case Z64Object.EntryType.LODLimb:
                            {
                                var holder = (Z64Object.SkeletonLimbHolder)entry;
                                sw.WriteLine($"LodLimb {entry.Name} = {{ {{ {holder.JointX}, {holder.JointY}, {holder.JointZ} }}, {holder.Child}, {holder.Sibling}, 0x{holder.DListSeg.VAddr:X8}, 0x{holder.DListFarSeg.VAddr:X8} }};");
                                break;
                            }
                        case Z64Object.EntryType.SkinLimb:
                            {
                                var holder = (Z64Object.SkeletonLimbHolder)entry;
                                sw.WriteLine($"SkinLimb {entry.Name} = {{ {{ {holder.JointX}, {holder.JointY}, {holder.JointZ} }}, {holder.Child}, {holder.Sibling}, 0x{holder.SegmentType}, 0x{holder.SkinSeg.VAddr:X8} }};");
                                break;
                            }
                        case Z64Object.EntryType.SkeletonLimbs:
                            {
                                var holder = (Z64Object.SkeletonLimbsHolder)entry;
                                sw.WriteLine($"void* {entry.Name}[] = {{");
                                foreach (var limb in holder.LimbSegments)
                                    sw.WriteLine($"0x{limb.VAddr:X8}, ");
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

        private void textBox_filter_TextChanged(object sender, EventArgs e)
        {
            UpdateMap();
        }
    }
}
