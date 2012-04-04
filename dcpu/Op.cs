using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.MattMcGill.Dcpu {
    /// <summary>
    /// Basic functionality common to DCPU operations.
    /// </summary>
    public abstract class Op : IOp {
        /// <summary>
        /// 6-bit A operand (2 high-order bits must be 0).
        /// </summary>
        public byte A { get; private set; }

        /// <summary>
        /// 6-bit B operand (2 high-order bits must be 0).
        /// </summary>
        public byte B { get; private set; }

        public abstract IState Apply(IState state);

        /// <summary>
        /// Decode the operand.
        /// </summary>
        /// <param name="operand">operand to decode</param>
        /// <param name="prev">current DCPU state</param>
        /// <param name="next">DCPU state after decoding</param>
        /// <returns>a value</returns>
        /// <remarks>
        /// Decode only modifies DCPU state for PUSH and POP operands.
        /// </remarks>
        public static ushort Decode(byte operand, IState prev, out IState next) {
            throw new NotImplementedException();
        }
    }
}
