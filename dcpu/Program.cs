using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.MattMcGill.Dcpu {
    class Program {

        static void Main(string[] args) {
            Trace.Listeners.Add(new ConsoleTraceListener());
            if (args.Length == 0) {
                PrintUsage();
                Environment.Exit(1);
            }

            IState state = MutableState.ReadFromFile(args[0]);
            while (true) {
                IState s1;
                var pc = state.Get(Register.PC);
                var op = Dcpu.FetchNextInstruction(state, out s1);
                Trace.WriteLine(string.Format("[0x{0:X}] {1}", pc, op));
                Console.Write("> ");
                var input = Console.ReadLine().Trim();
                if (input == "q") {
                    break;
                }
                state = op.Apply(s1);
            }
        }

        private static void PrintUsage() {
            Console.WriteLine("{0} <object file>", Environment.CommandLine[0]);
        }
    }
}
