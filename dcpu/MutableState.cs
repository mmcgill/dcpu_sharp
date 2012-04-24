using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Com.MattMcGill.Dcpu {

    /// <summary>
    /// Mutable DCPU state.
    /// </summary>
    public class MutableState : IState {
        private readonly ushort[] _memory;

        private readonly ushort[] _regs;

        private readonly MemoryMap _memoryMap;

        private readonly Dictionary<string, DeviceState> _deviceStates;

        public MutableState() {
            _memory = new ushort[Dcpu.MAX_ADDRESS + 1];
            _regs = new ushort[11];
            _memoryMap = new MemoryMap();
            _deviceStates = new Dictionary<string, DeviceState>();
        }

        public ushort Get(Register reg) {
            return _regs[(int)reg];
        }

        public ushort Get(ushort addr) {
            var deviceId = _memoryMap[addr];
            if (deviceId != null) {
                return _deviceStates[deviceId].Read(addr);
            }
            return _memory[addr];
        }

        public IState Set(Register reg, ushort value) {
            Trace.WriteLine(string.Format("  {0} <- {1}", reg, value));
            _regs[(int)reg] = value;
            return this;
        }

        public IState Set(ushort addr, ushort value) {
            var deviceId = _memoryMap[addr];
            if (deviceId != null) {
                _deviceStates[deviceId] = _deviceStates[deviceId].Write(addr, value);
            } else {
                _memory[addr] = value;
            }
            return this;
        }

        public IState Map(ushort from, ushort to, DeviceState device) {
            _memoryMap.Map(from, to, device.Id);
            _deviceStates[device.Id] = device;
            return this;
        }

        public IState Handle(IEvent e) {
            if (_deviceStates.ContainsKey(e.DeviceId)) {
                _deviceStates[e.DeviceId] = _deviceStates[e.DeviceId].Handle(e);
            }
            return this;
        }

        public static MutableState ReadFromFile(string path) {
            var state = new MutableState();
            var image = Dcpu.LoadImage(path);
            Array.Copy (image, state._memory, Dcpu.MAX_ADDRESS + 1);
            return state;
        }
    }
}
