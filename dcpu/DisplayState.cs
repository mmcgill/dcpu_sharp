using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.MattMcGill.Dcpu {

    public class DisplayState : DeviceState {
        public static readonly int Width = 32;
        public static readonly int Height = 12;
        public static readonly int DisplayAddress = 0x8000;

        public override string Id { get { return "Display"; } }

        private PersistentArray<ushort> _displayMemory;

        public DisplayState() {
            _displayMemory = new PersistentArray<ushort>(Width * Height);
        }

        protected DisplayState(DisplayState prev) : base(prev) {
            _displayMemory = prev._displayMemory;
        }

        public override ushort Read(ushort addr) {
            return _displayMemory[addr];
        }

        public override DeviceState Write(ushort addr, ushort newValue) {
            var next = new DisplayState(this);
            var oldValue = _displayMemory[addr - DisplayAddress];
            next._displayMemory = next._displayMemory.Set(addr - DisplayAddress, newValue);
            TriggerOnWrite(addr, oldValue, newValue);
            return next;
        }
    }
}
