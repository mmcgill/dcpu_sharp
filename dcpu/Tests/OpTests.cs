using NUnit.Framework;

namespace Com.MattMcGill.Dcpu.Tests
{
    [TestFixture]
    public class OpTests {

        [Test]
        public void Set_SetRegisterToLiteral_RegisterHasCorrectValue() {
            var prev = new MutableState();
            var next = new Set(0x0, 0x25).Apply(prev);
            Assert.AreEqual(5, next.Get(RegisterName.A));
        }

        [Test]
        public void GetOperand_RegisterValue_ReturnsCorrectRegisterValue() {
            var prev = new MutableState();
            prev.Set(RegisterName.A, 34);
            IState next;

            Assert.AreEqual(34, Op.GetOperand(0x00, prev, out next));
        }

        [Test]
        public void SetOperand_RegisterValue_SetsCorrectRegisterValue() {
            var prev = new MutableState();
            var next = Op.SetOperand(0x02, 42, prev);

            Assert.AreEqual(42, next.Get(RegisterName.C));
        }

        [Test]
        public void GetOperand_IndirectRegister_ReturnsCorrectValueFromMemory() {
            var prev = new MutableState();
            prev.Set(RegisterName.A, 34);
            prev.Set(34, 42);
            IState next;

            Assert.AreEqual(42, Op.GetOperand(0x08, prev, out next));
        }

        [Test]
        public void SetOperand_IndirectRegister_SetsValueToCorrectAddress() {
            var prev = new MutableState();

            IState next = Op.SetOperand(0x9, 12, prev.Set(RegisterName.B, 19));

            Assert.AreEqual(12, next.Get(19));
        }

        [Test]
        public void GetOperand_IndirectAddressPlusRegister_ReturnsCorrectValueFromMemory() {
            var prev = new MutableState();
            prev.Set(RegisterName.Z, 10);
            prev.Set(1, 10);
            prev.Set(20, 42);
            IState next;

            Assert.AreEqual(42, Op.GetOperand(21, prev, out next));
        }

        [Test]
        public void SetOperand_IndirectAddressPlusRegister_SetValueInCorrectLocation() {
            var prev = new MutableState()
                .Set(RegisterName.I, 10)
                .Set(RegisterName.PC, 0)
                .Set((ushort)0, 15);

            var next = Op.SetOperand(0x16, 34, prev);

            Assert.AreEqual(34, next.Get(25));
        }

        [Test]
        public void GetOperand_IndirectAddresssPlusRegister_NextStatePCIsUpdated() {
            var prev = new MutableState();
            prev.Set(RegisterName.Z, 10);
            prev.Set(1, 10);
            prev.Set(20, 42);
            IState next;

            Op.GetOperand(21, prev, out next);
            
            Assert.AreEqual(1, next.Get(RegisterName.PC));
        }

        [Test]
        public void SetOperand_IndirectAddressPlusRegister_IncrementsPC() {
            var next = Op.SetOperand(0x16, 34, new MutableState().Set(RegisterName.PC, 0));

            Assert.AreEqual(1, next.Get(RegisterName.PC));
        }

        [Test]
        public void GetOperand_Pop_ReturnsValueAtSPAddress() {
            var prev = new MutableState();
            prev.Set(RegisterName.SP, 456);
            prev.Set(456, 42);
            IState next;

            Assert.AreEqual(42, Op.GetOperand(0x18, prev, out next));
        }

        [Test]
        public void SetOperand_Pop_SetsValueAtSPAddress() {
            var prev = new MutableState().Set(RegisterName.SP, 1);

            var next = Op.SetOperand(0x18, 42, prev);

            Assert.AreEqual(42, next.Get((ushort)1));
        }

        [Test]
        public void GetOperand_Pop_SPIsIncremented() {
            var prev = new MutableState();
            prev.Set(RegisterName.SP, 456);
            prev.Set(456, 42);
            IState next;

            Op.GetOperand(0x18, prev, out next);

            Assert.AreEqual(457, next.Get(RegisterName.SP));
        }

        [Test]
        public void SetOperand_Pop_SPIsIncremented() {
            var state = Op.SetOperand(0x18, 0, new MutableState().Set(RegisterName.SP, 1));
            Assert.AreEqual(2, state.Get(RegisterName.SP));
        }

        [Test]
        public void GetOperand_Peek_ReturnsValueAtSPAddress() {
            var prev = new MutableState();
            prev.Set(RegisterName.SP, 456);
            prev.Set(456, 42);
            IState next;

            Assert.AreEqual(42, Op.GetOperand(0x19, prev, out next));
        }

        [Test]
        public void SetOperand_Peek_SetsValueAtSPAddress() {
            var prev = new MutableState().Set(RegisterName.SP, 5);

            Assert.AreEqual(10, Op.SetOperand(0x19, 10, prev).Get(5));
        }

        [Test]
        public void GetOperand_Peek_SPIsNotIncremented() {
            var prev = new MutableState();
            prev.Set(RegisterName.SP, 456);
            prev.Set(456, 42);
            IState next;

            Op.GetOperand(0x19, prev, out next);

            Assert.AreEqual(456, next.Get(RegisterName.SP));
        }

        [Test]
        public void SetOperand_Peek_SPIsNotIncremented() {
            var prev = new MutableState().Set(RegisterName.SP, 1);

            Assert.AreEqual(1, Op.SetOperand(0x19, 10, prev).Get(RegisterName.SP));
        }

