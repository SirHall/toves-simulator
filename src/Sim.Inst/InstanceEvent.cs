/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Sim.Inst {
    public class InstanceEvent {
        public enum Types {
            InstanceAdded = 1,
            InstanceRemoved = 2,
            InstanceDirty = 4, // should be repropagated
            InstancePortsChanged = 8,
            SimulationTicked = 16
        }

        public InstanceEvent(Types type) {
            this.Type = type;
        }

        public Types Type { get; private set; }
    }
}

