using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public ushort Get(RegisterName reg) {
            return _regs[(int)reg];
        }

        public ushort Get(ushort addr) {
            return _memory[addr];
        }

        public IState Set(RegisterName reg, ushort value) {
            _regs[(int)reg] = value;
            return this;
        }

        public IState Set(ushort addr, ushort value) {
            _memory[addr] = value;
            return this;
        }
    }

}
