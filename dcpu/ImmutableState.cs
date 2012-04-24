using System;
using System.Collections.Generic;

namespace Com.MattMcGill.Dcpu {

    public class ImmutableState : IState {

        private PersistentArray<ushort> _memory;
        private PersistentArray<ushort> _regs;
        // TODO: Replace with an ImmutableDictionary type.
        private Dictionary<string, DeviceState> _deviceStates;
		private MemoryMap _memoryMap;

        public ImmutableState() {
            _memory = new PersistentArray<ushort>(Dcpu.MAX_ADDRESS + 1);
            _regs = new PersistentArray<ushort>(11);
            _deviceStates = new Dictionary<string, DeviceState>();
            _memoryMap = new MemoryMap();
        }

        protected ImmutableState(ushort[] memory) {
            _memory = new PersistentArray<ushort>(memory);
            _regs = new PersistentArray<ushort>(11);
            _deviceStates = new Dictionary<string, DeviceState>();
            _memoryMap = new MemoryMap();
        }

        protected ImmutableState(ImmutableState prev, Register register, ushort value) {
            _memory = prev._memory;
            _regs = prev._regs.Set((int)register, value);
            _deviceStates = prev._deviceStates;
            _memoryMap = prev._memoryMap;
        }

        protected ImmutableState(ImmutableState prev, ushort addr, ushort value) {
            _memory = prev._memory;
            _regs = prev._regs;
            _memoryMap = prev._memoryMap;
            var deviceId = prev._memoryMap[addr];
            if (deviceId == null) {
                _memory = _memory.Set(addr, value);
                _deviceStates = prev._deviceStates;
            } else {
                _deviceStates = new Dictionary<string, DeviceState>(prev._deviceStates);
                _deviceStates[deviceId] = prev._deviceStates[deviceId].Write(addr, value);
            }
        }

        protected ImmutableState(ImmutableState prev, ushort from, ushort to, DeviceState device) {
            _memory = prev._memory;
            _regs = prev._regs;
            _memoryMap = new MemoryMap(prev._memoryMap);
            _memoryMap.Map(from, to, device.Id);
            _deviceStates = new Dictionary<string,DeviceState>(prev._deviceStates);
            _deviceStates[device.Id] = device;
        }

        protected ImmutableState(ImmutableState prev, IEvent e) {
            _memory = prev._memory;
            _regs = prev._regs;
            _memoryMap = prev._memoryMap;
            _deviceStates = new Dictionary<string, DeviceState>(prev._deviceStates);
            _deviceStates[e.DeviceId] = _deviceStates[e.DeviceId].Handle(e);
        }

        public ushort Get(Register reg) {
            return _regs[(int)reg];
        }

        public IState Set(Register reg, ushort value) {
            return new ImmutableState(this, reg, value);
        }

        public ushort Get(ushort addr) {
            var deviceId = _memoryMap[addr];
            if (deviceId != null) {
                return _deviceStates[deviceId].Read(addr);
            }
            return _memory[addr];
        }

        public IState Set(ushort addr, ushort value) {
            return new ImmutableState(this, addr, value);
        }

        public IState Map(ushort from, ushort to, DeviceState device) {
            return new ImmutableState(this, from, to, device);
        }

        public IState Handle(IEvent e) {
            if (_deviceStates.ContainsKey(e.DeviceId))
                return new ImmutableState(this, e);
            return this;
        }

        public static ImmutableState ReadFromFile(string path) {
            return new ImmutableState(Dcpu.LoadImage(path));
        }
    }
}

