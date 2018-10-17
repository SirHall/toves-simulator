/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Sim.Inst;
using Toves.Util.Transaction;

namespace Toves.Sim.Model {
    public interface ISimulationAccess : IResourceAccess {
        SimulationModel Simulation { get; }

        IEnumerable<Instance> Instances { get; }

        IEnumerable<Node> Nodes { get; }

        void AddLink(Link value);

        void RemoveLink(Link value);

        void AddInstance(Instance value);

        void RemoveInstance(Instance value);

        bool IsStepPending();

        void StepSimulation();

        Value Get(Node port);

        Value GetDriven(Port port);

        void Set(Port port, Value value, int delay);

        void MarkInstanceDirty(Instance value);
    }
}

