using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.MattMcGill.Dcpu {

    public class KeyboardState : DeviceState {

        public static readonly string DeviceId = "Keyboard";

        public static readonly int BufferLength = 0x10;

        public static readonly int BufferAddress = 0x9000;

        public override string Id { get { return DeviceId; } }

        private readonly PersistentArray<ushort> _ringBuffer;

        public KeyboardState() {
            _ringBuffer = new PersistentArray<ushort>(BufferLength + 1);
        }

        protected KeyboardState(KeyboardState prev, PersistentArray<ushort> ringBuffer) : base(prev) {
            _ringBuffer = ringBuffer;
        }

        public override ushort Read(ushort addr) {
            return _ringBuffer[addr - BufferAddress];
        }

        public override DeviceState Write(ushort addr, ushort value) {
            return new KeyboardState(this, _ringBuffer.Set(addr - BufferAddress, value));
        }

        public override DeviceState Handle(IEvent e) {
            var keyEvent = e as KeyboardEvent;
            if (keyEvent == null)
                return this;

            var cursor = _ringBuffer[BufferLength];
            var ringBuffer = _ringBuffer.Set(cursor, keyEvent.KeyChar).Set(BufferLength, (ushort)((cursor + 1) % BufferLength));
            return new KeyboardState(this, ringBuffer);
        }
    }

    public class KeyboardEvent : IEvent {

        public string DeviceId { get { return KeyboardState.DeviceId; } }

        public ushort KeyChar { get; private set; }

        public KeyboardEvent(ushort keyChar) {
            KeyChar = keyChar;
        }
    }
}
