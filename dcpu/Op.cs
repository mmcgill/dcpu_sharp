using System;

namespace Com.MattMcGill.Dcpu {
    public abstract class Op {
        /// <summary>
        /// Apply the instruction to the given state to produce a new state.
        /// </summary>
        /// <param name="prev">the state before applying the instruction</param>
        /// <returns>a new CPU state</returns>
        public abstract IState Apply(IState prev);

        /// <summary>
        /// Compute the number of cycles necessary to execute this instruction from
        /// the given state.
        /// </summary>
        /// <param name="state">a system state</param>
        /// <returns>number of cycles</returns>
        public abstract int Cycles(IState state);
    }
}

