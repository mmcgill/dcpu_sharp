using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.MattMcGill.Dcpu {
    public interface IDevice {
        ushort Read(ushort addr);
        void Write(ushort addr, ushort value);
    }
}
