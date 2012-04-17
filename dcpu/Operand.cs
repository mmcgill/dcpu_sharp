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
        public ushort Value { get; private set; }

        public Literal(ushort value) { Value = value; }

        public override ushort Get(IState state) {
            return Value;
        }

        public override IState Set(IState prev, ushort value) {
            return prev;
        }

        public override string ToString () {
            return string.Format("0x{0:X}", Value);
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
        public Register Register { get; private set; }

        public Reg(Register register) { Register = register; }

        public override ushort Get(IState state) {
            return state.Get(Register);
        }

        public override IState Set(IState prev, ushort value) {
            return prev.Set(Register, value);
        }

        public override string ToString() {
            return Register.ToString();
        }
    }

	public class RegIndirect : Operand {
		private Register _reg;

		public RegIndirect(Register register) { _reg = register; }

		public override ushort Get(IState state) {
			return state.Get(state.Get(_reg));
		}

		public override IState Set(IState prev, ushort value) {
			return prev.Set(prev.Get(_reg), value);
		}

        public override string ToString() {
			return string.Format("[{0}]", _reg);
		}
	}

    public class RegIndirectOffset : Operand {
        private Register _reg;
        private ushort _offset;

        public RegIndirectOffset(Register register, ushort offset) {
            _reg = register;
            _offset = offset;
        }

        public override ushort Get (IState state) {
            return state.Get((ushort)(state.Get(_reg) + _offset));
        }

        public override IState Set(IState prev, ushort value) {
            return prev.Set((ushort)(prev.Get(_reg) + _offset), value);
        }

        public override string ToString() {
            return string.Format("[{0} + 0x{1:X}]", _reg, _offset);
        }
    }

    public class Push : Address {
        public Push(ushort addr) : base(addr) {}

        public override string ToString() {
            return "Push";
        }
    }

    public class Pop : Address {
        public Pop(ushort addr) : base(addr) {}

        public override string ToString() {
            return "Pop";
        }
    }

    public class Peek : Address {
        public Peek(ushort addr) : base(addr) {}

        public override string ToString() {
            return "Peek";
        }
    }
}
