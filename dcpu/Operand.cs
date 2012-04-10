using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.MattMcGill.Dcpu {
    public abstract class Operand {
        public abstract ushort Get(IState state);
        public abstract IState Set(IState prev, ushort value);
    }

    public class Literal : Operand {
        private ushort _value;

        public Literal(ushort value) { _value = value; }

        public override ushort Get(IState state) {
            return _value;
        }

        public override IState Set(IState prev, ushort value) {
            return prev;
        }

        public override string ToString () {
            return string.Format("0x{0:X}", _value);
        }
    }

    public class Address : Operand {
        private ushort _addr;

        public Address(ushort addr) { _addr = addr; }

        public override ushort Get(IState state) {
            return state.Get(_addr);
        }

        public override IState Set(IState prev, ushort value) {
            return prev.Set(_addr, value);
        }

        public override string ToString () {
            return string.Format("[0x{0:X}]", _addr);
        }
    }

    public class Reg : Operand {
        private Register _reg;

        public Reg(Register register) { _reg = register; }

        public override ushort Get(IState state) {
            return state.Get(_reg);
        }

        public override IState Set(IState prev, ushort value) {
            return prev.Set(_reg, value);
        }

        public override string ToString() {
            return _reg.ToString();
        }
    }
}
