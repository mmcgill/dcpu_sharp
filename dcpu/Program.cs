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

            IState state = MutableState.ReadFromFile(args[0]);

            var terminal = new Terminal();
            terminal.Map(state);

            _cpu = new Dcpu(state);
            terminal.FormClosed += new FormClosedEventHandler( (obj, arg) => _cpu.Stop());
            _cpu.Start();

            Application.EnableVisualStyles();
            Application.Run(terminal);
        }

        private static void PrintUsage() {
            Console.WriteLine("Usage: dcpu <object file>", Environment.CommandLine[0]);
        }
    }
}
