using System;

namespace Com.MattMcGill.Dcpu {

    public class ImmutableState : AbstractState, IState {
        private PersistentArray<ushort> _memory;
        private PersistentArray<ushort> _regs;

        public ImmutableState() {
            _memory = new PersistentArray<ushort>(Dcpu.MAX_ADDRESS + 1);
            _regs = new PersistentArray<ushort>(11);
        }

        protected ImmutableState(ImmutableState prev, Register register, ushort value) : base(prev) {
            _memory = prev._memory;
            _regs = prev._regs.Set((int)register, value);
        }

        protected ImmutableState(ImmutableState prev, ushort addr, ushort value) : base(prev) {
            _memory = prev._memory.Set(addr, value);
            _regs = prev._regs;
        }

        protected ImmutableState(ushort[] memory) {
            _memory = new PersistentArray<ushort>(memory);
            _regs = new PersistentArray<ushort>(11);
        }

        public ushort Get(Register reg) {
            return _regs[(int)reg];
        }

        protected override ushort GetInternal(ushort addr) {
            return _memory[addr];
        }

        public IState Set(Register reg, ushort value) {
            return new ImmutableState(this, reg, value);
        }

        public IState Set(ushort addr, ushort value) {
            WriteToDevice(addr, value);
            return new ImmutableState(this, addr, value);
        }

        public static ImmutableState ReadFromFile(string path) {
            return new ImmutableState(LoadImage(path));
        }
    }
}

