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

            IState next;
            var op = Dcpu.FetchNextInstruction(state, out next);

            Assert.AreEqual(2, next.Get(Register.PC));
            Assert.IsTrue(op is Add);

            next = op.Apply(next);
            Assert.AreEqual(42, state.Get(Register.A));
        }

        [Test]
        public void GetOperand_RegisterValue_ReturnsCorrectRegisterValue() {
            var prev = new MutableState();
            prev.Set(Register.A, 34);
            IState next;

            Assert.AreEqual(34, Dcpu.GetOperand(0x00, prev, out next).Get(next));
        }

        [Test]
        public void GetOperand_IndirectRegister_ReturnsCorrectAddressOperand() {
            var prev = new MutableState();
            prev.Set(Register.A, 34);
            prev.Set(34, 42);
            IState next;

            Assert.AreEqual(42, Dcpu.GetOperand(0x08, prev, out next).Get(next));
        }

        [Test]
        public void GetOperand_IndirectAddressPlusRegister_ReturnsCorrectAddressOperand() {
            var prev = new MutableState();
            prev.Set(Register.Z, 10);
            prev.Set((ushort)0, 10);
            prev.Set(20, 42);
            IState next;

            Assert.AreEqual(42, Dcpu.GetOperand(21, prev, out next).Get(next));
        }

        [Test]
        public void GetOperand_IndirectAddresssPlusRegister_NextStatePCIsUpdated() {
            var prev = new MutableState();
            prev.Set(Register.Z, 10);
            prev.Set(1, 10);
            prev.Set(20, 42);
            IState next;

            Dcpu.GetOperand(21, prev, out next);
            
            Assert.AreEqual(1, next.Get(Register.PC));
        }

        [Test]
        public void GetOperand_Pop_ReturnsValueAtAddressInSP() {
            var prev = new MutableState();
            prev.Set(Register.SP, 456);
            prev.Set(456, 42);
            IState next;

            Assert.AreEqual(42, Dcpu.GetOperand(0x18, prev, out next).Get(next));
        }

        [Test]
        public void GetOperand_Pop_SPIsIncremented() {
            var prev = new MutableState();
            prev.Set(Register.SP, 456);
            prev.Set(456, 42);
            IState next;

            Dcpu.GetOperand(0x18, prev, out next);

            Assert.AreEqual(457, next.Get(Register.SP));
        }

        [Test]
        public void GetOperand_Peek_ReturnsValueAtSPAddress() {
            var prev = new MutableState();
            prev.Set(Register.SP, 456);
            prev.Set(456, 42);
            IState next;

            Assert.AreEqual(42, Dcpu.GetOperand(0x19, prev, out next).Get(next));
        }

        [Test]
        public void GetOperand_Peek_SPIsNotIncremented() {
            var prev = new MutableState();
            prev.Set(Register.SP, 456);
            prev.Set(456, 42);
            IState next;

            Dcpu.GetOperand(0x19, prev, out next);

            Assert.AreEqual(456, next.Get(Register.SP));
        }

        [Test]
        public void GetOperand_Push_ReturnsValueAtOneMinusSP() {
            var prev = new MutableState();
            prev.Set(Register.SP, 456);
            prev.Set(456, 10);
            prev.Set(455, 20);
            IState next;

            Assert.AreEqual(20, Dcpu.GetOperand(0x1a, prev, out next).Get(next));
        }

        [Test]
        public void GetOperand_Push_DecrementsSP() {
            var prev = new MutableState();
            prev.Set(Register.SP, 100);
            IState next;

            Dcpu.GetOperand(0x1a, prev, out next);

            Assert.AreEqual(99, next.Get(Register.SP));
        }

        [Test]
        public void GetOperand_SP_ReturnsSP() {
            var prev = new MutableState();
            prev.Set(Register.SP, 12);
            IState next;

            Assert.AreEqual(12, Dcpu.GetOperand(0x1b, prev, out next).Get(next));
        }

        [Test]
        public void GetOperand_PC_ReturnsPC() {
            var prev = new MutableState();
            prev.Set(Register.PC, 100);
            IState next;

            Assert.AreEqual(100, Dcpu.GetOperand(0x1c, prev, out next).Get(next));
        }

        [Test]
        public void GetOperand_O_ReturnsO() {
            var prev = new MutableState();
            prev.Set(Register.O, 9);
            IState next;

            Assert.AreEqual(9, Dcpu.GetOperand(0x1d, prev, out next).Get(next));
        }

        [Test]
        public void GetOperand_NextWord_ReturnsValueAtPC() {
            var prev = new MutableState();
            prev.Set(Register.PC, 1);
            prev.Set(1, 200);
            prev.Set(200, 42);
            IState next;

            Assert.AreEqual(42, Dcpu.GetOperand(0x1e, prev, out next).Get(next));
        }

        [Test]
        public void GetOperand_NextWord_AdvancesPC() {
            var prev = new MutableState();
            prev.Set(Register.PC, 0);
            IState next;

            Dcpu.GetOperand(0x1e, prev, out next);

            Assert.AreEqual(1, next.Get(Register.PC));
        }

        [Test]
        public void GetOperand_NextAsLiteral_ReturnsValueOfNextWord() {
            var prev = new MutableState();
            prev.Set(Register.PC, 0);
            prev.Set((ushort)0, 10);
            IState next;

            Assert.AreEqual(10, Dcpu.GetOperand(0x1f, prev, out next).Get(next));
        }

        [Test]
        public void GetOperand_NextAsLiteral_AdvancesPC() {
            var prev = new MutableState();
            prev.Set(Register.PC, 0);
            IState next;

            Dcpu.GetOperand(0x1f, prev, out next);

            Assert.AreEqual(1, next.Get(Register.PC));
        }

        [Test]
        public void GetOperand_Literal_ReturnsLiteralValue() {
            var prev = new MutableState();
            IState next;

            Assert.AreEqual(5, Dcpu.GetOperand(0x25, prev, out next).Get(next));
        }
    }
}

