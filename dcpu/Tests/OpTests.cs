using NUnit.Framework;

namespace Com.MattMcGill.Dcpu.Tests
{
    [TestFixture]
    public class OpTests {

        [Test]
        public void Set_SetRegisterToLiteral_RegisterHasCorrectValue() {
            var prev = new MutableState();
            var next = new Set(0x0, 0x25).Apply(prev);
            Assert.AreEqual(5, next.Get(Register.A));
        }

        [Test]
        public void Set_OperandAIsPush_ValueArrivesAtCorrectLocationAndSPIsUpdated() {
            var prev = new MutableState().Set(Register.SP, 1);
            var next = new Set(0x1a, 0x23).Apply(prev);
            Assert.AreEqual(0x3, next.Get((ushort)0));
            Assert.AreEqual(0, next.Get(Register.SP));
        }

        [Test]
        public void Add_AddLiteralToRegister_RegisterHasCorrectSum() {
            var prev = new MutableState().Set(Register.A, 14);
            var next = new Add(0x0, 0x25).Apply(prev);
            Assert.AreEqual(19, next.Get(Register.A));
        }

        [Test]
        public void Add_AddLiteralToRegister_OverflowCleared() {
            var prev = new MutableState().Set(Register.O, 1);
            var next = new Add(0x0, 0x25).Apply(prev);
            Assert.AreEqual(0, next.Get(Register.O));
        }

        [Test]
        public void Add_AddLiteralToRegisterWithOverflow_OverflowAndRegisterHaveCorrectValues() {
            var prev = new MutableState().Set(Register.A, 0xFFFE);
            var next = new Add(0x0, 0x25).Apply(prev);
            Assert.AreEqual(1, next.Get(Register.O));
            Assert.AreEqual(0x3, next.Get(Register.A));
        }

        [Test]
        public void Add_AddToOverflowRegisterWithoutOverflow_OverflowIsZero() {
            var prev = new MutableState().Set(Register.O, 1);
            var next = new Add(0x1d, 0x25).Apply(prev);
            Assert.AreEqual(0, next.Get(Register.O));
        }

        [Test]
        public void Add_AddToOverflowRegisterWithOverflow_OverflowIsOne() {
            var prev = new MutableState().Set(Register.O, 1).Set(Register.A, 0xFFFF);
            var next = new Add(0x1d, 0x0).Apply(prev);
            Assert.AreEqual(1, next.Get(Register.O));
        }

        [Test]
        public void Sub_SubtractRegisterAndLiteral_RegisterHasCorrectValue() {
            var state = new Sub(0x0, 0x25).Apply(new MutableState().Set(Register.A, 10));
            Assert.AreEqual(5, state.Get(Register.A));
            Assert.AreEqual(0, state.Get(Register.O));
        }

        [Test]
        public void Sub_SubtractRegisterAndLiteralWithOverflow_RegistersHaveCorrectValue() {
            var state = new Sub(0x0, 0x25).Apply(new MutableState().Set(Register.A, 3));
            Assert.AreEqual(0xFFFE, state.Get(Register.A));
            Assert.AreEqual(0xFFFF, state.Get(Register.O));
        }

        [Test]
        public void Mul_MultiplyRegisters_ResultAndOverflowAreCorrect() {
            var prev = new MutableState().Set(Register.A, 0x345).Set(Register.B, 0x678);
            var state = new Mul(0x0, 0x1).Apply(prev);
            Assert.AreEqual(0x2658, state.Get(Register.A));
            Assert.AreEqual(0x15, state.Get(Register.O));
        }

        [Test]
        public void Div_DivideRegisters_ResultAndOverflowAreCorrect() {
            var prev = new MutableState().Set(Register.A, 3).Set(Register.B, 2);
            var state = new Div(0x0, 0x1).Apply(prev);
            Assert.AreEqual(0x1, state.Get(Register.A));
            Assert.AreEqual(0x8000, state.Get(Register.O));
        }

        [Test]
        public void Div_DivisorIsZero_ResultAndOverflowAreZero() {
            var prev = new MutableState().Set(Register.A, 1);
            var state = new Div(0x0, 0x20).Apply(prev);
            Assert.AreEqual(0x0, state.Get(Register.A));
            Assert.AreEqual(0x0, state.Get(Register.O));
        }

        [Test]
        public void Mod_ModWithRegisters_ResultIsCorrect() {
            var prev = new MutableState().Set(Register.A, 3).Set(Register.B, 2);
            var state = new Mod(0x0, 0x1).Apply(prev);
            Assert.AreEqual(0x1, state.Get(Register.A));
        }

        [Test]
        public void Mod_SecondOperandIsZero_ResultIsZero() {
            var prev = new MutableState().Set(Register.A, 3).Set(Register.B, 0);
            var state = new Mod(0x0, 0x1).Apply(prev);
            Assert.AreEqual(0x0, state.Get(Register.A));
        }
    }
}
