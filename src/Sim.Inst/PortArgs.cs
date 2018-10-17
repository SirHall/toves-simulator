/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Sim.Inst {
    public class PortArgs {
        public PortArgs(PortType type, int width) {
            this.Type = type;
            this.Width = width;
        }
        
        public PortType Type { get; private set; }
        
        public int Width { get; private set; }
    }
}

