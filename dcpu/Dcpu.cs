using System;
using System.Diagnostics;

namespace Com.MattMcGill.Dcpu {
    public static class Dcpu {

        public static Op FetchNextInstruction(IState state, ref ushort pc, ref ushort sp) {
            var word = state.Get(pc++);
            byte opcode = (byte)(word & 0xF);
            if (opcode == 0) {
                return DecodeNonBasic(state, word, ref pc, ref sp);
            }
            return DecodeBasic(state, word, ref pc, ref sp);
        }

        public static BasicOp DecodeBasic(IState state, ushort word, ref ushort pc, ref ushort sp) {
            IState s1;
            byte opcode = (byte)(word & 0xF);
            byte a = (byte)((word >> 4) & 0x3F);
            var operandA = GetOperand(state, a, ref pc, ref sp);
            byte b = (byte)((word >> 10) & 0x3F);
            var operandB = GetOperand(state, b, ref pc, ref sp);
            switch (opcode) {
                case 0x01: return new Set(operandA, operandB);
                case 0x02: return new Add(operandA, operandB);
                case 0x03: return new Sub(operandA, operandB);
                case 0x04: return new Mul(operandA, operandB);
                case 0x05: return new Div(operandA, operandB);
                case 0x06: return new Mod(operandA, operandB);
                case 0x07: return new Shl(operandA, operandB);
                case 0x08: return new Shr(operandA, operandB);
                case 0x09: return new And(operandA, operandB);
                case 0x0a: return new Bor(operandA, operandB);
                case 0x0b: return new Xor(operandA, operandB);
                case 0x0c: return new Ife(operandA, operandB);
                case 0x0d: return new Ifn(operandA, operandB);
                case 0x0e: return new Ifg(operandA, operandB);
                case 0x0f: return new Ifb(operandA, operandB);
                default: throw new NotImplementedException();
            }
        }

        private static NonBasicOp DecodeNonBasic(IState state, ushort word, ref ushort pc, ref ushort sp) {
            var opcode = (byte)((word >> 4) & 0x3F);
            var operandCode = (byte)((word >> 10) & 0x3F);
            var operand = GetOperand(state, operandCode, ref pc, ref sp);
            switch (opcode) {
                case 0x01: return new Jsr(operand);
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
        public static Operand GetOperand(IState state, byte operand, ref ushort pc, ref ushort sp) {
            if (0 <= operand && operand < 8) { // register
                return new Reg((Register)operand);
            }
            if (8 <= operand && operand < 16) { // [register]
                return new RegIndirect((Register)(operand & 0x07));
            }
            if (16 <= operand && operand < 24) { // [next word + register]
                return new RegIndirectOffset((Register)(operand & 0x07), state.Get(pc++));
            }
            if (operand == 0x18) { // [SP++]
                return new Pop(sp++);
            }
            if (operand == 0x19) { // [SP]
                return new Peek(sp);
            }
            if (operand == 0x1a) { // [--SP]
                return new Push(--sp);
            }
            if (operand == 0x1b) { // SP
                return new Reg(Register.SP);
            }
            if (operand == 0x1c) { // PC
                return new Reg(Register.PC);
            }
            if (operand == 0x1d) { // O
                return new Reg(Register.O);
            }
            if (operand == 0x1e) { // [next word]
                return new Address(state.Get(pc++));
            }
            if (operand == 0x1f) { // next literal
                return new Literal(state.Get(pc++));
            }
            if (0x20 <= operand && operand < 0x40) { // literal
                return new Literal((ushort)(operand - 0x20));
            }
            throw new ArgumentException("Invalid operand " + operand);
        }
     }
}

