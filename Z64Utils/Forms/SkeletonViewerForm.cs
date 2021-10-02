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
using F3DZEX.Render.Zelda;

namespace Z64.Forms
{
    public partial class SkeletonViewerForm : MicrosoftFontForm
    {
        enum PlayState
        {
            Pause,
            Forward,
            Backward,
        }

        bool _formClosing = false;
        System.Timers.Timer _timer;
        PlayState _playState;
        string _dlistError = null;

        Z64Game _game;
        F3DZEX.Render.Renderer _renderer;
        SegmentEditorForm _segForm;
        DisasmForm _disasForm;
        SettingsForm _settingsForm;
        F3DZEX.Render.Renderer.Config _rendererCfg;

        SkeletonRenderer _skelRenderer;
        List<AnimationHolder> _anims;
        Func<int, SegmentedAddress, string> _limbNamer;

        public SkeletonViewerForm(Z64Game game)
        {
            _game = game;
            _rendererCfg = new F3DZEX.Render.Renderer.Config();
            _skelRenderer = new SkeletonRenderer();

            InitializeComponent();
            Toolkit.Init();

            _renderer = new F3DZEX.Render.Renderer(game, _rendererCfg);
            modelViewer.RenderCallback = RenderCallback;

            _timer = new System.Timers.Timer();
            _timer.Elapsed += Timer_Elapsed;

            RequestRender();

            FormClosing += (s, e) => {
                if (_timer.Enabled && !_formClosing)
                {
                    _formClosing = true;
                    e.Cancel = true;
                }
            };
            _playState = PlayState.Pause;
        }

        void RenderCallback(Matrix4 proj, Matrix4 view)
        {
            if (_dlistError != null)
            {
                toolStripErrorLabel.Text = _dlistError;
                return;
            }

            _renderer.RenderStart(proj, view);

            List<SkeletonLimbHolder> selNodes = new List<SkeletonLimbHolder>();
            if (treeView_hierarchy.SelectedNode != null)
                selNodes.Add(treeView_hierarchy.SelectedNode.Tag as SkeletonLimbHolder);

            _skelRenderer.RenderFrame(_renderer, selNodes, false);
            /*
            GL.PointSize(10.0f);
            GL.LineWidth(2.0f);
            GL.Color3(0xFF, 0, 0);
            _skelRenderer.RenderFrame(_renderer, selNodes, true);
            */

            toolStripErrorLabel.Text = _renderer.RenderFailed()
                ? $"RENDER ERROR AT 0x{_renderer.RenderErrorAddr:X8}! ({_renderer.ErrorMsg})"
                : "";
        }

        private void TreeView_hierarchy_AfterSelect(object sender, EventArgs e)
        {
            var tag = treeView_hierarchy.SelectedNode?.Tag ?? null;
            if (tag != null && tag is SkeletonLimbHolder)
            {
                var dlist = _skelRenderer.LimbDlists[_skelRenderer.Limbs.IndexOf((SkeletonLimbHolder)tag)];
                if (dlist != null)
                    _disasForm?.UpdateDlist(dlist);
                else
                    _disasForm?.SetMessage("Empty limb");
            }

            RequestRender();
        }

        private void RequestRender(object sender = null, EventArgs e = null)
        {
            _renderer.ClearErrors();

            toolStripErrorLabel.Text = "";

            modelViewer.Render();

        }






        public void SetSkeleton(SkeletonHolder skel, List<AnimationHolder> anims, Func<int, SegmentedAddress, string> limbNamer = null)
        {
            _dlistError = null;
            try
            {
                _skelRenderer.SetSkeleton(skel, _renderer.Memory, limbNamer);
            }
            catch (Exception ex)
            {
                _dlistError = ex.Message;
            }
            _anims = anims;
            _limbNamer = limbNamer;

            listBox_anims.Items.Clear();
            _anims.ForEach(a => listBox_anims.Items.Add(a.Name));

            UpdateSkeletonTreeView();
            RequestRender();
        }
        
