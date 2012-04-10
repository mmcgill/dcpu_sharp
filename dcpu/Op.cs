using System;

namespace Com.MattMcGill.Dcpu {
    public abstract class Op {
        public abstract IState Apply(IState prev);
    }
}

