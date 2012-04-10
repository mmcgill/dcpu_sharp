using NUnit.Framework;

namespace Com.MattMcGill.Dcpu.Tests
{
    [TestFixture]
    public class BasicOpTest {

        [Test]
        public void Set_SetRegisterToLiteral_RegisterHasCorrectValue() {
            var prev = new MutableState();
            var next = new Set(new Reg(Register.A), new Literal(0x5)).Apply(prev);
            Assert.AreEqual(5, next.Get(Register.A));
        }

        [Test]
        public void Add_AddLiteralToRegister_RegisterHasCorrectSum() {
            var prev = new MutableState().Set(Register.A, 14);
            var next = new Add(new Reg(Register.A), new Literal(0x5)).Apply(prev);
            Assert.AreEqual(19, next.Get(Register.A));
        }

        [Test]
        public void Add_AddLiteralToRegister_OverflowCleared() {
            var prev = new MutableState().Set(Register.O, 1);
            var next = new Add(new Reg(Register.A), new Literal(0x5)).Apply(prev);
            Assert.AreEqual(0, next.Get(Register.O));
        }

        [Test]
        public void Add_AddLiteralToRegisterWithOverflow_OverflowAndRegisterHaveCorrectValues() {
            var prev = new MutableState().Set(Register.A, 0xFFFE);
            var next = new Add(new Reg(Register.A), new Literal(0x5)).Apply(prev);
            Assert.AreEqual(1, next.Get(Register.O));
            Assert.AreEqual(0x3, next.Get(Register.A));
        }

        [Test]
        public void Add_AddToOverflowRegisterWithoutOverflow_OverflowIsZero() {
            var prev = new MutableState().Set(Register.O, 1);
            var next = new Add(new Reg(Register.O), new Literal(0x5)).Apply(prev);
            Assert.AreEqual(0, next.Get(Register.O));
        }

        [Test]
        public void Add_AddToOverflowRegisterWithOverflow_OverflowIsOne() {
            var prev = new MutableState().Set(Register.O, 1).Set(Register.A, 0xFFFF);
            var next = new Add(new Reg(Register.O), new Reg(Register.A)).Apply(prev);
            Assert.AreEqual(1, next.Get(Register.O));
        }

        [Test]
        public void Sub_SubtractRegisterAndLiteral_RegisterHasCorrectValue() {
            var state = new Sub(new Reg(Register.A), new Literal(0x5)).Apply(new MutableState().Set(Register.A, 10));
            Assert.AreEqual(5, state.Get(Register.A));
            Assert.AreEqual(0, state.Get(Register.O));
        }

        [Test]
        public void Sub_SubtractRegisterAndLiteralWithOverflow_RegistersHaveCorrectValue() {
            var state = new Sub(new Reg(Register.A), new Literal(0x5)).Apply(new MutableState().Set(Register.A, 3));
            Assert.AreEqual(0xFFFE, state.Get(Register.A));
            Assert.AreEqual(0xFFFF, state.Get(Register.O));
        }

