using System;

namespace Com.MattMcGill.Dcpu {
    public abstract class NonBasicOp : Op {
        private string _name;

        public byte A { get; private set; }

        protected NonBasicOp(string name, byte a) {
            _name = name;
            A = a;
        }

        public override string ToString() {
            return string.Format("{0} {1}]", _name, A);
        }
    }

    public class Jsr : NonBasicOp {
        public Jsr(byte a) : base("JSR", a) {}

        public override IState Apply(IState prev) {
            IState s1, s2;
            var jumpTarget = Dcpu.GetOperand(A, prev, out s1);
            s2 = Dcpu.SetOperand(0x1a, s1.Get(Register.PC), s1);
            return s2.Set(Register.PC, jumpTarget);
        }
    }
}

