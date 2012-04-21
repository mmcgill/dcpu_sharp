using System;

namespace Com.MattMcGill.Dcpu {
    public abstract class NonBasicOp : Op {
        private string _name;

        public Operand A { get; private set; }

        protected NonBasicOp(string name, Operand a) {
            _name = name;
            A = a;
        }

        public override string ToString() {
            return string.Format("{0} {1}", _name, A);
        }
    }

    public class Jsr : NonBasicOp {
        public Jsr(Operand a) : base("JSR", a) {}

        public override IState Apply(IState state) {
            var jumpTarget = A.Get(state);
            var sp = (ushort)(state.Get(Register.SP) - 1);
            var op = new Set(new Push(sp), new Reg(Register.PC));
            return op.Apply(state.Set(Register.SP, sp)).Set(Register.PC, jumpTarget);
        }

        public override int Cycles(IState state) { return 2; }
    }
}

