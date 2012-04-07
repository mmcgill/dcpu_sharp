using System;
using NUnit.Framework;

namespace Com.MattMcGill.Dcpu {

    [TestFixture]
    public class DcpuTest {

        [Test]
        public void ExecuteInstruction_InstructionGetsDecodedAndExecuted() {
            var state = new MutableState()
                .Set((ushort)0, 0x7C02)  // ADD A, next word
                .Set((ushort)1, 0x002A); // 42

            state = Dcpu.ExecuteInstruction(state);

            Assert.AreEqual(42, state.Get(RegisterName.A));
            Assert.AreEqual(2, state.Get (RegisterName.PC));
        }

    }

}