        [Test]
        public void Mul_MultiplyRegisters_ResultAndOverflowAreCorrect() {
            var prev = new MutableState().Set(Register.A, 0x345).Set(Register.B, 0x678);
            var state = new Mul(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(0x2658, state.Get(Register.A));
            Assert.AreEqual(0x15, state.Get(Register.O));
        }

        [Test]
        public void Div_DivideRegisters_ResultAndOverflowAreCorrect() {
            var prev = new MutableState().Set(Register.A, 3).Set(Register.B, 2);
            var state = new Div(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(0x1, state.Get(Register.A));
            Assert.AreEqual(0x8000, state.Get(Register.O));
        }

        [Test]
        public void Div_DivisorIsZero_ResultAndOverflowAreZero() {
            var prev = new MutableState().Set(Register.A, 1);
            var state = new Div(new Reg(Register.A), new Literal(0)).Apply(prev);
            Assert.AreEqual(0x0, state.Get(Register.A));
            Assert.AreEqual(0x0, state.Get(Register.O));
        }

        [Test]
        public void Mod_ModWithRegisters_ResultIsCorrect() {
            var prev = new MutableState().Set(Register.A, 3).Set(Register.B, 2);
            var state = new Mod(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(0x1, state.Get(Register.A));
        }

        [Test]
        public void Mod_SecondOperandIsZero_ResultIsZero() {
            var prev = new MutableState().Set(Register.A, 3).Set(Register.B, 0);
            var state = new Mod(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(0x0, state.Get(Register.A));
        }

        [Test]
        public void Shl_RegisterAndLiteral_ResultAndOverflowAreCorrect() {
            var prev = new MutableState().Set(Register.A, 0x5201);
            var state = new Shl(new Reg(Register.A), new Literal(4)).Apply(prev);
            Assert.AreEqual(0x2010, state.Get(Register.A));
            Assert.AreEqual(0x5, state.Get(Register.O));
        }

        [Test]
        public void Shl_SecondOperandGreaterThan15_ShiftPerformedModulo16() {
            // TODO: Get clarification on the DCPU spec that this is actually
            // TODO: how shifts should work.
            var prev = new MutableState().Set(Register.A, 0x1234).Set(Register.B, 20);
            var state = new Shl(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(0x2340, state.Get(Register.A));
            Assert.AreEqual(0x0001, state.Get(Register.O));
        }

        [Test]
        public void Shr_RegisterAndLiteral_ResultAndOverflowAreCorrect() {
            var prev = new MutableState().Set(Register.A, 0x3456);
            var state = new Shr(new Reg(Register.A), new Literal(4)).Apply(prev);
            Assert.AreEqual(0x0345, state.Get(Register.A));
            Assert.AreEqual(0x6, state.Get(Register.O));
        }

        [Test]
        public void Shr_SecondOperandGreaterThan15_ShiftPerformedModulo16() {
            // TODO: Get clarification on the DCPU spec that this is actually
            // TODO: how shifts should work.
            var prev = new MutableState().Set(Register.A, 0x3456).Set(Register.B, 20);
            var state = new Shr(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(0x0345, state.Get(Register.A));
            Assert.AreEqual(0x6, state.Get(Register.O));
        }

        [Test]
        public void And_RegisterOperands_ResultIsCorrect() {
            var prev = new MutableState().Set(Register.A, 6).Set(Register.B, 5);
            var state = new And(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(0x04, state.Get(Register.A));
        }

        [Test]
        public void Bor_RegisterOperands_ResultIsCorrect() {
            var prev = new MutableState().Set(Register.A, 6).Set(Register.B, 5);
            var state = new Bor(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(0x7, state.Get(Register.A));
        }

        [Test]
        public void Xor_RegisterOperands_ResultIsCorrect() {
            var prev = new MutableState().Set(Register.A, 6).Set(Register.B, 5);
            var state = new Xor(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(0x3, state.Get(Register.A));
        }

        [Test]
        public void Ife_RegisterOperandsAreEqual_PCIsNotUpdated() {
            var prev = new MutableState().Set(Register.A, 1).Set(Register.B, 1);
            var state = new Ife(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(0, state.Get(Register.PC));
        }

        [Test]
        public void Ife_RegisterOperandsAreNotEqual_PCSkipsNextInstructionAndOperands() {
            var prev = new MutableState()
                .Set(Register.A, 1).Set(Register.B, 2)
                .Set((ushort)0, 0x7C02)
                .Set((ushort)1, 0x002A);
            var state = new Ife(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(2, state.Get(Register.PC));
        }

        [Test]
        public void Ifn_RegisterOperandsAreNotEqual_PCIsNotUpdated() {
            var prev = new MutableState().Set(Register.A, 1).Set(Register.B, 2);
            var state = new Ifn(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(0, state.Get(Register.PC));
        }

        [Test]
        public void Ifn_RegisterOperandsAreEqual_PCSkipsNextInstructionAndOperands() {
            var prev = new MutableState()
                .Set(Register.A, 1).Set(Register.B, 1)
                .Set((ushort)0, 0x7C02)
                .Set((ushort)1, 0x002A);
            var state = new Ifn(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(2, state.Get(Register.PC));
        }

        [Test]
        public void Ifg_FirstOperandGreater_PCIsNotUpdated() {
            var prev = new MutableState().Set(Register.A, 2).Set(Register.B, 1);
            var state = new Ifg(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(0, state.Get(Register.PC));
        }

        [Test]
        public void Ifg_FirstOperandNotGreater_PCSkipsNextInstructionAndOperands() {
            var prev = new MutableState()
                .Set(Register.A, 1).Set(Register.B, 1)
                .Set((ushort)0, 0x7C02)
                .Set((ushort)1, 0x002A);
            var state = new Ifg(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(2, state.Get(Register.PC));
        }

        [Test]
        public void Ifb_AndOfOperandsIsNonZero_PCIsNotUpdated() {
            var prev = new MutableState().Set(Register.A, 3).Set(Register.B, 1);
            var state = new Ifb(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(0, state.Get(Register.PC));
        }

        [Test]
        public void Ifb_AndOfOperandsIsZero_PCSkipsNextInstructionAndOperands() {
            var prev = new MutableState()
                .Set(Register.A, 4).Set(Register.B, 2)
                .Set((ushort)0, 0x7C02)
                .Set((ushort)1, 0x002A);
            var state = new Ifb(new Reg(Register.A), new Reg(Register.B)).Apply(prev);
            Assert.AreEqual(2, state.Get(Register.PC));
        }
    }
}
