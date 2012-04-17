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

        [STAThread]
        static void Main(string[] args) {
            Trace.Listeners.Add(new ConsoleTraceListener());
            if (args.Length == 0) {
                PrintUsage();
                Environment.Exit(1);
            }
            IState state = MutableState.ReadFromFile(args[0]);
            var display = new Display();
            state.MapMemory(0x8000, (ushort)(0x8000 + Display.WIDTH * Display.HEIGHT), display);

            Task.Factory.StartNew(() => CpuLoop(state, display));

            Application.EnableVisualStyles();
            Application.Run(display);
        }

        private static void CpuLoop(IState state, Display display) {
            bool debugging = true;
            while (true) {
                var pc = state.Get(Register.PC);

                if (debugging)
                    Trace.Write(string.Format("[0x{0:X}] ", pc));

                var sp = state.Get(Register.SP);
                var origSp = sp;
                var op = Dcpu.FetchNextInstruction(state, ref pc, ref sp);


                if (debugging) {
                    Trace.WriteLine(string.Format("{0}", op));

                    Console.Write("> ");
                    var input = Console.ReadLine().Trim();
                    if (input == "q") {
                        break;
                    } else if (input == "r") {
                        debugging = false;
                    }
                }
                state = state.Set(Register.PC, pc);
                if (origSp != sp)
                    state = state.Set(Register.SP, sp);
                state = op.Apply(state);
            }
            display.Invoke(new Action(display.Close));
        }

        private static void PrintUsage() {
            Console.WriteLine("Usage: dcpu <object file>", Environment.CommandLine[0]);
        }
    }
}
