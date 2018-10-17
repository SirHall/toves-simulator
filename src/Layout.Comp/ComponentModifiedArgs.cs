/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Layout.Comp {
    public class ComponentModifiedArgs : EventArgs {
        public enum Types {
            PortsChanged
        }

        public ComponentModifiedArgs(Types type) {
            this.Type = type;
        }

        public Types Type { get; private set; }
    }
}

