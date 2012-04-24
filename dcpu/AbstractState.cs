using System;
using System.IO;
using System.Collections.Generic;

namespace Com.MattMcGill.Dcpu {
    public abstract class AbstractState {

        private readonly List<MappedRange> _mappedRanges = new List<MappedRange>();

        public AbstractState() {
        }

        protected AbstractState(AbstractState previous) {
            _mappedRanges.AddRange(previous._mappedRanges);
        }

        public ushort Get(ushort addr) {
            foreach (var range in _mappedRanges) {
                if (range.Start <= addr && addr <= range.End) {
                    return range.Device.Read(addr);
                }
            }

            return GetInternal(addr);
        }

        protected abstract ushort GetInternal(ushort addr);

        protected void WriteToDevice(ushort addr, ushort value) {
            foreach (var range in _mappedRanges) {
                if (range.Start <= addr && addr <= range.End) {
                    range.Device.Write(addr, value);
                }
            }
        }

        protected static ushort[] LoadImage(string path) {
            ushort[] image = new ushort[Dcpu.MAX_ADDRESS + 1];
            using (var objFileReader = new BinaryReader(new FileStream(path, FileMode.Open))) {
                ushort i = 0;
                try {
                    while (true) {
                        var msb = objFileReader.ReadByte();
                        var lsb = objFileReader.ReadByte();
                        image[i++] = (ushort)((msb << 8) ^ lsb);
                    }
                } catch (EndOfStreamException) { }
            }
            return image;
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

