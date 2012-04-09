using System;

namespace Com.MattMcGill.Dcpu {
    /// <summary>
    /// Basic functionality common to DCPU operations.
    /// </summary>
    public abstract class Op {
        /// <summary>
        /// 6-bit A operand (2 high-order bits must be 0).
        /// </summary>
        public byte A { get; private set; }

        /// <summary>
        /// 6-bit B operand (2 high-order bits must be 0).
        /// </summary>
        public byte B { get; private set; }

        private string _name;

        protected Op (string name, byte a, byte b) {
            _name = name;
            A = a;
            B = b;
        }

        protected void LoadOperands(ref IState state, out ushort a, out ushort b) {
            IState s1;
            a = Dcpu.GetOperand(A, state, out s1);
            b = Dcpu.GetOperand(B, s1, out state);
        }

        public abstract IState Apply(IState state);

        public override string ToString () {
            return string.Format ("{0} {1}, {2}", _name, Dcpu.OperandString(A), Dcpu.OperandString(B));
        }

    }

    public class Set : Op {
        public Set(byte a, byte b) : base("SET", a, b) {}

        public override IState Apply (IState state) {
            IState s1;
            var val = Dcpu.GetOperand(B, state, out s1);
            return Dcpu.SetOperand(A, val, s1);
        }
    }

    public class Add : Op {
        public Add(byte a, byte b) : base("ADD", a, b) {}

        public override IState Apply(IState state) {
            ushort a, b; LoadOperands(ref state, out a, out b);
            var sum = (uint)a + (uint)b;
            return Dcpu.SetOperand(A, (ushort)sum, state).Set(Register.O, (ushort)(sum >> 16));
        }
    }

    public class Sub : Op {
        public Sub(byte a, byte b) : base("SUB", a, b) {}

        public override IState Apply(IState state) {
            ushort a, b; LoadOperands(ref state, out a, out b);
            var diff = (uint)a - (uint)b;
            return Dcpu.SetOperand(A, (ushort)diff, state).Set(Register.O, (ushort)(diff >> 16));
        }
    }

    public class Mul : Op {
        public Mul(byte a, byte b) : base("MUL", a, b) {}

        public override IState Apply(IState state) {
            ushort a, b; LoadOperands(ref state, out a, out b);
            var product = (uint)a * (uint)b;
            var overflow = (ushort)((product >> 16) & 0xFFFF);
            return Dcpu.SetOperand(A, (ushort)product, state).Set(Register.O, overflow);
        }
    }

    public class Div : Op {
        public Div(byte a, byte b) : base("DIV", a, b) {}

        public override IState Apply(IState state) {
            ushort a, b; LoadOperands(ref state, out a, out b);
            var product = b == 0 ? (uint)0 : (uint)a / (uint)b;
            var overflow = b == 0 ? (ushort)0 : (ushort)(((a << 16) / (uint)b) & 0xFFFF);
            return Dcpu.SetOperand(A, (ushort)product, state).Set(Register.O, overflow);
        }
    }

    public class Mod : Op {
        public Mod(byte a, byte b) : base("MOD", a, b) {}

        public override IState Apply(IState state) {
            ushort a, b; LoadOperands(ref state, out a, out b);
            return Dcpu.SetOperand(A, b == 0 ? (ushort)0 : (ushort)(a % b), state);
        }
    }

    public class Shl : Op {
        public Shl(byte a, byte b) : base("SHL", a, b) {}

        public override IState Apply(IState state) {
            ushort a, b; LoadOperands(ref state, out a, out b);
            var overflowShift = 16 - (b % 16);
            var result = (ushort)(a << (b % 16));
            return Dcpu.SetOperand(A, result, state).Set(Register.O, (ushort)(a >> overflowShift));
        }
    }

    public class Shr : Op {
        public Shr(byte a, byte b) : base("SHR", a, b) {}

        public override IState Apply(IState state) {
            ushort a, b; LoadOperands(ref state, out a, out b);
            var shift = b % 16;
            var underflow = (ushort)(a & ((ushort)Math.Pow(2, shift) - 1));
            var result = (ushort)(a >> shift);
            return Dcpu.SetOperand(A, result, state).Set(Register.O, underflow);
        }
    }

    public class And : Op {
        public And(byte a, byte b) : base("AND", a, b) {}

        public override IState Apply(IState state) {
            ushort a, b; LoadOperands(ref state, out a, out b);
            return Dcpu.SetOperand(A, (ushort)(a & b), state);
        }
    }

    public class Bor : Op {
        public Bor(byte a, byte b) : base("BOR", a, b) {}

        public override IState Apply(IState state) {
            ushort a, b; LoadOperands(ref state, out a, out b);
            return Dcpu.SetOperand(A, (ushort)(a | b), state);
        }
    }

    public class Xor : Op {
        public Xor(byte a, byte b) : base("XOR", a, b) {}

        public override IState Apply(IState state) {
            ushort a, b; LoadOperands(ref state, out a, out b);
            return Dcpu.SetOperand(A, (ushort)(a ^ b), state);
        }
    }

    public class Ife : Op {
        public Ife(byte a, byte b) : base("IFE", a, b) {}

        public override IState Apply (IState state) {
            ushort a, b; LoadOperands(ref state, out a, out b);
            if (a != b) {
                IState s1;
                var nextOp = Dcpu.Decode(Dcpu.Fetch(state, out s1));
                return Dcpu.SkipOperand(nextOp.B, Dcpu.SkipOperand(nextOp.A, s1));
            }
            return state;
        }
    }
}
