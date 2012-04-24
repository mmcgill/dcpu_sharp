using System;
namespace Com.MattMcGill.Dcpu {
    public class DeviceWriteEventArgs : EventArgs {

        public ushort Address { get; private set; }

        public ushort OldValue { get; private set; }

        public ushort NewValue { get; private set; }

        public DeviceWriteEventArgs(ushort addr, ushort oldValue, ushort newValue) {
            Address = addr;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public abstract class DeviceState {
        public abstract string Id { get; }

		public abstract ushort Read(ushort addr);

		public abstract DeviceState Write(ushort addr, ushort value);

        public virtual DeviceState Handle(IEvent e) {
            return this;
        }

        public event EventHandler<DeviceWriteEventArgs> OnWrite;

        protected DeviceState() { }

        protected DeviceState(DeviceState previous) {
            OnWrite = previous.OnWrite;
        }

        protected void TriggerOnWrite(ushort addr, ushort oldValue, ushort newValue) {
            if (OnWrite != null) {
                OnWrite(this, new DeviceWriteEventArgs(addr, oldValue, newValue));
            }
        }
	}
}

