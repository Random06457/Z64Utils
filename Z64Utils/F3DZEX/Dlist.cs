using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace F3DZEX
{
    public class Dlist : IEnumerable<Dlist.CommandHolder>
    {
        public struct CommandHolder
        {
            public uint addr;
            public int depth;
            public Command.CommandInfo cmd;

            public CommandHolder(uint addr, int depth, Command.CommandInfo cmd)
                => (this.addr, this.depth, this.cmd) = (addr, depth, cmd);
        }

        List<CommandHolder> _cmds;
        int _maxDepth;

        public Dlist(int maxDepth = 16)
        {
            _cmds = new List<CommandHolder>();
            _maxDepth = maxDepth;
        }
        public Dlist(Memory mem, uint addr, int maxDepth = 16) : this(maxDepth)
        {
            DecodeDlist(mem, addr, 0);
        }

        public Dlist(byte[] data, uint addr = 0) : this(1)
        {
            var cmds = Command.DecodeDList(data, 0);

            cmds.ForEach(cmd => { _cmds.Add(new CommandHolder(addr, 0, cmd)); addr += (uint)cmd.GetSize(); });
        }

        private void DecodeDlist(Memory mem, uint addr, int depth)
        {
            if (depth >= _maxDepth)
                return;

            for (int size = 0; ; size += 8)
            {
                Command.OpCodeID id = (Command.OpCodeID)mem.ReadBytes(addr + (uint)size, 1)[0];

                if (id == Command.OpCodeID.G_DL || id == Command.OpCodeID.G_ENDDL)
                {
                    var cmds = Command.DecodeDList(mem.ReadBytes(addr, size + 8), 0);

                    // append previous command to the list
                    cmds.ForEach(cmd => { _cmds.Add(new CommandHolder(addr, depth, cmd)); addr += (uint)cmd.GetSize(); });

                    if (id == Command.OpCodeID.G_DL)
                    {
                        var gdl = cmds[^1].Convert<Command.GDl>();

                        // decode dlist recursively
                        DecodeDlist(mem, gdl.dl, depth + 1);

                        size = -8;
                    }
                    else
                    {
                        // end of dlist
                        return;
                    }
                }
            }
        }

        public IEnumerator<CommandHolder> GetEnumerator()
        {
            foreach (var cmd in _cmds)
                yield return cmd;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int CommandCount() => _cmds.Count;
        public CommandHolder AtIndex(int i) => _cmds[i];
        public CommandHolder? AtAddress(uint addr)
        {
            foreach (var cmd in _cmds)
            {
                if (cmd.addr == addr)
                    return cmd;
            }
            return null;
        }

        public bool CheckInfiniteLoop()
        {
            for (int i = 0; i < _cmds.Count; i++)
            {
                for (int j = i+1; j < _cmds.Count; j++)
                {
                    if (_cmds[j].addr == _cmds[i].addr)
                        return true;
                }
            }

            return false;
        }

    }
}
