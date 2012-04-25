using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Com.MattMcGill.Dcpu {
    public partial class MainForm : Form {

        private Dcpu _dcpu;

        public MainForm(Dcpu dcpu, DisplayState initialDisplayState) {
            InitializeComponent();
            _dcpu = dcpu;
            _terminalPanel.Dcpu = dcpu;
            _terminalPanel.BindTo(initialDisplayState);
        }
    }
}
