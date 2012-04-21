using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Com.MattMcGill.Dcpu {

    /// <summary>
    /// Mutable DCPU state.
    /// </summary>
    public class MutableState : IState {
        private readonly ushort[] _memory;

        private readonly ushort[] _regs;

        private readonly List<MappedRange> _mappedRanges = new List<MappedRange>();

        public MutableState() {
            _memory = new ushort[0x10000];
            _regs = new ushort[11];
        }

        public ushort Get(Register reg) {
            return _regs[(int)reg];
        }

        public ushort Get(ushort addr) {
            foreach (var range in _mappedRanges) {
                if (range.Start <= addr && addr <= range.End) {
                    return range.Device.Read(addr);
                }
            }

            return _memory[addr];
        }

        public IState Set(Register reg, ushort value) {
            Trace.WriteLine(string.Format("  {0} <- {1}", reg, value));
            _regs[(int)reg] = value;
            return this;
        }

        public IState Set(ushort addr, ushort value) {
            foreach (var range in _mappedRanges) {
                if (range.Start <= addr && addr <= range.End) {
                    range.Device.Write(addr, value);
                    return this;
                }
            }

            Trace.WriteLine(string.Format("  [0x{0:X}] <- {1}", addr, value));
            _memory[addr] = value;

            return this;
        }

        public static MutableState ReadFromFile(string path) {
            var state = new MutableState();
            using (var objFileReader = new BinaryReader(new FileStream(path, FileMode.Open))) {
                ushort i = 0;
                try {
                    while (true) {
                        var msb = objFileReader.ReadByte();
                        var lsb = objFileReader.ReadByte();
                        state._memory[i++] = (ushort)((msb << 8) ^ lsb);
                    }
                } catch (EndOfStreamException) {}
            }
            return state;
        }

        public void MapMemory(ushort from, ushort to, IDevice device) {
            int i = 0;
            while (i < _mappedRanges.Count && _mappedRanges[i].End < from) ++i;

            if (i == _mappedRanges.Count) {
                _mappedRanges.Add(new MappedRange(from, to, device));
            } else if (_mappedRanges[i].Start <= to) {
                var msg = string.Format("Cannot map [{0}...{1}] to {2} -- [{3}...{4}] is already mapped to {5}!",
                    from, to, _mappedRanges[i].Start, _mappedRanges[i].End, _mappedRanges[i].Device);
                throw new ArgumentException(msg);
            } else {
                _mappedRanges.Insert(i, new MappedRange(from, to, device));
            }
        }
    }

    public class MappedRange {
        public ushort Start { get; private set; }
        public ushort End { get; private set; }
        public IDevice Device { get; private set; }
        public MappedRange(ushort start, ushort end, IDevice device) {
            Start = start;
            End = end;
            Device = device;
        }
    }

}
