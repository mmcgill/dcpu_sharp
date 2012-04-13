using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.MattMcGill.Dcpu {
    /// <summary>
    /// High-level description of DCPU state that supports both mutable
    /// and immutable implementations.
    /// 
    /// Implementations need not be thread-safe.
    /// </summary>
    public interface IState {
        /// <summary>
        /// Get value of the given register.
        /// </summary>
        /// <param name="reg">a register name</param>
        /// <returns>value of reg</returns>
        ushort Get(Register reg);

        /// <summary>
        /// Get the word at the given address.
        /// </summary>
        /// <param name="addr">16-bit address</param>
        /// <returns>the word at the given address</returns>
        ushort Get(ushort addr);

        /// <summary>
        /// Set the value of the given register.
        /// </summary>
        /// <param name="reg">a register name</param>
        /// <param name="value">the new value</param>
        /// <returns>the state obtained by setting reg to value</returns>
        IState Set(Register reg, ushort value);

        /// <summary>
        /// Set the word at the given address.
        /// </summary>
        /// <param name="addr">address of the word to set</param>
        /// <param name="value">value to set at the address</param>
        /// <returns>the state obtained by setting the word at addr to value</returns>
        IState Set(ushort addr, ushort value);

        void MapMemory(ushort from, ushort to, IDevice device);
    }
}
