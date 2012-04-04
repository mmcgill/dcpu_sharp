using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.MattMcGill.Dcpu {
    /// <summary>
    /// An operation on DCPU state.
    /// </summary>
    public interface IOp {
        IState Apply(IState state);
    }
}
