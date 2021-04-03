using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using RDP;
using static Z64.Z64Object;
using System.IO;
using Syroot.BinaryData;
using System.Diagnostics;
using DList = System.Collections.Generic.List<System.Tuple<uint, F3DZEX.Command.CommandInfo>>;
using Common;

namespace Z64.Forms
{
    public partial class SkeletonViewerForm : MicrosoftFontForm
    {
        Z64Game _game;
        F3DZEX.Render.Renderer _renderer;
        SegmentEditorForm _segForm;
        DisasmForm _disasForm;
        RenderSettingsForm _settingsForm;
        F3DZEX.Render.Renderer.Config _rendererCfg;
        
        SkeletonHolder _skel;
        List<AnimationHolder> _anims;
        List<SkeletonLimbHolder> _limbs;
        List<DList> _limbDlists;

        AnimationHolder _curAnim;
        short[] _frameData;
        AnimationJointIndicesHolder.JointIndex[] _curJoints;

        public SkeletonViewerForm(Z64Game game)
        {
            _game = game;
            _rendererCfg = new F3DZEX.Render.Renderer.Config();

            InitializeComponent();
            Toolkit.Init();

            _renderer = new F3DZEX.Render.Renderer(game, _rendererCfg);
            modelViewer.RenderCallback = RenderCallback;

            RenderModelViewer();
        }

        void RenderLimb(int limbIdx, bool overlay = false)
        {
            _renderer.PushMatrix();


            if (_curAnim != null)
            {
                _renderer.LoadMatrix(CalcMatrix(_renderer.TopMatrix(), limbIdx));

                if (overlay)
                {
                    GL.Begin(PrimitiveType.Points);
                    GL.Vertex3(0, 0, 0);
                    GL.End();

                    if (_limbs[limbIdx].Child != 0xFF)
                    {
                        Vector3 childPos = GetLimbPos(_limbs[limbIdx].Child);
                        GL.Begin(PrimitiveType.Lines);
                        GL.Vertex3(0, 0, 0);
                        GL.Vertex3(childPos);
                        GL.End();
                    }
                }
                else
                {
                    var node = treeView_hierarchy.SelectedNode;
                    _renderer.SetHightlightEnabled(node?.Tag?.Equals(_limbs[limbIdx]) ?? false);

                    if (_limbDlists[limbIdx] != null)
                        _renderer.RenderDList(_limbDlists[limbIdx]);
                }


            }

            if (_limbs[limbIdx].Child != 0xFF)
                RenderLimb(_limbs[limbIdx].Child, overlay);

            _renderer.PopMatrix();

            if (_limbs[limbIdx].Sibling != 0xFF)
                RenderLimb(_limbs[limbIdx].Sibling, overlay);
        }

        void RenderCallback(Matrix4 proj, Matrix4 view)
        {
            _renderer.RenderStart(proj, view);
            RenderLimb(0);
            
            /*
            GL.PointSize(10.0f);
            GL.LineWidth(2.0f);
            GL.Color3(0xFF, 0, 0);
            RenderLimb(0, true);
            */
        }

        private void NewRender(object sender = null, EventArgs e = null)
        {
            _renderer.ClearErrors();

            //UpdateLimbs();
            //UpdateLimbsDlists();

            RenderModelViewer();
        }

        private void RenderModelViewer(object sender = null, EventArgs e = null)
        {
            modelViewer.Render();

            toolStripErrorLabel.Text = _renderer.RenderFailed()
                ? $"RENDER ERROR AT 0x{_renderer.RenderErrorAddr:X8}! ({_renderer.ErrorMsg})"
                : "";
        }






        public void SetSkeleton(SkeletonHolder skel, List<AnimationHolder> anims)
        {
            _skel = skel;
            _anims = anims;

            listBox_anims.Items.Clear();
            _anims.ForEach(a => listBox_anims.Items.Add(a.Name));

            UpdateSkeleton();
            NewRender();
        }

        void UpdateLimbsDlists()
        {
            _limbDlists = new List<DList>();
            _limbs.ForEach(l => _limbDlists.Add(l.DListSeg.VAddr != 0 ? _renderer.GetFullDlist(l.DListSeg.VAddr) : null));
        }

