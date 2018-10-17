/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;

namespace Toves.Sim.Inst {
    public abstract class Instance {
        private static readonly Port[] EmptyPorts = new Port[0];

        private Port[] portArray = EmptyPorts;

        public Instance(IEnumerable<PortArgs> ports) {
            ComputePorts(ports);
        }

        public IEnumerable<Port> Ports { get; private set; }

        protected void UpdatePorts(IEnumerable<PortArgs> portData) {
            ComputePorts(portData);
            HandleEvent(new InstanceEvent(InstanceEvent.Types.InstancePortsChanged),
                  new DummyInstanceState(this));
        }

        private void ComputePorts(IEnumerable<PortArgs> portData) {
            Port[] oldPorts = portArray;
            Dictionary<PortArgs, Port> oldPortMap = new Dictionary<PortArgs, Port>();
            foreach (Port oldPort in oldPorts) {
                oldPortMap[oldPort.PortArgs] = oldPort;
            }
            List<Port> newPorts = new List<Port>();
            foreach (PortArgs arg in portData) {
                Port newPort;
                if (oldPortMap.TryGetValue(arg, out newPort)) {
                    oldPortMap.Remove(arg);
                } else {
                    newPort = new Port(this, arg);
                }
                newPorts.Add(newPort);
            }
            portArray = newPorts.ToArray();
            this.Ports = Array.AsReadOnly(portArray);
        }

        public Port GetPort(int index) {
            Port[] ports = portArray;
            if (index >= 0 && index < ports.Length) {
                return portArray[index];
            } else {
                throw new IndexOutOfRangeException(string.Format("requested {0}, maximum port index is {1}",
                                                                 index, ports.Length));
            }
        }

        public abstract void HandleEvent(InstanceEvent evnt, IInstanceState state);
    }
}

