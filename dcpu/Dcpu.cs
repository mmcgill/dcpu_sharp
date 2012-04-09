using System;

namespace Com.MattMcGill.Dcpu {
    public static class Dcpu {

        public static IState ExecuteInstruction(IState state) {
            var addr = state.Get(Register.PC);
            var opcode = state.Get(addr);
            state = state.Set(Register.PC, (ushort)(addr + 1));

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
                case 0x06: return new Mod(a, b);
                case 0x07: return new Shl(a, b);
                case 0x08: return new Shr(a, b);
                case 0x09: return new And(a, b);
                case 0x0a: return new Bor(a, b);
                case 0x0b: return new Xor(a, b);
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
                return prev.Get((Register) operand);
            }
            if (8 <= operand && operand < 16) { // [register]
                return prev.Get(prev.Get((Register) (operand & 0x07)));
            }
            if (16 <= operand && operand < 24) { // [next word + register]
                var pc = (ushort) (prev.Get(Register.PC) + 1);
                var addr = (ushort) (prev.Get(pc) + prev.Get((Register) (operand & 0x07)));
                var result = prev.Get(addr);
                next = prev.Set(Register.PC, pc);
                return result;
            }
            if (operand == 0x18) { // [SP++]
                var addr = prev.Get(Register.SP);
                var result = prev.Get(addr);
                next = prev.Set(Register.SP, (ushort)(addr + 1));
                return result;
            }
            if (operand == 0x19) { // [SP]
                return prev.Get(prev.Get(Register.SP));
            }
            if (operand == 0x1a) { // [--SP]
                var addr = (ushort)(prev.Get(Register.SP) - 1);
                var result = prev.Get(addr);
                next = prev.Set(Register.SP, addr);
                return result;
            }
            if (operand == 0x1b) { // SP
                return prev.Get(Register.SP);
            }
            if (operand == 0x1c) { // PC
                return prev.Get(Register.PC);
            }
            if (operand == 0x1d) { // O
                return prev.Get(Register.O);
            }
            if (operand == 0x1e) { // [next word]
                var addr = prev.Get(Register.PC);
                var result = prev.Get(prev.Get(addr));
                next = prev.Set(Register.PC, (ushort)(addr + 1));
                return result;
            }
            if (operand == 0x1f) { // next literal
                var addr = prev.Get(Register.PC);
                var result = prev.Get(addr);
                next = prev.Set(Register.PC, (ushort)(addr + 1));
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
                return state.Set((Register)operand, value);
            }
            if (0x8 <= operand && operand < 0x10) { // [register]
                var addr = state.Get((Register)(operand - 0x8));
                return state.Set(addr, value);
            }
            if (0x10 <= operand && operand < 0x18) { // [next word + register]
                var pc = state.Get(Register.PC);
                var addr = state.Get(pc);
                var offset = state.Get((Register)(operand - 0x10));
                return state.Set(Register.PC, (ushort)(pc + 1))
                            .Set((ushort)(addr + offset), value);
            }
            if (operand == 0x18) { // [SP++]
                var addr = state.Get(Register.SP);
                return state.Set(addr, value).Set(Register.SP, (ushort)(addr + 1));
            }
            if (operand == 0x19) { // [SP]
                return state.Set(state.Get(Register.SP), value);
            }
            if (operand == 0x1a) { // [--SP]
                var addr = (ushort) (state.Get(Register.SP) - 1);
                return state.Set(addr, value).Set(Register.SP, addr);
            }
            if (operand == 0x1b) { // SP
                return state.Set(Register.SP, value);
            }
            if (operand == 0x1c) { // PC
                return state.Set(Register.PC, value);
            }
            if (operand == 0x1d) { // O
                return state.Set(Register.O, value);
            }
            if (operand == 0x1e) { // [next word]
                var addr = state.Get(Register.PC);
                return state.Set(addr, value).Set(Register.PC, (ushort)(addr + 1));
            }
            if (operand == 0x1f) { // next word
                var addr = state.Get(Register.PC);
                return state.Set(Register.PC, (ushort)(addr + 1));
            }
            if (0x20 <= operand && operand < 0x40) { // literal
                var addr = state.Get (Register.PC);
                return state.Set (Register.PC, (ushort)(addr + 1));
            }
            throw new ArgumentException("Invalid operand " + operand);
        }

        /// <summary>
        /// Skip the operand, incrementing the PC if the operand involves the next word.
        /// </summary>
        /// <param name='operand'>
        /// the operand code
        /// </param>
        /// <param name='state'>
        /// the current state
        /// </param>
        /// <returns>
        /// the state after skipping the operand
        /// </returns>
        public static IState SkipOperand( byte operand, IState state) {
            if ((0x10 <= operand && operand < 0x18) || operand == 0x1e || operand == 0x1f) {
                var pc = state.Get(Register.PC);
                return state.Set(Register.PC, (ushort)(pc + 1));
            } else {
                return state;
            }
        }

        public static string OperandString( byte operand ) {
            if (0x0 <= operand && operand < 0x8) { // register
                return ((Register)operand).ToString();
            }
            if (0x8 <= operand && operand < 0x10) { // [register]
                return "[" + ((Register)(operand - 0x8)).ToString() + "]";
            }
            if (0x10 <= operand && operand < 0x18) { // [next word + register]
                var reg = (Register)(operand - 0x10);
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

