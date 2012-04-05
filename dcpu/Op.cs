using System;

namespace Com.MattMcGill.Dcpu {
    /// <summary>
    /// Basic functionality common to DCPU operations.
    /// </summary>
    public abstract class Op : IOp {
        /// <summary>
        /// 6-bit A operand (2 high-order bits must be 0).
        /// </summary>
        public byte A { get; private set; }

        /// <summary>
        /// 6-bit B operand (2 high-order bits must be 0).
        /// </summary>
        public byte B { get; private set; }

        public abstract IState Apply(IState state);

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
            throw new NotImplementedException();
        }
    }
}
