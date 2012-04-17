using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Com.MattMcGill.Dcpu {
    public partial class Display : Form, IDevice {
        public const int WIDTH = 32;
        public const int HEIGHT = 12;

        private ushort[] _buffer = new ushort[WIDTH * HEIGHT];
        private IKeyboard _keyboard;

        private Bitmap _tileset;

        public Display(IKeyboard keyboard) {
            _keyboard = keyboard;
            InitializeComponent();
            LoadDefaultTileset();
            WriteDisplayFromBuffer();

            KeyPress += new KeyPressEventHandler(HandleKeyPress);
        }

        private void LoadDefaultTileset() {
            var assembly = typeof(Dcpu).Assembly;
            using (var stream = assembly.GetManifestResourceStream(typeof(Dcpu), "tileset.png")) {
                _tileset = new Bitmap(stream);
            }
        }

        private void HandleKeyPress(object sender, KeyPressEventArgs args) {
            _keyboard.KeyPressed(args.KeyChar);
            args.Handled = true;
        }

        private void WriteDisplayFromBuffer() {
            Invalidate();
        }

        public ushort Read(ushort addr) {
            lock (_buffer) {
                return _buffer[addr];
            }
        }

        public void Write(ushort addr, ushort value) {
            lock (_buffer) {
                _buffer[addr] = value;
            }
            Invoke(new Action(WriteDisplayFromBuffer));
        }

        private void DoPaint(object sender, PaintEventArgs e) {
            var g = e.Graphics;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            for (int row=0; row < HEIGHT; ++row) {
                for (int col=0; col < WIDTH; ++col) {
                    PaintTile(g, row, col, _buffer[row * WIDTH + col]);
                }
            }
        }

        private void PaintTile(Graphics g, int row, int col, ushort word) {
            byte ch = (byte)(word & 0x007F);
            int tileX = ch % 16;
            int tileY = ch / 16;
            g.DrawImage(_tileset,
                new Rectangle(col * 4 * 3, row * 8 * 3, 12, 24),
                new Rectangle(tileX * 4 * 3, tileY * 8 * 3, 12, 24),
                GraphicsUnit.Pixel);
        }
    }
}
