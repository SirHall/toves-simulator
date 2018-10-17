/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Sim.Inst {
    [Flags]
    public enum PortType {
        Passive = 0,
        Input = 1,
        Output = 2,
        InOut = 3
    }
}