        // Updates skeleton -> limbs / limbs dlists -> matrices
        void UpdateSkeleton()
        {
            treeView_hierarchy.Nodes.Clear(); 
            var root = treeView_hierarchy.Nodes.Add("skeleton");
            root.Tag = _skel;

            byte[] limbsData = _renderer.Memory.ReadBytes(_skel.LimbsSeg, _skel.LimbCount * 4);

            var limbs = new SkeletonLimbsHolder("limbs", limbsData);

            _limbs = new List<SkeletonLimbHolder>();
            for (int i = 0; i < limbs.LimbSegments.Length; i++)
            {
                byte[] limbData = _renderer.Memory.ReadBytes(limbs.LimbSegments[i], SkeletonLimbHolder.ENTRY_SIZE);
                var limb = new SkeletonLimbHolder($"limb_{i}", limbData);
                _limbs.Add(limb);
            }

            UpdateLimbsDlists();
            UpdateLimbs();
        }

        // Updates limbs -> matrices
        void UpdateLimbs()
        {
            TreeNode skelNode = treeView_hierarchy.Nodes[0];

            if (_limbs.Count > 0)
                AddLimbRoutine(skelNode, 0, 0, 0, 0);

            UpdateMatrixBuf();
        }

        void AddLimbRoutine(TreeNode parent, int i, int x, int y, int z)
        {
            var node = parent.Nodes.Add($"limb_{i}");
            node.Tag = _limbs[i];

            if (_limbs[i].Sibling != 0xFF)
                AddLimbRoutine(parent, _limbs[i].Sibling, x, y, z);
            if (_limbs[i].Child != 0xFF)
                AddLimbRoutine(node, _limbs[i].Child, x + _limbs[i].JointX, y + _limbs[i].JointY, z + _limbs[i].JointZ);
        }



        float S16ToRad(short x) => x * (float)Math.PI / 0x7FFF;
        float S16ToDeg(short x) => x * 360.0f / 0xFFFF;
        float DegToRad(float x) => x * (float)Math.PI / 180.0f;

        short GetFrameData(int frameDataIdx) => _frameData[frameDataIdx < _curAnim.StaticIndexMax ? frameDataIdx : frameDataIdx + trackBar_anim.Value];
        Vector3 GetLimbPos(int limbIdx) => (limbIdx == 0)
                ? new Vector3(_curJoints[limbIdx].X, _curJoints[limbIdx].Y, _curJoints[limbIdx].Z)
                : new Vector3(_limbs[limbIdx].JointX, _limbs[limbIdx].JointY, _limbs[limbIdx].JointZ);

        // Update anims -> matrices
        void UpdateAnim()
        {
            trackBar_anim.Minimum = 0;
            trackBar_anim.Maximum = _curAnim.FrameCount-1;
            trackBar_anim.Value = 0;

            byte[] buff = _renderer.Memory.ReadBytes(_curAnim.JointIndices, (_limbs.Count+1) * AnimationJointIndicesHolder.ENTRY_SIZE);
            _curJoints = new AnimationJointIndicesHolder("joints", buff).JointIndices;

            int max = 0;
            foreach (var joint in _curJoints)
            {
                max = Math.Max(max, joint.X);
                max = Math.Max(max, joint.Y);
                max = Math.Max(max, joint.Z);
            }

            buff = _renderer.Memory.ReadBytes(_curAnim.FrameData, (max < _curAnim.StaticIndexMax ? max+1 : _curAnim.FrameCount+max) * 2);
            _frameData = new AnimationFrameDataHolder("framedata", buff).FrameData;

            UpdateMatrixBuf();
        }

        Matrix4 CalcMatrix(Matrix4 src, int limbIdx)
        {
            if (_curAnim == null)
                return src;

            Vector3 pos = GetLimbPos(limbIdx);

            short rotX = GetFrameData(_curJoints[limbIdx + 1].X);
            short rotY = GetFrameData(_curJoints[limbIdx + 1].Y);
            short rotZ = GetFrameData(_curJoints[limbIdx + 1].Z);

            src = Matrix4.CreateRotationX(S16ToRad(rotX)) *
                Matrix4.CreateRotationY(S16ToRad(rotY)) *
                Matrix4.CreateRotationZ(S16ToRad(rotZ)) *
                Matrix4.CreateTranslation(pos) *
                src;

            return src;
        }

