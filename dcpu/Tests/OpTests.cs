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

        [TestCase]
        public void GetOperand_Pop_ReturnsValueAtSPAddress() {
            var prev = new MutableState();
            prev.Set(RegisterName.SP, 456);
            prev.Set(456, 42);
            IState next;

            Assert.AreEqual(42, Op.GetOperand(0x18, prev, out next));
        }

        [TestCase]
        public void GetOperand_Pop_SPIsIncremented() {
            var prev = new MutableState();
            prev.Set(RegisterName.SP, 456);
            prev.Set(456, 42);
            IState next;

            Op.GetOperand(0x18, prev, out next);

            Assert.AreEqual(457, next.Get(RegisterName.SP));
        }

        [TestCase]
        public void GetOperand_Peek_ReturnsValueAtSPAddress() {
            var prev = new MutableState();
            prev.Set(RegisterName.SP, 456);
            prev.Set(456, 42);
            IState next;

            Assert.AreEqual(42, Op.GetOperand(0x19, prev, out next));
        }

        [TestCase]
        public void GetOperand_Peek_SPIsNotIncremented() {
            var prev = new MutableState();
            prev.Set(RegisterName.SP, 456);
            prev.Set(456, 42);
            IState next;

            Op.GetOperand(0x19, prev, out next);

            Assert.AreEqual(456, next.Get(RegisterName.SP));
        }

        [TestCase]
        public void GetOperand_Push_ReturnsValueAtOneMinusSP() {
            var prev = new MutableState();
            prev.Set(RegisterName.SP, 456);
            prev.Set(456, 10);
            prev.Set(455, 20);
            IState next;

            Assert.AreEqual(20, Op.GetOperand(0x1a, prev, out next));
        }

        [TestCase]
        public void GetOperand_Push_DecrementsSP() {
            var prev = new MutableState();
            prev.Set(RegisterName.SP, 100);
            IState next;

            Op.GetOperand(0x1a, prev, out next);

            Assert.AreEqual(99, next.Get(RegisterName.SP));
        }

        [TestCase]
        public void GetOperand_SP_ReturnsSP() {
            var prev = new MutableState();
            prev.Set(RegisterName.SP, 12);
            IState next;

            Assert.AreEqual(12, Op.GetOperand(0x1b, prev, out next));
        }

        [TestCase]
        public void GetOperand_PC_ReturnsPC() {
            var prev = new MutableState();
            prev.Set(RegisterName.PC, 100);
            IState next;

            Assert.AreEqual(100, Op.GetOperand(0x1c, prev, out next));
        }

        [TestCase]
        public void GetOperand_O_ReturnsO() {
            var prev = new MutableState();
            prev.Set(RegisterName.O, 9);
            IState next;

            Assert.AreEqual(9, Op.GetOperand(0x1d, prev, out next));
        }

        [TestCase]
        public void GetOperand_NextWord_ReturnsValueAtPC() {
            var prev = new MutableState();
            prev.Set(RegisterName.PC, 1);
            prev.Set(1, 200);
            prev.Set(200, 42);
            IState next;

            Assert.AreEqual(42, Op.GetOperand(0x1e, prev, out next));
        }

        [TestCase]
        public void GetOperand_NextWord_AdvancesPC() {
            var prev = new MutableState();
            prev.Set(RegisterName.PC, 0);
            IState next;

            Op.GetOperand(0x1e, prev, out next);

            Assert.AreEqual(1, next.Get(RegisterName.PC));
        }

        [TestCase]
        public void GetOperand_NextAsLiteral_ReturnsValueOfNextWord() {
            var prev = new MutableState();
            prev.Set(RegisterName.PC, 0);
            prev.Set((ushort)0, 10);
            IState next;

            Assert.AreEqual(10, Op.GetOperand(0x1f, prev, out next));
        }

        [TestCase]
        public void GetOperand_NextAsLiteral_AdvancesPC() {
            var prev = new MutableState();
            prev.Set(RegisterName.PC, 0);
            IState next;

            Op.GetOperand(0x1f, prev, out next);

            Assert.AreEqual(1, next.Get(RegisterName.PC));
        }

        [TestCase]
        public void GetOperand_Literal_ReturnsLiteralValue() {
            var prev = new MutableState();
            IState next;

            Assert.AreEqual(5, Op.GetOperand(0x25, prev, out next));
        }
    }
}
