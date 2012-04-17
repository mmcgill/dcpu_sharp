using System;
using System.Windows.Forms;

namespace Com.MattMcGill.Dcpu
{
    public interface IKeyboard {
        void KeyPressed(char key);
    }

    public class Keyboard : IDevice, IKeyboard {
        public static readonly int BUFFER_WORDS = 16;

        private ushort[] _ringBuffer = new ushort[BUFFER_WORDS];
        private int _cursor = 0;

        public ushort Read(ushort offset) {
            lock (_ringBuffer) {
                return _ringBuffer[offset];
            }
        }

        public void Write(ushort addr, ushort value) {
            lock (_ringBuffer) {
                _ringBuffer[addr] = value;
            }
        }

        public void KeyPressed(char key) {
            lock (_ringBuffer) {
                _ringBuffer[_cursor] = key;
                _cursor = (_cursor + 1) % _ringBuffer.Length;
            }
        }
    }
}

