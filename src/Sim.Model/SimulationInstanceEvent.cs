/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Sim.Inst;

namespace Toves.Sim.Model {
    public class SimulationInstanceEvent : InstanceEvent {
        public SimulationInstanceEvent(InstanceEvent.Types type, SimulationModel simModel) : base(type) {
            this.SimulationModel = simModel;
        }

        public SimulationModel SimulationModel { get; private set; }
    }
}

