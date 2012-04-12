using System;
using System.Diagnostics;

namespace Com.MattMcGill.Dcpu {
    public static class Dcpu {

        public static Op FetchNextInstruction(IState before, out IState after) {
            IState s1, s2;
            var word = FetchNextWord(before, out s1);
            byte opcode = (byte)(word & 0xF);
            if (opcode == 0) {
                return DecodeNonBasic(word, s1, out after);
            }
            return DecodeBasic(word, s1, out after);
        }

        public static ushort FetchNextWord(IState before, out IState after) {
            var addr = before.Get(Register.PC);
            var word = before.Get(addr);
            after = before.Set(Register.PC, (ushort)(addr + 1));
            return word;
        }

        public static BasicOp DecodeBasic(ushort word, IState before, out IState after) {
            IState s1;
            byte opcode = (byte)(word & 0xF);
            byte a = (byte)((word >> 4) & 0x3F);
            var operandA = GetOperand(a, before, out s1);
            byte b = (byte)((word >> 10) & 0x3F);
            var operandB = GetOperand(b, s1, out after);
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

        private static NonBasicOp DecodeNonBasic(ushort word, IState before, out IState after) {
            var opcode = (byte)((word >> 4) & 0x3F);
            var operandCode = (byte)((word >> 10) & 0x3F);
            var operand = GetOperand(operandCode, before, out after);
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
        public static Operand GetOperand(byte operand, IState prev, out IState next) {
            next = prev;
            if (0 <= operand && operand < 8) { // register
                return new Reg((Register)operand);
            }
            if (8 <= operand && operand < 16) { // [register]
                return new RegIndirect((Register)(operand & 0x07));
            }
            if (16 <= operand && operand < 24) { // [next word + register]
                return new RegIndirectOffset((Register)(operand & 0x07), FetchNextWord(prev, out next));
            }
            if (operand == 0x18) { // [SP++]
                var addr = prev.Get(Register.SP);
                next = prev.Set(Register.SP, (ushort)(addr + 1));
                return new Pop(addr);
            }
            if (operand == 0x19) { // [SP]
                return new Peek(prev.Get(Register.SP));
            }
            if (operand == 0x1a) { // [--SP]
                var addr = (ushort)(prev.Get(Register.SP) - 1);
                next = prev.Set(Register.SP, addr);
                return new Push(addr);
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
                var addr = FetchNextWord(prev, out next);
                return new Address(addr);
            }
            if (operand == 0x1f) { // next literal
                return new Literal(FetchNextWord(prev, out next));
            }
            if (0x20 <= operand && operand < 0x40) { // literal
                return new Literal((ushort)(operand - 0x20));
            }
            throw new ArgumentException("Invalid operand " + operand);
        }
     }
}

