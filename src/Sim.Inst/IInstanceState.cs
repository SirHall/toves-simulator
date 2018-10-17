/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Sim.Model;

namespace Toves.Sim.Inst {
    public interface IInstanceState {
        Instance Instance { get; }

        Value Get(int index);

        Value GetDriven(int index);

        void Set(int index, Value value, int delay);
    }
}

