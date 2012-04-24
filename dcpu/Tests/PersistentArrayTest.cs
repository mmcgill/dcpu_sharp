using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Com.MattMcGill.Dcpu.Tests {
    [TestFixture]
    public class PersistentArrayTest {
        [Test]
        public void EmptyArrayHasLengthZero() {
            Assert.AreEqual(0, new PersistentArray<string>(0).Length);
        }

        [Test]
        public void EmptyArrayThrowsIndexOutOfRange() {
            Assert.Throws<IndexOutOfRangeException>(() => new PersistentArray<string>(0).Set(0, "foo"));
        }

        [Test]
        public void Set_LengthOneArray_OldValueIdentical() {
            var array = new PersistentArray<string>(1);
            var array2 = array.Set(0, "foo");
            Assert.AreEqual(null, array[0]);
        }

        [Test]
        public void Set_LengthOneArray_NewValueCorrect() {
            var array = new PersistentArray<string>(1);
            var array2 = array.Set(0, "foo");
            Assert.AreEqual("foo", array2[0]);
        }

        [Test]
        public void GetAndSet_ManyElementArray_CorrectElementsReturned() {
            var array = new PersistentArray<int>(100);
            for (int i=0; i < 100; ++i) {
                array = array.Set(i, 2 * i);
            }

            for (int i=0; i < 100; ++i) {
                Assert.AreEqual(2 * i, array[i]);
            }

            var array2 = array;
            for (int i=0; i < 100; ++i) {
                array2 = array2.Set(i, array[99 - i]);
            }

            for (int i=99; i >= 0; --i) {
                Assert.AreEqual(2 * i, array2[99 - i]);
            }
        }

        [Test]
        public void ConstructFromSourceArray() {
            var source = new int[60];
            for (int i=0; i < source.Length; ++i)
                source[i] = i * 3;
            var array = new PersistentArray<int>(source);
            for (int i=0; i < source.Length; ++i)
                Assert.AreEqual(source[i], array[i]);
        }
    }
}
