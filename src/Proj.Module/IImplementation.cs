/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Sim.Inst;

namespace Toves.Proj.Module {
    public interface IImplementation {
        void UpdateModule(ProjectModule value);
        IEnumerable<Connection> Connections { get; }
        IEnumerable<ModuleComponent> GetModuleComponents();
        Instance CreateInstance(ModuleComponent component);
        void Propagate(IInstanceState state);
    }
}