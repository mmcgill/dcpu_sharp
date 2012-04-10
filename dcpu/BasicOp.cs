using System;

namespace Com.MattMcGill.Dcpu {
    /// <summary>
    /// Basic functionality common to DCPU operations.
    /// </summary>
    public abstract class BasicOp : Op {
        /// <summary>
        /// 6-bit A operand (2 high-order bits must be 0).
        /// </summary>
        public Operand A { get; private set; }

        /// <summary>
        /// 6-bit B operand (2 high-order bits must be 0).
        /// </summary>
        public Operand B { get; private set; }

        private string _name;

        protected BasicOp(string name, Operand a, Operand b) {
            _name = name;
            A = a;
            B = b;
        }

        public override string ToString () {
            return string.Format ("{0} {1}, {2}", _name, A, B);
        }

    }

    public class Set : BasicOp {
        public Set(Operand a, Operand b) : base("SET", a, b) {}

        public override IState Apply (IState state) {
            return A.Set(state, B.Get(state));
        }
    }

    public class Add : BasicOp {
        public Add(Operand a, Operand b) : base("ADD", a, b) {}

        public override IState Apply(IState state) {
            var sum = A.Get(state) + B.Get(state);
            return A.Set(state, (ushort)sum).Set(Register.O, (ushort)(sum >> 16));
        }
    }

    public class Sub : BasicOp {
        public Sub(Operand a, Operand b) : base("SUB", a, b) {}

        public override IState Apply(IState state) {
            var diff = A.Get(state) - B.Get(state);
            return A.Set(state, (ushort)diff).Set(Register.O, (ushort)(diff >> 16));
        }
    }

    public class Mul : BasicOp {
        public Mul(Operand a, Operand b) : base("MUL", a, b) {}

        public override IState Apply(IState state) {
            var a = A.Get(state); var b = B.Get(state);
            var product = a * b;
            var overflow = (ushort)((product >> 16) & 0xFFFF);
            return A.Set(state, (ushort)product).Set(Register.O, overflow);
        }
    }

    public class Div : BasicOp {
        public Div(Operand a, Operand b) : base("DIV", a, b) {}

        public override IState Apply(IState state) {
            var a = A.Get(state); var b = B.Get(state);
            var product = b == 0 ? (uint)0 : (uint)a / (uint)b;
            var overflow = b == 0 ? (ushort)0 : (ushort)(((a << 16) / (uint)b) & 0xFFFF);
            return A.Set(state, (ushort)product).Set(Register.O, overflow);
        }
    }

    public class Mod : BasicOp {
        public Mod(Operand a, Operand b) : base("MOD", a, b) {}

        public override IState Apply(IState state) {
            var a = A.Get(state); var b = B.Get(state);
            return A.Set(state, b == 0 ? (ushort)0 : (ushort)(a % b));
        }
    }

    public class Shl : BasicOp {
        public Shl(Operand a, Operand b) : base("SHL", a, b) {}

        public override IState Apply(IState state) {
            var a = A.Get(state); var b = B.Get(state);
            var overflowShift = 16 - (b % 16);
            var result = (ushort)(a << (b % 16));
            return A.Set(state, result).Set(Register.O, (ushort)(a >> overflowShift));
        }
    }

    public class Shr : BasicOp {
        public Shr(Operand a, Operand b) : base("SHR", a, b) {}

        public override IState Apply(IState state) {
            var a = A.Get(state); var b = B.Get(state);
            var shift = b % 16;
            var underflow = (ushort)(a & ((ushort)Math.Pow(2, shift) - 1));
            var result = (ushort)(a >> shift);
            return A.Set(state, result).Set(Register.O, underflow);
        }
    }

    public class And : BasicOp {
        public And(Operand a, Operand b) : base("AND", a, b) {}

        public override IState Apply(IState state) {
            return A.Set(state, (ushort)(A.Get(state) & B.Get(state)));
        }
    }

    public class Bor : BasicOp {
        public Bor(Operand a, Operand b) : base("BOR", a, b) {}

        public override IState Apply(IState state) {
            return A.Set(state, (ushort)(A.Get(state) | B.Get(state)));
        }
    }

    public class Xor : BasicOp {
        public Xor(Operand a, Operand b) : base("XOR", a, b) {}

        public override IState Apply(IState state) {
            return A.Set(state, (ushort)(A.Get(state) ^ B.Get(state)));
        }
    }

    public abstract class If : BasicOp {
        public If(string name, Operand a, Operand b) : base(name, a, b) {}
        public override IState Apply(IState state) {
            var a = A.Get(state); var b = B.Get(state);
            if (IsNextSkipped(a, b)) {
                IState next;
                Dcpu.FetchNextInstruction(state, out next);
                return next;
            }
            return state;
        }
        protected abstract bool IsNextSkipped(ushort a, ushort b);
    }

    public class Ife : If {
        public Ife(Operand a, Operand b) : base("IFE", a, b) {}
        protected override bool IsNextSkipped (ushort a, ushort b) { return a != b; }
    }

    public class Ifn : If {
        public Ifn(Operand a, Operand b) : base("IFN", a, b) {}
        protected override bool IsNextSkipped (ushort a, ushort b) { return a == b; }
    }

    public class Ifg : If {
        public Ifg(Operand a, Operand b) : base("IFG", a, b) {}
        protected override bool IsNextSkipped (ushort a, ushort b) { return a <= b; }
    }

    public class Ifb : If {
        public Ifb(Operand a, Operand b) : base("IFB", a, b) {}
        protected override bool IsNextSkipped (ushort a, ushort b) { return (a & b) == 0; }
    }
}
