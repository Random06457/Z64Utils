using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace F3DZEX.Render
{
    public class VertexAttribs
    {
        struct AttribEntry
        {
            public enum AttribEntryType
            {
                Float,
                Int,
                Double,
            }

            public int count;
            public AttribEntryType entryType;
            public VertexAttribPointerType ptrType;
            public VertexAttribIntegerType intType;
            public bool normalize;

            public int GetSize()
            {
                switch (entryType)
                {
                    case AttribEntryType.Float: return count * GetSize(ptrType);
                    case AttribEntryType.Int: return count * GetSize(intType);
                    case AttribEntryType.Double: return count * 8;
                    default: throw new Exception();
                }
            }


            private static int GetSize(VertexAttribPointerType type)
            {
                switch (type)
                {
                    case VertexAttribPointerType.Byte:
                    case VertexAttribPointerType.UnsignedByte:
                        return 1;
                    case VertexAttribPointerType.Short:
                    case VertexAttribPointerType.UnsignedShort:
                    case VertexAttribPointerType.HalfFloat:
                        return 2;
                    case VertexAttribPointerType.Int:
                    case VertexAttribPointerType.UnsignedInt:
                    case VertexAttribPointerType.Float:
                    case VertexAttribPointerType.Fixed:
                    case VertexAttribPointerType.UnsignedInt2101010Rev:
                    case VertexAttribPointerType.Int2101010Rev:
                    case VertexAttribPointerType.UnsignedInt10F11F11FRev:
                        return 4;
                    case VertexAttribPointerType.Double:
                        return 8;
                    default:
                        return -1;
                }
            }
            private static int GetSize(VertexAttribIntegerType type)
            {
                switch (type)
                {
                    case VertexAttribIntegerType.Byte:
                    case VertexAttribIntegerType.UnsignedByte:
                        return 1;
                    case VertexAttribIntegerType.Short:
                    case VertexAttribIntegerType.UnsignedShort:
                        return 2;
                    case VertexAttribIntegerType.Int:
                    case VertexAttribIntegerType.UnsignedInt:
                        return 4;
                    default:
                        return -1;
                }
            }

        }

        int _vao;
        int _vbo;
        List<AttribEntry> _attribs;
        bool _built;

        public VertexAttribs()
        {
            _built = false;
            _attribs = new List<AttribEntry>();

            GL.GenVertexArrays(1, out _vao);
            GL.GenBuffers(1, out _vbo);
        }

        public void LayoutAddFloat(int count, VertexAttribPointerType type, bool normalize)
            => _attribs.Add(new AttribEntry() { entryType = AttribEntry.AttribEntryType.Float, count = count, ptrType = type, normalize = normalize });
        public void LayoutAddInt(int count, VertexAttribIntegerType type)
            => _attribs.Add(new AttribEntry() { entryType = AttribEntry.AttribEntryType.Int, count = count, intType = type });

        public void LayoutAddDouble(int count)
            => _attribs.Add(new AttribEntry() { entryType = AttribEntry.AttribEntryType.Double, count = count });

        private int GetStride()
        {
            int stride = 0;
            _attribs.ForEach(e => stride += e.GetSize());
            return stride;
        }

        private void BuldLayout()
        {
            if (_built)
                throw new Exception();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

            int stride = GetStride();

            int idx = 0;
            int off = 0;
            foreach (var entry in _attribs)
            {
                switch (entry.entryType)
                {
                    case AttribEntry.AttribEntryType.Float:
                        GL.VertexAttribPointer(idx, entry.count, entry.ptrType, entry.normalize, stride, off);
                        break;
                    case AttribEntry.AttribEntryType.Int:
                        GL.VertexAttribIPointer(idx, entry.count, entry.intType, stride, new IntPtr(off));
                        break;
                    case AttribEntry.AttribEntryType.Double:
                        GL.VertexAttribLPointer(idx, entry.count, VertexAttribDoubleType.Double, stride, new IntPtr(off));
                        break;
                    default:
                        throw new Exception();
                }
                GL.EnableVertexAttribArray(idx++);
                off += entry.GetSize();
            }

            _built = true;
        }

        // todo : endian swap
        public void SetData(byte[] buffer, bool bigEndian = false, BufferUsageHint hint = BufferUsageHint.StaticDraw)
        {
            if (!_built)
                BuldLayout();

            // endian swap

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, buffer.Length, buffer, hint);
        }

        public void SetData<T>(T[] buffer, int size, BufferUsageHint hint = BufferUsageHint.StaticDraw)
            where T : struct, IComparable
        {
            if (!_built)
                BuldLayout();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, size, buffer, hint);
        }

        public void Draw(PrimitiveType type, uint[] indices)
        {
            if (!_built)
                throw new Exception();

            GL.BindVertexArray(_vao);
            GL.DrawElements(type, indices.Length, DrawElementsType.UnsignedInt, indices);
        }

        public void Draw(PrimitiveType type, byte[] indices)
        {
            if (!_built)
                throw new Exception();

            GL.BindVertexArray(_vao);
            GL.DrawElements(type, indices.Length, DrawElementsType.UnsignedByte, indices);
        }


    }
}
