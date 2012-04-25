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
    public partial class MainForm : Form
    {
        public static readonly int NumSavedDcpuStates = 200;
        public static readonly int DcpuSavedStateInterval = 100; // In milliseconds

        private readonly Dcpu _dcpu;

        private List<IState> _savedDcpuStates; 
        private readonly Timer _dcpuStateTimer;

        public MainForm(Dcpu dcpu, DisplayState initialDisplayState) {
            InitializeComponent();
            _dcpu = dcpu;

            _savedDcpuStates = new List<IState>(NumSavedDcpuStates);
            _dcpuStateTimer = new Timer();
            _dcpuStateTimer.Tick += HandleDcpuStateTimerTick;
            _dcpuStateTimer.Interval = DcpuSavedStateInterval;

            _stopStartBtn.Text = "Start";
            _terminalPanel.Dcpu = dcpu;
            _terminalPanel.BindTo(initialDisplayState);

            _timeTrackBar.Maximum = NumSavedDcpuStates - 1;
            _timeTrackBar.Value = NumSavedDcpuStates - 1;
        }

        private void HandleStartStopClick(object sender, EventArgs e) {
            if (_dcpu.IsRunning) {
                _dcpuStateTimer.Stop();
                _dcpu.Stop();
                _stopStartBtn.Text = "Start";
                _timeTrackBar.Enabled = true;
            } else {
                _dcpu.Start();
                _dcpuStateTimer.Start();
                _stopStartBtn.Text = "Stop";
                _timeTrackBar.Enabled = false;
                _timeTrackBar.Value = NumSavedDcpuStates - 1;
            }
        }

        private void HandleTimeTrackbarValueChanged(object sender, EventArgs e) {
            if (!_timeTrackBar.Enabled)
                return;
            int index = _timeTrackBar.Value - (NumSavedDcpuStates - _savedDcpuStates.Count);
            if (index > 0 && index < _savedDcpuStates.Count) {
                _dcpu.State = _savedDcpuStates[index];
                _terminalPanel.ResetBuffer(_dcpu.State.GetDeviceState("Display") as DisplayState); 
            }
        }

        private void HandleDcpuStateTimerTick(object sender, EventArgs e) {
            if (_savedDcpuStates.Count == NumSavedDcpuStates)
                _savedDcpuStates.RemoveAt(0);
            _savedDcpuStates.Add(_dcpu.State);
        }
    }
}
