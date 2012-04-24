using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Com.MattMcGill.Dcpu {

    /// <summary>
    /// Mutable DCPU state.
    /// </summary>
    public class MutableState : AbstractState, IState {
        private readonly ushort[] _memory;

        private readonly ushort[] _regs;

        public MutableState() {
            _memory = new ushort[Dcpu.MAX_ADDRESS + 1];
            _regs = new ushort[11];
        }

        public ushort Get(Register reg) {
            return _regs[(int)reg];
        }

        protected override ushort GetInternal(ushort addr) {
            return _memory[addr];
        }

        public IState Set(Register reg, ushort value) {
            Trace.WriteLine(string.Format("  {0} <- {1}", reg, value));
            _regs[(int)reg] = value;
            return this;
        }

        public IState Set(ushort addr, ushort value) {
            WriteToDevice(addr, value);
            _memory[addr] = value;

            return this;
        }

        public static MutableState ReadFromFile(string path) {
            var state = new MutableState();
            var image = LoadImage(path);
            Array.Copy (image, state._memory, Dcpu.MAX_ADDRESS + 1);
            return state;
        }
    }
}
