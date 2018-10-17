/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Sim.Inst;

namespace Toves.Layout.Comp {
    public class ConnectionPoint : PortArgs {
        public static ConnectionPoint newInput(int dx, int dy) {
            return new ConnectionPoint(dx, dy, PortType.Input);
        }
        
        public static ConnectionPoint newOutput(int dx, int dy) {
            return new ConnectionPoint(dx, dy, PortType.Output);
        }
        
        public static ConnectionPoint newPassive(int dx, int dy) {
            return new ConnectionPoint(dx, dy, PortType.Passive);
        }

        private ConnectionPoint(int dx, int dy, PortType type) : base(type, 1) {
            this.Dx = dx;
            this.Dy = dy;
        }

        public int Dx { get; private set; }

        public int Dy { get; private set; }
    }
}