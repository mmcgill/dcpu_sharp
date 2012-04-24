using System;

namespace Com.MattMcGill.Dcpu {

    public class ImmutableState : AbstractState, IState {
        private ushort[] _memory;
        private ushort[] _regs;

        public ImmutableState() {
            _memory = new ushort[Dcpu.MAX_ADDRESS + 1];
            _regs = new ushort[11];
        }

        protected ImmutableState(ImmutableState prev) : base(prev) {
            // Temporarily use a complete array copy until we implement
            // a more efficient persistent data structure.
            _memory = new ushort[Dcpu.MAX_ADDRESS + 1];
            Array.Copy(prev._memory, _memory, Dcpu.MAX_ADDRESS + 1);
            _regs = new ushort[11];
            Array.Copy(prev._regs, _regs, 11);
        }

        public ushort Get(Register reg) {
            return _regs[(int)reg];
        }

        protected override ushort GetInternal(ushort addr) {
            return _memory[addr];
        }

        public IState Set(Register reg, ushort value) {
            var next = new ImmutableState(this);
            next._regs[(int)reg] = value;
            return next;
        }

        public IState Set(ushort addr, ushort value) {
            WriteToDevice(addr, value);

            var next = new ImmutableState(this);
            next._memory[addr] = value;
            return next;
        }

        public static ImmutableState ReadFromFile(string path) {
            var state = new ImmutableState();
            var image = LoadImage(path);
            Array.Copy(image, state._memory, Dcpu.MAX_ADDRESS + 1);
            return state;
        }
    }
}