        [Test]
        public void GetOperand_Push_ReturnsValueAtOneMinusSP() {
            var prev = new MutableState();
            prev.Set(RegisterName.SP, 456);
            prev.Set(456, 10);
            prev.Set(455, 20);
            IState next;

            Assert.AreEqual(20, Op.GetOperand(0x1a, prev, out next));
        }

        [Test]
        public void SetOperand_Push_SetsValueAtOneMinusSP() {
            var prev = new MutableState().Set(RegisterName.SP, 10);
            
            Assert.AreEqual(42, Op.SetOperand(0x1a, 42, prev).Get(9));
        }

        [Test]
        public void GetOperand_Push_DecrementsSP() {
            var prev = new MutableState();
            prev.Set(RegisterName.SP, 100);
            IState next;

            Op.GetOperand(0x1a, prev, out next);

            Assert.AreEqual(99, next.Get(RegisterName.SP));
        }

        [Test]
        public void SetOperand_Push_DecrementsSP() {
            var prev = new MutableState().Set(RegisterName.SP, 10);

            Assert.AreEqual(9, Op.SetOperand(0x1a, 1, prev).Get(RegisterName.SP));
        }

        [Test]
        public void GetOperand_SP_ReturnsSP() {
            var prev = new MutableState();
            prev.Set(RegisterName.SP, 12);
            IState next;

            Assert.AreEqual(12, Op.GetOperand(0x1b, prev, out next));
        }

        [Test]
        public void SetOperand_SP_SetsSP() {
            var prev = new MutableState().Set(RegisterName.SP, 10);

            Assert.AreEqual(42, Op.SetOperand(0x1b, 42, prev).Get(RegisterName.SP));
        }

        [Test]
        public void GetOperand_PC_ReturnsPC() {
            var prev = new MutableState();
            prev.Set(RegisterName.PC, 100);
            IState next;

            Assert.AreEqual(100, Op.GetOperand(0x1c, prev, out next));
        }

        [Test]
        public void SetOperand_PC_SetsPC() {
            var prev = new MutableState().Set(RegisterName.PC, 10);

            Assert.AreEqual(42, Op.SetOperand(0x1c, 42, prev).Get(RegisterName.PC));
        }

        [Test]
        public void GetOperand_O_ReturnsO() {
            var prev = new MutableState();
            prev.Set(RegisterName.O, 9);
            IState next;

            Assert.AreEqual(9, Op.GetOperand(0x1d, prev, out next));
        }

        [Test]
        public void SetOperand_O_SetsO() {
            var prev = new MutableState().Set(RegisterName.O, 10);

            Assert.AreEqual(42, Op.SetOperand(0x1d, 42, prev).Get(RegisterName.O));
        }

        [Test]
        public void GetOperand_NextWord_ReturnsValueAtPC() {
            var prev = new MutableState();
            prev.Set(RegisterName.PC, 1);
            prev.Set(1, 200);
            prev.Set(200, 42);
            IState next;

            Assert.AreEqual(42, Op.GetOperand(0x1e, prev, out next));
        }

        [Test]
        public void SetOperand_NextWord_SetsValueAtAddrInPC() {
            var prev = new MutableState().Set(RegisterName.PC, 22);

            Assert.AreEqual(99, Op.SetOperand(0x1e, 99, prev).Get(22));
        }

        [Test]
        public void GetOperand_NextWord_AdvancesPC() {
            var prev = new MutableState();
            prev.Set(RegisterName.PC, 0);
            IState next;

            Op.GetOperand(0x1e, prev, out next);

            Assert.AreEqual(1, next.Get(RegisterName.PC));
        }

        [Test]
        public void SetOperand_NextWord_AdvancesPC() {
            var prev = new MutableState().Set(RegisterName.PC, 10);

            Assert.AreEqual(11, Op.SetOperand(0x1e, 42, prev).Get(RegisterName.PC));
        }

        [Test]
        public void GetOperand_NextAsLiteral_ReturnsValueOfNextWord() {
            var prev = new MutableState();
            prev.Set(RegisterName.PC, 0);
            prev.Set((ushort)0, 10);
            IState next;

            Assert.AreEqual(10, Op.GetOperand(0x1f, prev, out next));
        }

        [Test]
        public void SetOperand_NextAsLiteral_DoesNotSetValue() {
            var prev = new MutableState().Set(RegisterName.PC, 10);

            Assert.AreEqual(0, Op.SetOperand(0x1f, 5, prev).Get(10));
        }

        [Test]
        public void GetOperand_NextAsLiteral_AdvancesPC() {
            var prev = new MutableState();
            prev.Set(RegisterName.PC, 0);
            IState next;

            Op.GetOperand(0x1f, prev, out next);

            Assert.AreEqual(1, next.Get(RegisterName.PC));
        }

        [Test]
        public void SetOperand_NextAsLiteral_AdvancesPC() {
            var prev = new MutableState().Set(RegisterName.PC, 10);

            Assert.AreEqual(11, Op.SetOperand(0x1f, 99, prev).Get(RegisterName.PC));
        }

        [Test]
        public void GetOperand_Literal_ReturnsLiteralValue() {
            var prev = new MutableState();
            IState next;

            Assert.AreEqual(5, Op.GetOperand(0x25, prev, out next));
        }

        [Test]
        public void SetOperand_Literal_AdvancesPC() {
            var prev = new MutableState().Set(RegisterName.PC, 10);

            Assert.AreEqual(11, Op.SetOperand(0x22, 0, prev).Get(RegisterName.PC));
        }
    }
}
