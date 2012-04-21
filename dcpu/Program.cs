﻿using System;
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
            //Trace.Listeners.Add(new ConsoleTraceListener());
            if (args.Length == 0) {
                PrintUsage();
                Environment.Exit(1);
            }

            IState state = MutableState.ReadFromFile(args[0]);

            var terminal = new Terminal();
            terminal.Map(state);

            Task.Factory.StartNew(() => CpuLoop(state, terminal));

            Application.EnableVisualStyles();
            Application.Run(terminal);
        }

        private static void CpuLoop(IState state, Terminal terminal) {
            bool debugging = false;
            while (true) {
                var pc = state.Get(Register.PC);

                if (debugging)
                    Trace.Write(string.Format("[0x{0:X}] ", pc));

                var sp = state.Get(Register.SP);
                var origSp = sp;
                var op = Dcpu.FetchNextInstruction(state, ref pc, ref sp);

                if (IsHang(op)) {
                    Console.WriteLine("Hit infinite loop, halting.");
                    break;
                }

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
            terminal.Invoke(new Action(terminal.Close));
        }

        private static bool IsHang(Op op) {
            var set = op as Sub;
            if (set == null) return false;

            var a = set.A as Reg;
            if (a == null) return false;

            var b = set.B as Literal;
            if (b == null) return false;

            return a.Register == Register.PC && b.Value == 1;
        }

        private static void PrintUsage() {
            Console.WriteLine("Usage: dcpu <object file>", Environment.CommandLine[0]);
        }
    }
}
