using NUnit.Framework;

namespace Com.MattMcGill.Dcpu.Tests
{
    [TestFixture]
    public class OpTests {

        [TestCase]
        public void GetOperand_RegisterValue_ReturnsCorrectRegisterValue() {
            var prev = new MutableState();
            prev.Set(RegisterName.A, 34);
            IState next;

            Assert.AreEqual(34, Op.GetOperand(0x00, prev, out next));
        }

        [TestCase]
        public void GetOperand_IndirectRegister_ReturnsCorrectValueFromMemory() {
            var prev = new MutableState();
            prev.Set(RegisterName.A, 34);
            prev.Set(34, 42);
            IState next;

            Assert.AreEqual(42, Op.GetOperand(0x08, prev, out next));
        }

        [TestCase]
        public void GetOperand_IndirectAddressPlusRegister_ReturnsCorrectValueFromMemory() {
            var prev = new MutableState();
            prev.Set(RegisterName.Z, 10);
            prev.Set(1, 10);
            prev.Set(20, 42);
            IState next;

            Assert.AreEqual(42, Op.GetOperand(21, prev, out next));
        }

        [TestCase]
        public void GetOperand_IndirectAddresssPlusRegister_NextStatePCIsUpdated() {
            var prev = new MutableState();
            prev.Set(RegisterName.Z, 10);
            prev.Set(1, 10);
            prev.Set(20, 42);
            IState next;

            Op.GetOperand(21, prev, out next);
            
            Assert.AreEqual(1, next.Get(RegisterName.PC));
        }
    }
}
