using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.MattMcGill.Dcpu {
    public class PersistentArray<T> {
        private readonly int _length;
        private readonly T _value;
        private readonly PersistentArray<T> _left;
        private readonly PersistentArray<T> _right;

        public int Length { get { return _length; } }

        public PersistentArray(int length) {
            _length = length;
            if (_length > 1) {
                _left = new PersistentArray<T>((length + 1) / 2);
                _right = new PersistentArray<T>(length - _left._length);
            }
        }

        public PersistentArray(T[] source) : this(source, 0, source.Length) {}

        public PersistentArray(T[] source, int index, int length) {
            _length = length;
            if (_length > 1) {
                _left = new PersistentArray<T>(source, index, (length + 1) / 2);
                _right = new PersistentArray<T>(source, index + _left._length, length - _left._length);
            } else {
                _value = source[index];
            }
        }

        private PersistentArray(T value) {
            _length = 1;
            _value = value;
        }

        private PersistentArray(PersistentArray<T> left, PersistentArray<T> right) {
            _length = left._length + right._length;
            _left = left;
            _right = right;
        }

        public T this[int index] {
            get {
                if (index >= _length)       throw new IndexOutOfRangeException();
                if (_length == 1)           return _value;
                if (index < _left._length)  return _left[index];
                else                        return _right[index - _left._length];
            }
        }

        public PersistentArray<T> Set(int index, T value) {
            if (index >= _length)       throw new IndexOutOfRangeException();
            if (_length == 1)           return new PersistentArray<T>(value);
            if (index < _left._length)  return new PersistentArray<T>(_left.Set(index, value), _right);
            else                        return new PersistentArray<T>(_left, _right.Set(index - _left._length, value));
        }
    }
}
