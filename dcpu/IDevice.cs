using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.MattMcGill.Dcpu {
    public interface IDevice {
        /// <summary>
        /// Map the device to the state's memory.
        /// </summary>
        /// <param name="state"></param>
        void Map(IState state);

        /// <summary>
        /// Read from the device.
        /// </summary>
        /// <param name="addr">the address to read from</param>
        /// <returns></returns>
        ushort Read(ushort addr);

        /// <summary>
        /// Write to the device.
        /// </summary>
        /// <param name="addr">the address to write to</param>
        /// <param name="value">the word to write</param>
        void Write(ushort addr, ushort value);
    }
}
