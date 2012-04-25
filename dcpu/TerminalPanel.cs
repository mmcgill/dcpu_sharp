using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Com.MattMcGill.Dcpu {
    public partial class TerminalPanel : UserControl {

        public Dcpu Dcpu { get; set; }
        private Image _tileset;
        
        private ushort[] _buffer = new ushort[DisplayState.Width * DisplayState.Height];

        public TerminalPanel() {
            InitializeComponent();
            Width = 384;
            Height = 288;
            LoadDefaultTileset();
        }

        public void BindTo(DisplayState displayState) {
            displayState.OnWrite += new EventHandler<DeviceWriteEventArgs>(HandleDisplayUpdate);
        }

        private void LoadDefaultTileset() {
            var assembly = typeof(Dcpu).Assembly;
            using (var stream = assembly.GetManifestResourceStream(typeof(Dcpu), "tileset.png")) {
                _tileset = new Bitmap(stream);
                // Work-around for GDI+ bug, triggered by call to Graphics.DrawImage
                // http://siderite.blogspot.com/2009/09/outofmemoryexception-in.html
                _tileset = _tileset.GetThumbnailImage(_tileset.Width, _tileset.Height, null, IntPtr.Zero);
            }
        }

        private void HandleKeyPress(object sender, KeyPressEventArgs args) {
            Dcpu.NewEvent(new KeyboardEvent(args.KeyChar));
            args.Handled = true;
        }

        private void HandleDisplayUpdate(object sender, DeviceWriteEventArgs args) {
            if (args.OldValue == args.NewValue)
                return;
            _buffer[args.Address - DisplayState.DisplayAddress] = args.NewValue;
            Invoke(new Action(() => InvalidateCharacterAt((ushort)(args.Address - DisplayState.DisplayAddress))));
        }

        private void InvalidateCharacterAt(ushort addr) {
            var row = addr / DisplayState.Width;
            var col = addr % DisplayState.Width;
            Invalidate(new Rectangle(col * 12, row * 24, 12, 24));
        }

        private void DoPaint(object sender, PaintEventArgs e) {
            var g = e.Graphics;

            var startCol = e.ClipRectangle.Left / 12;
            var startRow = e.ClipRectangle.Top / 24;
            var endCol = e.ClipRectangle.Right / 12 + 1;
            endCol = endCol > DisplayState.Width ? DisplayState.Width : endCol;
            var endRow = e.ClipRectangle.Bottom / 24 + 1;
            endRow = endRow > DisplayState.Height ? DisplayState.Height : endRow;

            for (int row=startRow; row < endRow; ++row) {
                for (int col=startCol; col < endCol; ++col) {
                    PaintTile(g, row, col, _buffer[row * DisplayState.Width + col]);
                }
            }
        }

        private void PaintTile(Graphics g, int row, int col, ushort word) {
            byte bg = (byte)((word >> 8) & 0x0F);
            byte fg = (byte)((word >> 12) & 0x0F);
            var fgColor = AsColor(fg);
            var bgColorMap = new ColorMap { OldColor = Color.FromArgb(0, 0, 0xAA), NewColor = AsColor(bg) };
            var fgColorMap = new ColorMap { OldColor = Color.FromArgb(0xFF, 0xFF, 0xFF), NewColor = AsColor(fg) };
            var imageAttrs = new ImageAttributes();
            imageAttrs.SetRemapTable(new [] { bgColorMap, fgColorMap });

            byte ch = (byte)(word & 0x007F);
            int tileX = ch % 16;
            int tileY = ch / 16;
            g.DrawImage(_tileset,
                new Rectangle(col * 4 * 3, row * 8 * 3, 12, 24),
                tileX * 4 * 3, tileY * 8 * 3, 12, 24,
                GraphicsUnit.Pixel,
                imageAttrs);
        }

        private static Color AsColor(byte val) {
            var r = (val & 0x4) != 0 ? 0x7F : 0;
            var g = (val & 0x2) != 0 ? 0x7F : 0;
            var b = (val & 0x1) != 0 ? 0x7F : 0;
            if ((val & 0x8) != 0) {
                r *= 2;
                g *= 2;
                b *= 2;
            }
            return Color.FromArgb(r, g, b);
        }
    }
}
