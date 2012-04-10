using System;
using NUnit.Framework;

namespace Com.MattMcGill.Dcpu {
    [TestFixture]
    public class NonBasicOpTest {
        [Test]
        public void Jsr_RegisterOperand_PCSetToRegisterValue() {
            var prev = new MutableState()
                .Set(Register.A, 0x10);
            var next = new Jsr(new Reg(Register.A)).Apply(prev);
            Assert.AreEqual(0x10, next.Get(Register.PC));
        }

        [Test]
        public void Jsr_RegisterOperand_PCValuePushedOnStack() {
            var prev = new MutableState()
                .Set(Register.A, 0x10)
                .Set(Register.PC, 0x5)
                .Set(Register.SP, 0xFFFF);
            var next = new Jsr(new Reg(Register.A)).Apply(prev);
            Assert.AreEqual(5, next.Get(0xFFFE));
            Assert.AreEqual(0xFFFE, next.Get(Register.SP));
        }
    }
}

