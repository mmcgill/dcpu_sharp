using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Com.MattMcGill.Dcpu {
    public class Dcpu {
        public static readonly int MAX_ADDRESS = 0x10000;
        private bool _running;
        private IState _state;
        private Task _cpuTask;

        public Dcpu(IState initial) {
            _running = false;
            _state = initial;
        }

        public void Start() {
            if (!_running) {
                _running = true;
                _cpuTask = Task.Factory.StartNew(() => CpuLoop(_state));
            }
        }

        public void Stop() {
            if (_running) {
                _running = false;
                _cpuTask.Wait();
            }
        }

        public void CpuLoop(IState initial) {
            var state = initial;
            while (_running) {
                var pc = state.Get(Register.PC);
                var sp = state.Get(Register.SP);
                var origSp = sp;

                var op = Dcpu.FetchNextInstruction(state, ref pc, ref sp);

                state = state.Set(Register.PC, pc);
                if (origSp != sp)
                    state = state.Set(Register.SP, sp);
                state = op.Apply(state);
            }
        }

        public static Op FetchNextInstruction(IState state, ref ushort pc, ref ushort sp) {
            var firstWord = state.Get(pc++);
            return 0 == (firstWord & 0xF) ?
                (Op)DecodeNonBasic(state, firstWord, ref pc, ref sp) :
                (Op)DecodeBasic(state, firstWord, ref pc, ref sp);
        }

        public static BasicOp DecodeBasic(IState state, ushort word, ref ushort pc, ref ushort sp) {
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
                default: throw new ArgumentException(string.Format("{0:X} is not a valid basic opcode", opcode));
            }
        }

        private static NonBasicOp DecodeNonBasic(IState state, ushort word, ref ushort pc, ref ushort sp) {
            var opcode = (byte)((word >> 4) & 0x3F);
            var operandCode = (byte)((word >> 10) & 0x3F);
            var operand = GetOperand(state, operandCode, ref pc, ref sp);
            switch (opcode) {
                case 0x01: return new Jsr(operand);
                default: throw new ArgumentException(string.Format("{0:X} is not a valid non-basic opcode", opcode));
            }
        }

        public static Operand GetOperand(IState state, byte a, ref ushort pc, ref ushort sp) {
            if (0x00 <= a && a < 0x08) return new Reg((Register)a);
            if (0x08 <= a && a < 0x10) return new RegIndirect((Register)(a & 0x07));
            if (0x10 <= a && a < 0x18) return new RegIndirectOffset((Register)(a & 0x07), state.Get(pc++));
            if (0x20 <= a && a < 0x40) return new Literal((ushort)(a - 0x20));
            switch (a) {
                case 0x18: return new Pop(sp++);
                case 0x19: return new Peek(sp);
                case 0x1a: return new Push(--sp);
                case 0x1b: return new Reg(Register.SP);
                case 0x1c: return new Reg(Register.PC);
                case 0x1d: return new Reg(Register.O);
                case 0x1e: return new Address(state.Get(pc++));
                case 0x1f: return new Literal(state.Get(pc++));
            }
            throw new ArgumentException("Invalid operand " + a);
        }
     }
}

