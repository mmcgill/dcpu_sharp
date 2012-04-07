using System;

namespace Com.MattMcGill.Dcpu {
    public static class Dcpu {

        public static IState ExecuteInstruction(IState state) {
            var addr = state.Get(RegisterName.PC);
            var opcode = state.Get(addr);
            state = state.Set(RegisterName.PC, (ushort)(addr + 1));

            var op = Decode(opcode);

            return op.Apply(state);
        }

        public static Op Decode(ushort word) {
            byte opcode = (byte)(word & 0xF);
            if (opcode == 0) {
                throw new NotImplementedException();
            }
            byte a = (byte)((word >> 4) & 0x3F);
            byte b = (byte)((word >> 10) & 0x3F);
            switch (opcode) {
                case 0x01: return new Set(a, b);
                case 0x02: return new Add(a, b);
                case 0x03: return new Sub(a, b);
                case 0x04: return new Mul(a, b);
                case 0x05: return new Div(a, b);
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Read the operand.
        /// </summary>
        /// <param name="operand">operand to read</param>
        /// <param name="prev">current DCPU state</param>
        /// <param name="next">DCPU state after reading</param>
        /// <returns>a value</returns>
        /// <remarks>
        /// GetOperand modifies SP for PUSH and POP operands,
        /// and the PC for addressing modes that read the next
        /// word of RAM.
        ///
        /// We're presently assuming that getting an operand never
        /// affects the Overflow register.
        /// </remarks>
        public static ushort GetOperand(byte operand, IState prev, out IState next) {
            next = prev;
            if (0 <= operand && operand < 8) { // register
                return prev.Get((RegisterName) operand);
            }
            if (8 <= operand && operand < 16) { // [register]
                return prev.Get(prev.Get((RegisterName) (operand & 0x07)));
            }
            if (16 <= operand && operand < 24) { // [next word + register]
                var pc = (ushort) (prev.Get(RegisterName.PC) + 1);
                var addr = (ushort) (prev.Get(pc) + prev.Get((RegisterName) (operand & 0x07)));
                var result = prev.Get(addr);
                next = prev.Set(RegisterName.PC, pc);
                return result;
            }
            if (operand == 0x18) { // [SP++]
                var addr = prev.Get(RegisterName.SP);
                var result = prev.Get(addr);
                next = prev.Set(RegisterName.SP, (ushort)(addr + 1));
                return result;
            }
            if (operand == 0x19) { // [SP]
                return prev.Get(prev.Get(RegisterName.SP));
            }
            if (operand == 0x1a) { // [--SP]
                var addr = (ushort)(prev.Get(RegisterName.SP) - 1);
                var result = prev.Get(addr);
                next = prev.Set(RegisterName.SP, addr);
                return result;
            }
            if (operand == 0x1b) { // SP
                return prev.Get(RegisterName.SP);
            }
            if (operand == 0x1c) { // PC
                return prev.Get(RegisterName.PC);
            }
            if (operand == 0x1d) { // O
                return prev.Get(RegisterName.O);
            }
            if (operand == 0x1e) { // [next word]
                var addr = prev.Get(RegisterName.PC);
                var result = prev.Get(prev.Get(addr));
                next = prev.Set(RegisterName.PC, (ushort)(addr + 1));
                return result;
            }
            if (operand == 0x1f) { // next literal
                var addr = prev.Get(RegisterName.PC);
                var result = prev.Get(addr);
                next = prev.Set(RegisterName.PC, (ushort)(addr + 1));
                return result;
            }
            if (0x20 <= operand && operand < 0x40) { // literal
                return (ushort)(operand - 0x20);
            }
            throw new ArgumentException("Invalid operand " + operand);
        }

        /// <summary>
        /// Write the operand.
        /// </summary>
        /// <param name="operand">operand to write</param>
        /// <param name="state">current DCPU state</param>
        /// <param name="value">value to write</param>
        /// <returns>DCPU state after writing</returns>
        /// <remarks>
        /// We're presently assuming that getting an operand write (other than 0x1d) never
        /// affects the Overflow register.
        /// </remarks>
        public static IState SetOperand(ushort operand, ushort value, IState state) {
            if (0x0 <= operand && operand < 0x8) { // register
                return state.Set((RegisterName)operand, value);
            }
            if (0x8 <= operand && operand < 0x10) { // [register]
                var addr = state.Get((RegisterName)(operand - 0x8));
                return state.Set(addr, value);
            }
            if (0x10 <= operand && operand < 0x18) { // [next word + register]
                var pc = state.Get(RegisterName.PC);
                var addr = state.Get(pc);
                var offset = state.Get((RegisterName)(operand - 0x10));
                return state.Set(RegisterName.PC, (ushort)(pc + 1))
                            .Set((ushort)(addr + offset), value);
            }
            if (operand == 0x18) { // [SP++]
                var addr = state.Get(RegisterName.SP);
                return state.Set(addr, value).Set(RegisterName.SP, (ushort)(addr + 1));
            }
            if (operand == 0x19) { // [SP]
                return state.Set(state.Get(RegisterName.SP), value);
            }
            if (operand == 0x1a) { // [--SP]
                var addr = (ushort) (state.Get(RegisterName.SP) - 1);
                return state.Set(addr, value).Set(RegisterName.SP, addr);
            }
            if (operand == 0x1b) { // SP
                return state.Set(RegisterName.SP, value);
            }
            if (operand == 0x1c) { // PC
                return state.Set(RegisterName.PC, value);
            }
            if (operand == 0x1d) { // O
                return state.Set(RegisterName.O, value);
            }
            if (operand == 0x1e) { // [next word]
                var addr = state.Get(RegisterName.PC);
                return state.Set(addr, value).Set(RegisterName.PC, (ushort)(addr + 1));
            }
            if (operand == 0x1f) { // next word
                var addr = state.Get(RegisterName.PC);
                return state.Set(RegisterName.PC, (ushort)(addr + 1));
            }
            if (0x20 <= operand && operand < 0x40) { // literal
                var addr = state.Get (RegisterName.PC);
                return state.Set (RegisterName.PC, (ushort)(addr + 1));
            }
            throw new ArgumentException("Invalid operand " + operand);
        }

        public static string OperandString( byte operand ) {
            if (0x0 <= operand && operand < 0x8) { // register
                return ((RegisterName)operand).ToString();
            }
            if (0x8 <= operand && operand < 0x10) { // [register]
                return "[" + ((RegisterName)(operand - 0x8)).ToString() + "]";
            }
            if (0x10 <= operand && operand < 0x18) { // [next word + register]
                var reg = (RegisterName)(operand - 0x10);
                return "[" + "PC++ + " + reg + "]";
            }
            if (operand == 0x18) { // [SP++]
                return "[SP++]";
            }
            if (operand == 0x19) { // [SP]
                return "[SP]";
            }
            if (operand == 0x1a) { // [--SP]
                return "[--SP]";
            }
            if (operand == 0x1b) { // SP
                return "SP";
            }
            if (operand == 0x1c) { // PC
                return "PC";
            }
            if (operand == 0x1d) { // O
                return "O";
            }
            if (operand == 0x1e) { // [next word]
                return "[PC++]";
            }
            if (operand == 0x1f) { // next word
                return "PC++";
            }
            if (0x20 <= operand && operand < 0x40) { // literal
                return ((ushort)(operand - 0x20)).ToString();
            }
            throw new ArgumentException("Invalid operand " + operand);
        }

     }
}
