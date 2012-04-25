using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Com.MattMcGill.Dcpu {
    class Program {
        private static Dcpu _cpu;

        [STAThread]
        static void Main(string[] args) {
            if (args.Length == 0) {
                PrintUsage();
                Environment.Exit(1);
            }

            var keyboardState = new KeyboardState();
            var displayState = new DisplayState();
            IState state = ImmutableState.ReadFromFile(args[0]);
            state = state.Map((ushort)KeyboardState.BufferAddress,
                (ushort)(KeyboardState.BufferAddress + KeyboardState.BufferLength + 1),
                keyboardState);
            state = state.Map((ushort)DisplayState.DisplayAddress,
                (ushort)(DisplayState.DisplayAddress + DisplayState.Width * DisplayState.Height),
                displayState);
            _cpu = new Dcpu(state);

            var mainForm = new MainForm(_cpu, displayState);

            mainForm.FormClosed += new FormClosedEventHandler( (obj, arg) => _cpu.Stop());

            Application.EnableVisualStyles();
            Application.Run(mainForm);
        }

        private static void PrintUsage() {
            Console.WriteLine("Usage: dcpu <object file>", Environment.CommandLine[0]);
        }
    }
}
