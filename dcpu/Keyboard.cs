using System;
using System.Windows.Forms;

namespace Com.MattMcGill.Dcpu
{
    public interface IKeyboard {
        void KeyPressed(char key);
    }

    public class Keyboard : IDevice, IKeyboard {
        public static readonly int KEYBOARD_BUFFER_WORDS = 16;
        public static readonly ushort KEYBOARD_ADDR = 0x9000;

        private ushort[] _ringBuffer = new ushort[KEYBOARD_BUFFER_WORDS];
        private int _cursor = 0;

        public ushort Read(ushort addr) {
            lock (_ringBuffer) {
                return _ringBuffer[addr - KEYBOARD_ADDR];
            }
        }

        public void Write(ushort addr, ushort value) {
            lock (_ringBuffer) {
                _ringBuffer[addr - KEYBOARD_ADDR] = value;
            }
        }

        public void KeyPressed(char key) {
            lock (_ringBuffer) {
                _ringBuffer[_cursor] = key;
                _cursor = (_cursor + 1) % _ringBuffer.Length;
            }
        }

        public void Map(IState state) {
            state.MapMemory(KEYBOARD_ADDR, (ushort)(KEYBOARD_ADDR + KEYBOARD_BUFFER_WORDS), this);
        }
    }
}

