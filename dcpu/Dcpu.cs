using System;

namespace Com.MattMcGill.Dcpu {
    public static class Dcpu {

        public static IState ExecuteInstruction(IState state) {
            var addr = state.Get(RegisterName.PC);
            var opcode = state.Get(addr);
            state = state.Set(RegisterName.PC, (ushort)(addr + 1));

            var op = Op.Decode(opcode);

            return op.Apply(state);
        }

    }
}

