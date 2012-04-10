using System;
using System.IO;
using System.Diagnostics;

namespace Com.MattMcGill.Dcpu {

    /// <summary>
    /// Mutable DCPU state.
    /// </summary>
    public class MutableState : IState {
        private readonly ushort[] _memory;

        private readonly ushort[] _regs;

        public MutableState() {
            _memory = new ushort[0x10000];
            _regs = new ushort[11];
        }

        public ushort Get(Register reg) {
            return _regs[(int)reg];
        }

        public ushort Get(ushort addr) {
            return _memory[addr];
        }

        public IState Set(Register reg, ushort value) {
            Trace.WriteLine(string.Format("  {0} <- {1}", reg, value));
            _regs[(int)reg] = value;
            return this;
        }

        public IState Set(ushort addr, ushort value) {
            Trace.WriteLine(string.Format("  [0x{0:X}] <- {1}", addr, value));
            _memory[addr] = value;
            return this;
        }

        public static MutableState ReadFromFile(string path) {
            var state = new MutableState();
            using (var objFileReader = new BinaryReader(new FileStream(path, FileMode.Open))) {
                ushort i = 0;
                try {
                    while (true) {
                        var msb = objFileReader.ReadByte();
                        var lsb = objFileReader.ReadByte();
                        state._memory[i++] = (ushort)((msb << 8) ^ lsb);
                    }
                } catch (EndOfStreamException) {}
            }
            return state;
        }
    }

}
