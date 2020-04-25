using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using RDP;
using N64;
using System.Text.RegularExpressions;
using System.IO;

namespace Z64
{
    [Serializable]
    public class Z64ObjectAnalyzerException : Exception
    {
        public Z64ObjectAnalyzerException() { }
        public Z64ObjectAnalyzerException(string message) : base(message) { }
        public Z64ObjectAnalyzerException(string message, Exception inner) : base(message, inner) { }
        protected Z64ObjectAnalyzerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public static class Z64ObjectAnalyzer
    {
        public class Config
        {
            public class OpCodePattern
            {
                public F3DZEX.OpCodeID ID { get; private set; }
                private List<Tuple<int, List<F3DZEX.OpCodeID>>> _pattern;

                private OpCodePattern()
                {

                }

                public bool Check(byte[] data, int off)
                {
                    if (data[off] != (byte)ID)
                        return false;

                    foreach (var item in _pattern)
                    {
                        int off2 = off + item.Item1*8;
                        if (off2 < 0 || off2 >= data.Length)
                            return false;

                        if (item.Item2.Count == 0)
                            continue;

                        if (item.Item2.FindAll(id => (byte)id == data[off2]).Count == 0)
                            return false;
                    }

                    return true;
                }

                private static bool ValidOpCodeID(string id)
                {
                    var values = Enum.GetValues(typeof(F3DZEX.OpCodeID));
                    foreach (var v in values)
                        if (v.ToString() == id)
                            return true;
                    return false;
                }
                public static OpCodePattern Parse(string exp)
                {
                    var ret = new OpCodePattern();

                    exp = exp.Replace(" ", "");
                    var parts = exp.Split(':');
                    if (parts.Length != 2)
                        return null;

                    if (!ValidOpCodeID(parts[0]))
                        return null;

                    ret.ID = (F3DZEX.OpCodeID)Enum.Parse(typeof(F3DZEX.OpCodeID), parts[0]);
                    var entries = parts[1].Split(',').ToList();
                    if (entries.FindAll(s => s == "*").Count != 1)
                        return null;

                    int idx = entries.IndexOf("*");

                    ret._pattern = new List<Tuple<int, List<F3DZEX.OpCodeID>>>();
                    for (int i = 0; i < entries.Count; i++)
                    {
                        if (entries[i] == "*")
                        {
                            ret._pattern.Add(new Tuple<int, List<F3DZEX.OpCodeID>>(0, new List<F3DZEX.OpCodeID>() { ret.ID }));
                            continue;
                        }

                        if (entries[i] == "?")
                        {
                            ret._pattern.Add(new Tuple<int, List<F3DZEX.OpCodeID>>(i-idx, new List<F3DZEX.OpCodeID>()));
                            continue;
                        }

                        var ids = new List<F3DZEX.OpCodeID>();

                        var idStr = entries[i].Split('|').ToList();
                        if (!idStr.TrueForAll(s => ValidOpCodeID(s)))
                            return null;
                        foreach (var s in idStr)
                            ids.Add((F3DZEX.OpCodeID)Enum.Parse(typeof(F3DZEX.OpCodeID), s));

                        ret._pattern.Add(new Tuple<int, List<F3DZEX.OpCodeID>>(i-idx, ids));
                    }

                    return ret;
                }
                public override string ToString()
                {
                    StringWriter sw = new StringWriter();

                    sw.Write($"{ID}: ");
                    _pattern.Sort((a, b) => a.Item1 >= b.Item1 ? 1 : -1);
                    for (int i = 0; i < _pattern.Count; i++)
                    {
                        var ids = _pattern[i].Item2;


                        if (_pattern[i].Item1 == 0)
                            sw.Write("*");
                        else if (ids.Count == 0)
                            sw.Write("?");
                        else
                            sw.Write(string.Join("|", ids.ToArray()));

                        if (i + 1 < _pattern.Count)
                            sw.Write(", ");
                    }

                    return sw.ToString();
                }
            }

            public List<F3DZEX.OpCodeID> ImprobableOpCodes { get; set; } = new List<F3DZEX.OpCodeID>();
            public List<OpCodePattern> Patterns { get; set; } = new List<OpCodePattern>();

        }

        private class ReservedRegion
        {
            public int Off;
            public int Size;
            public ReservedRegion(int off, int size)
            {
                Off = off;
                Size = size;
            }
            public override string ToString() => $"0x{Off:X8}(0x{Size:X})";
        }
        private static bool IsOverlap(int off, List<ReservedRegion> regions) => GetRegionIndex(off, regions) != -1;
        private static int GetRegionIndex(int off, List<ReservedRegion> regions)
        {
            for (int i = 0; i < regions.Count; i++)
            {
                if (off >= regions[i].Off && off < regions[i].Off + regions[i].Size)
                    return i;
            }
            return -1;
        }
        private static void AddRegion(List<ReservedRegion> regions, int curSegId, uint vaddr, int size, int totalSize)
        {
            var addr = new SegmentedAddress(vaddr);
            if (addr.Segmented && addr.SegmentId == curSegId && ((int)addr.SegmentOff + size) <= totalSize)
                regions.Add(new ReservedRegion((int)addr.SegmentOff, size));
        }
        private static void AddDlist(List<int> dlists, int curSegId, uint vaddr, int totalSize)
        {
            var addr = new SegmentedAddress(vaddr);
            if (addr.Segmented && addr.SegmentId == curSegId && (int)addr.SegmentOff < totalSize)
                dlists.Add((int)addr.SegmentOff);
        }


        private static Tuple<List<int>, List<ReservedRegion>> FindRegions(byte[] data, int segmentId, Config cfg)
        {
            List<ReservedRegion> regions = new List<ReservedRegion>();
            List<int> dlists = new List<int>();

            var codeEnds = Utils.FindData(data, new byte[] { (byte)F3DZEX.OpCodeID.G_ENDDL, 0, 0, 0, 0, 0, 0, 0, }, 8);
            codeEnds.Insert(0, 0);
            for (int i = 1; i < codeEnds.Count; i++)
            {
                int end = codeEnds[i];
                int texels = -1;
                uint half1 = 0xFFFFFFFF;

                for (int off = end; off >= codeEnds[i-1]+8; off -= 8)
                {
                    F3DZEX.OpCodeID op = (F3DZEX.OpCodeID)data[off];

                    if (IsOverlap(off, regions) || !IsOpCodeCorrect(data, off, cfg))
                        break;

                    switch (op)
                    {
                        case F3DZEX.OpCodeID.G_RDPHALF_1:
                            {
                                half1 = F3DZEX.DecodeCommand<F3DZEX.GRdpHalf>(data, off).word;
                                break;
                            }
                        case F3DZEX.OpCodeID.G_BRANCH_Z:
                            {
                                AddDlist(dlists, segmentId, half1, data.Length);
                                break;
                            }
                        case F3DZEX.OpCodeID.G_DL:
                            {
                                var gdl = F3DZEX.DecodeCommand<F3DZEX.GDl>(data, off);
                                AddDlist(dlists, segmentId, gdl.dl, data.Length);
                                break;
                            }
                        case F3DZEX.OpCodeID.G_VTX:
                            {
                                var gmtx = F3DZEX.DecodeCommand<F3DZEX.GVtx>(data, off);
                                AddRegion(regions, segmentId, gmtx.vaddr, gmtx.numv * 0x10, data.Length);
                                break;
                            }
                        case F3DZEX.OpCodeID.G_SETTIMG:
                            {
                                var settimg = F3DZEX.DecodeCommand<F3DZEX.GSetTImg>(data, off);
                                if (texels == -1)
                                    break;
                                AddRegion(regions, segmentId, settimg.imgaddr, N64Texture.GetTexSize(texels, settimg.siz), data.Length);
                                texels = -1;
                                break;
                            }
                        case F3DZEX.OpCodeID.G_LOADBLOCK:
                            {
                                var loadblock = F3DZEX.DecodeCommand<F3DZEX.GLoadBlock>(data, off);
                                texels = loadblock.texels+1;
                                break;
                            }
                        case F3DZEX.OpCodeID.G_LOADTLUT:
                            {
                                var loadtlut = F3DZEX.DecodeCommand<F3DZEX.GLoadTlut>(data, off);
                                texels = loadtlut.count+1;
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            return new Tuple<List<int>, List<ReservedRegion>>(dlists, regions);
        }

        private static bool IsOpCodeCorrect(byte[] data, int off, Config cfg)
        {
            F3DZEX.OpCodeID id = (F3DZEX.OpCodeID)data[off];

            // check invalid opcodes
            if (!F3DZEX.DEC_TABLE.ContainsKey(id))
                return false;

            // check improbable opcodes
            if (cfg.ImprobableOpCodes.Contains(id))
                return false;

            var patterns = cfg.Patterns.FindAll(p => p.ID == id);
            if (patterns.Count == 0)
                return true;
            foreach (var pattern in patterns)
                if (pattern.Check(data, off))
                    return true;

            return false;

        }
        
        private static List<int> FindDlistEntries(List<ReservedRegion> regions, List<int> confirmedDlists, byte[] data, Config cfg)
        {
            List<int> dlists = new List<int>();

            var codeEnds = Utils.FindData(data, new byte[] { (byte)F3DZEX.OpCodeID.G_ENDDL, 0, 0, 0, 0, 0, 0, 0, }, 8);
            codeEnds.Insert(0, 0);

            regions.Sort((a, b) => a.Off >= b.Off ? 1 : -1);

            for (int i = 1; i < codeEnds.Count; i++)
            {
                int end = codeEnds[i]+8;
                int start = codeEnds[i-1]+8;

                bool skip = false;
                foreach (var dl in confirmedDlists)
                {
                    if (dl >= start && dl < end)
                    {
                        dlists.Add(dl);
                        skip = true;
                        break;
                    }
                }
                if (skip)
                    continue;

                // check reserved regions
                foreach (var region in regions)
                {
                    int regionEnd = region.Off + region.Size;
                    if (regionEnd <= end && regionEnd >= start)
                        start = regionEnd;
                }

                // check opcodes
                for (int off = start; off < end; off += 8)
                {
                    if (!IsOpCodeCorrect(data, off, cfg))
                        start = off+8;
                }


                dlists.Add(start);
            }

            return dlists;
        }
        
        private static int GetTlutWidth(int texels)
        {
            for (int i = (int)Math.Sqrt(texels); i >=1; i--)
                if (texels % i == 0)
                    return Math.Max(i, texels / i);
            return 0;
        }
        

        public static void FindDlists(Z64Object obj, byte[] data, int segmentId, Config cfg)
        {
            obj.Entries.Clear();
            obj.AddUnknow(data.Length);

            var ret = FindRegions(data, segmentId, cfg);
            List<int> entries = FindDlistEntries(ret.Item2, ret.Item1, data, cfg);
            foreach (var start in entries)
            {
                for (int off = start; off < data.Length; off += 8)
                {
                    if (data[off] == (byte)F3DZEX.OpCodeID.G_ENDDL)
                    {
                        obj.AddDList(off + 8 - start, off:start);
                        break;
                    }
                }
            }

            obj.GroupUnkEntries();
            obj.FixNames();
            obj.SetData(data);
        }
        public static List<string> AnalyzeDlists(Z64Object obj, byte[] data, int segmentId)
        {
            List<string> errors = new List<string>();

            List<int> dlists = new List<int>();
            for (int i = 0; i < obj.Entries.Count; i++)
            {
                var entry = obj.Entries[i];
                if (entry.GetEntryType() == Z64Object.EntryType.DList)
                    dlists.Add(obj.OffsetOf(entry));
                else
                    obj.Entries[i] = new Z64Object.UnknowHolder($"unk_{obj.OffsetOf(entry):X8}", entry.GetData());
            }

            foreach (var dlist in dlists)
            {
                uint lastTexAddr = 0xFFFFFFFF;
                RDPEnum.G_IM_FMT lastFmt = (RDPEnum.G_IM_FMT)(-1);
                RDPEnum.G_IM_SIZ lastSiz = (RDPEnum.G_IM_SIZ)(-1);
                Z64Object.TextureHolder lastTlut = null;
                Z64Object.TextureHolder lastCiTex = null;

                bool exit = false;
                for (int i = dlist; i < data.Length && !exit; i += 8)
                {
                    F3DZEX.OpCodeID op = (F3DZEX.OpCodeID)data[i];
                    switch (op)
                    {
                        case F3DZEX.OpCodeID.G_QUAD:
                        case F3DZEX.OpCodeID.G_TRI2:
                        case F3DZEX.OpCodeID.G_TRI1:
                        case F3DZEX.OpCodeID.G_TEXRECTFLIP:
                        case F3DZEX.OpCodeID.G_TEXRECT:
                            {
                                if (lastCiTex != null && lastTlut != null)
                                {
                                    lastCiTex.Tlut = lastTlut;
                                    lastCiTex = null;
                                }
                                break;
                            }
                        case F3DZEX.OpCodeID.G_ENDDL:
                            {
                                exit = true;
                                break;
                            }
                        case F3DZEX.OpCodeID.G_VTX:
                            {
                                var gvtx = F3DZEX.DecodeCommand<F3DZEX.GVtx>(data, i);

                                var addr = new SegmentedAddress(gvtx.vaddr);
                                if (addr.Segmented && addr.SegmentId == segmentId)
                                {
                                    try
                                    {
                                        obj.AddVertices(gvtx.numv, off: (int)addr.SegmentOff);
                                    }
                                    catch (Exception ex)
                                    {
                                        errors.Add($"Error in Dlist 0x{new SegmentedAddress(segmentId, dlist).VAddr:X8} : {ex.Message}");
                                    }
                                }
                                break;
                            }
                        case F3DZEX.OpCodeID.G_SETTIMG:
                            {
                                var settimg = F3DZEX.DecodeCommand<F3DZEX.GSetTImg>(data, i);
                                lastTexAddr = settimg.imgaddr;
                                break;
                            }
                        case F3DZEX.OpCodeID.G_SETTILE:
                            {
                                var settile = F3DZEX.DecodeCommand<F3DZEX.GSetTile>(data, i);
                                if (settile.tile != RDPEnum.G_TX_tile.G_TX_LOADTILE)
                                {
                                    lastFmt = settile.fmt;
                                    lastSiz = settile.siz;
                                }
                                break;
                            }
                        case F3DZEX.OpCodeID.G_SETTILESIZE:
                            {
                                var settilesize = F3DZEX.DecodeCommand<F3DZEX.GLoadTile>(data, i);
                                var addr = new SegmentedAddress(lastTexAddr);

                                if ((int)lastFmt == -1 || (int)lastSiz == -1 || lastTexAddr == 0xFFFFFFFF)
                                    /* can't really thow an exception here since in some object files, there are two gsDPSetTileSize next to each other (see object_en_warp_uzu) */
                                    //throw new Z64ObjectAnalyzerException();
                                    break;

                                if (addr.Segmented && addr.SegmentId == segmentId)
                                {

                                    try
                                    {
                                        var tex = obj.AddTexture((int)(settilesize.lrs.Float() + 1), (int)(settilesize.lrt.Float() + 1), N64Texture.ConvertFormat(lastFmt, lastSiz), off: (int)addr.SegmentOff);
                                        if (lastFmt == RDPEnum.G_IM_FMT.G_IM_FMT_CI)
                                            lastCiTex = tex;
                                    }
                                    catch (Exception ex)
                                    {
                                        errors.Add($"Error in Dlist 0x{new SegmentedAddress(segmentId, dlist).VAddr:X8} : {ex.Message}");
                                    }
                                }

                                lastFmt = (RDPEnum.G_IM_FMT)(-1);
                                lastSiz = (RDPEnum.G_IM_SIZ)(-1);
                                lastTexAddr = 0xFFFFFFFF;

                                break;
                            }
                        case F3DZEX.OpCodeID.G_LOADTLUT:
                            {
                                var loadtlut = F3DZEX.DecodeCommand<F3DZEX.GLoadTlut>(data, i);
                                var addr = new SegmentedAddress(lastTexAddr);

                                if (lastTexAddr == 0xFFFFFFFF)
                                    throw new Z64ObjectAnalyzerException();

                                int w = GetTlutWidth(loadtlut.count + 1);
                                if (addr.Segmented && addr.SegmentId == segmentId)
                                {
                                    try
                                    {
                                        lastTlut = obj.AddTexture(w, (loadtlut.count + 1) / w, N64Texture.ConvertFormat(RDPEnum.G_IM_FMT.G_IM_FMT_RGBA, RDPEnum.G_IM_SIZ.G_IM_SIZ_16b), off: (int)addr.SegmentOff);
                                    }
                                    catch (Exception ex)
                                    {
                                        errors.Add($"Error in Dlist 0x{new SegmentedAddress(segmentId, dlist).VAddr:X8} : {ex.Message}");
                                    }
                                }

                                break;
                            }
                    }
                }
            }

            obj.GroupUnkEntries();
            obj.FixNames();
            obj.SetData(data);

            return errors;
        }
    }
}
