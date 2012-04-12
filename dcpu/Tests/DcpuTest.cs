using System;
using NUnit.Framework;

namespace Com.MattMcGill.Dcpu {

    [TestFixture]
    public class DcpuTest {

        [Test]
        public void FetchNextInstruction_InstructionGetsCorrectlyDecoded() {
            var state = new MutableState()
                .Set((ushort)0, 0x7C02)  // ADD A, next word
                .Set((ushort)1, 0x002A); // 42

            ushort pc=0, sp=0;
            var op = Dcpu.FetchNextInstruction(state, ref pc, ref sp);

            Assert.AreEqual(2, pc);
            Assert.IsTrue(op is Add);

            state = op.Apply(state);
            Assert.AreEqual(42, state.Get(Register.A));
        }
    }
}

