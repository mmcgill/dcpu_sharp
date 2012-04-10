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

        public override IState Apply(IState prev) {
            IState s1;
            var jumpTarget = A.Get(prev);
            var s2 = Dcpu.GetOperand(0x1a, prev, out s1).Set(s1, prev.Get(Register.PC)); // SET PUSH, PC
            return s2.Set(Register.PC, jumpTarget);
        }
    }
}