        void UpdateSkeletonTreeView()
        {
            treeView_hierarchy.Nodes.Clear(); 
            var root = treeView_hierarchy.Nodes.Add("skeleton");
            root.Tag = _skelRenderer.Skeleton;

            if (_skelRenderer.Limbs.Count > 0)
                UpdateSkeletonTreeNode(root, 0, 0, 0, 0);
        }
        void UpdateSkeletonTreeNode(TreeNode parent, int i, int x, int y, int z)
        {
            var limb = _skelRenderer.Limbs[i];
            
            var node = parent.Nodes.Add(limb.Name);
            node.Tag = limb;

            if (limb.Sibling != 0xFF)
                UpdateSkeletonTreeNode(parent, limb.Sibling, x, y, z);
            if (limb.Child != 0xFF)
                UpdateSkeletonTreeNode(node, limb.Child, x + limb.JointX, y + limb.JointY, z + limb.JointZ);
        }

        private void ToolStripRenderCfgBtn_Click(object sender, System.EventArgs e)
        {
            if (_settingsForm != null)
            {
                _settingsForm.Activate();
            }
            else
            {
                _settingsForm = new SettingsForm(_rendererCfg);
                _settingsForm.FormClosed += (sender, e) => { _settingsForm = null; };
                _settingsForm.SettingsChanged += RequestRender;
                _settingsForm.Show();
            }
        }

        private void ToolStripDisassemblyBtn_Click(object sender, System.EventArgs e)
        {
            if (_disasForm != null)
            {
                _disasForm.Activate();
            }
            else
            {
                _disasForm = new DisasmForm(defaultText: "No limb selected");

                _disasForm.FormClosed += (sender, e) => _disasForm = null;
                _disasForm.Show();
            }

            var tag = treeView_hierarchy.SelectedNode?.Tag ?? null;
            if (tag != null && tag is SkeletonLimbHolder)
            {
                var dlist = _skelRenderer.LimbDlists[_skelRenderer.Limbs.IndexOf((SkeletonLimbHolder)tag)];
                _disasForm.UpdateDlist(dlist);
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

            if (_skelRenderer.Limbs!= null)
            {
                SetSkeleton(_skelRenderer.Skeleton, _anims, _limbNamer);
            }
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

                        SetSkeleton(_skelRenderer.Skeleton, _anims, _limbNamer);
                        RequestRender();
                    }
                };
                _segForm.FormClosed += (sender, e) => _segForm = null;
                _segForm.Show();
            }
        }

        private void listBox_anims_SelectedIndexChanged(object sender, EventArgs e)
        {
            button_playAnim.Enabled =
            button_playbackAnim.Enabled =
            trackBar_anim.Enabled = listBox_anims.SelectedIndex >= 0;


            if (listBox_anims.SelectedIndex >= 0)
            {
                var anim = _anims[listBox_anims.SelectedIndex];

                trackBar_anim.Minimum = 0;
                trackBar_anim.Maximum = anim.FrameCount - 1;
                trackBar_anim.Value = 0;

                _skelRenderer.SetAnim(anim, _renderer.Memory);
                RequestRender();
            }
        }

        private void trackBar_anim_ValueChanged(object sender, EventArgs e)
        {
            label_anim.Text = $"{trackBar_anim.Value}/{trackBar_anim.Maximum}";

            _skelRenderer.SetFrame(_renderer.Memory, trackBar_anim.Value);
            RequestRender();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.IsDisposed || _formClosing)
            {
                _timer.Stop();
                Invoke(new Action(Close));
                return;
            }

            Invoke(new Action(() =>
            {
                if (_playState == PlayState.Forward)
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
            }));
        }
        private void button_playbackAnim_Click(object sender, EventArgs e)
        {
            if (_playState == PlayState.Backward)
            {
                _playState = PlayState.Pause;
                _timer.Stop();
                button_playbackAnim.BackgroundImage = Properties.Resources.playback_icon;
            }
            else
            {
                _playState = PlayState.Backward;
                _timer.Start();
                button_playbackAnim.BackgroundImage = Properties.Resources.pause_icon;
                button_playAnim.BackgroundImage = Properties.Resources.play_icon;
            }
        }

        private void button_playAnim_Click(object sender, EventArgs e)
        {
            if (_playState == PlayState.Forward)
            {
                _playState = PlayState.Pause;
                _timer.Stop();
                button_playAnim.BackgroundImage = Properties.Resources.play_icon;
            }
            else
            {
                _playState = PlayState.Forward;
                _timer.Start();
                button_playAnim.BackgroundImage = Properties.Resources.pause_icon;
                button_playbackAnim.BackgroundImage = Properties.Resources.playback_icon;
            }
        }
    }
}