        // Flex Only
        void UpdateMatrixBuf()
        {
            if (!(_skel is FlexSkeletonHolder flexSkel))
                return;

            byte[] mtxBuff = new byte[flexSkel.DListCount * Mtx.SIZE];

            using (MemoryStream ms = new MemoryStream(mtxBuff))
            {
                BinaryStream bw = new BinaryStream(ms, Syroot.BinaryData.ByteConverter.Big);


                //GL.GetFloat(GetPName.ModelviewMatrix, out Matrix4 curMtx);

                UpdateMatrixBuf(bw, 0, 0, Matrix4.Identity);
            }

            _renderer.Memory.Segments[0xD] = F3DZEX.Memory.Segment.FromBytes("[RESERVED] Anim Matrices", mtxBuff);
        }
        int UpdateMatrixBuf(BinaryStream bw, int limbIdx, int dlistIdx, Matrix4 src)
        {
            Matrix4 mtx = CalcMatrix(src, limbIdx);

            if (_limbDlists[limbIdx] != null)
            {
                bw.Seek(dlistIdx++ * Mtx.SIZE, SeekOrigin.Begin);
                Mtx.FromMatrix4(mtx).Write(bw);
            }


            if (_limbs[limbIdx].Child != 0xFF)
                dlistIdx = UpdateMatrixBuf(bw, _limbs[limbIdx].Child, dlistIdx, mtx);

            if (_limbs[limbIdx].Sibling != 0xFF)
                dlistIdx = UpdateMatrixBuf(bw, _limbs[limbIdx].Sibling, dlistIdx, src);

            return dlistIdx;
        }



        private void ToolStripRenderCfgBtn_Click(object sender, System.EventArgs e)
        {
            if (_settingsForm != null)
            {
                _settingsForm.Activate();
            }
            else
            {
                _settingsForm = new RenderSettingsForm(_rendererCfg);
                _settingsForm.FormClosed += (sender, e) => { _settingsForm = null; };
                _settingsForm.SettingsChanged += RenderModelViewer;
                _settingsForm.Show();
            }
        }

        private void SkeletonViewerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _disasForm?.Close();
            _segForm?.Close();
            _settingsForm?.Close();
        }

        public void SetSegment(int idx, F3DZEX.Memory.Segment seg)
        {
            if (idx < 0 || idx > F3DZEX.Memory.Segment.COUNT)
                throw new IndexOutOfRangeException();

            _renderer.Memory.Segments[idx] = seg;
        }
        private void ToolStripSegmentsBtn_Click(object sender, System.EventArgs e)
        {
            if (_segForm != null)
            {
                _segForm.Activate();
            }
            else
            {
                _segForm = new SegmentEditorForm(_game, _renderer);
                _segForm.SegmentsChanged += (sender, seg) =>
                {
                    int idx = (int)sender;
                    if (idx == 0xD)
                        MessageBox.Show("Error", "Cannot set segment 13 (reserved for animation matrices)");
                    else
                    {
                        _renderer.Memory.Segments[(int)sender] = seg;
                        NewRender();
                    }
                };
                _segForm.FormClosed += (sender, e) => _segForm = null;
                _segForm.Show();
            }
        }


        private void listBox_anims_SelectedIndexChanged(object sender, EventArgs e)
        {
            _curAnim = null;
            if (listBox_anims.SelectedIndex >= 0)
            {
                _curAnim = _anims[listBox_anims.SelectedIndex];
                UpdateAnim();
                NewRender();
            }
        }

        private void trackBar_anim_ValueChanged(object sender, EventArgs e)
        {
            label_anim.Text = $"{trackBar_anim.Value}/{trackBar_anim.Maximum}";
            UpdateMatrixBuf();
            NewRender();
        }

        private void button_playbackAnim_Click(object sender, EventArgs e)
        {
            timer1.Start();
            button_playbackAnim.Enabled = false;
            button_playAnim.Enabled = button_pauseAnim.Enabled = true;
        }

        private void button_playAnim_Click(object sender, EventArgs e)
        {
            timer1.Start();
            button_playAnim.Enabled = false;
            button_playbackAnim.Enabled = button_pauseAnim.Enabled = true;
        }

        private void button_pauseAnim_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            button_pauseAnim.Enabled = false;
            button_playAnim.Enabled = button_playbackAnim.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (button_playbackAnim.Enabled)
            {
                trackBar_anim.Value = trackBar_anim.Value < trackBar_anim.Maximum
                    ? trackBar_anim.Value + 1
                    : 0;
            }
            else
            {
                trackBar_anim.Value = trackBar_anim.Value > 0
                    ? trackBar_anim.Value - 1
                    : trackBar_anim.Maximum;
            }
        }

        
    }
}
