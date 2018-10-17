/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Sim.Inst;

namespace Toves.Sim.Model {
    public class InstanceState : IInstanceState {
        private ISimulationAccess access;

        public InstanceState(ISimulationAccess access, Instance instance) {
            this.access = access;
            this.Instance = instance;
        }

        public Instance Instance { get; set; }

        public SimulationModel Simulation { get { return access.Simulation; } }

        public Value Get(int index) {
            return access.Get(Instance.GetPort(index));
        }

        public Value GetDriven(int index) {
            return access.GetDriven(Instance.GetPort(index));
        }

        public void Set(int index, Value value, int delay) {
            access.Set(Instance.GetPort(index), value, delay);
        }
    }
}