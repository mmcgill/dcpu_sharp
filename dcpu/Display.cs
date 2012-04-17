using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Com.MattMcGill.Dcpu {
    public partial class Display : Form, IDevice {
        public const int WIDTH = 32;
        public const int HEIGHT = 12;
        private char[] buffer = new char[WIDTH * HEIGHT];
        private IKeyboard _keyboard;

        public Display(IKeyboard keyboard) {
            _keyboard = keyboard;
            InitializeComponent();
            WriteDisplayFromBuffer();

            KeyPress += new KeyPressEventHandler(HandleKeyPress);
        }

        private void HandleKeyPress(object sender, KeyPressEventArgs args) {
            _keyboard.KeyPressed(args.KeyChar);
            args.Handled = true;
        }

        private void WriteDisplayFromBuffer() {
            var text = new StringBuilder();
            lock (buffer) {
                for (int row = 0; row < HEIGHT; ++row) {
                    for (int col = 0; col < WIDTH; ++col) {
                        text.Append(buffer[row * WIDTH + col]);
                    }
                    text.AppendLine();
                }
            }
            displayLabel.Text = text.ToString();
        }

        public ushort Read(ushort addr) {
            lock (buffer) {
                return buffer[addr];
            }
        }

        public void Write(ushort addr, ushort value) {
            lock (buffer) {
                buffer[addr] = (char)value;
            }
            Invoke(new Action(WriteDisplayFromBuffer));
        }
    }
}
