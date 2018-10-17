/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Proj.Module {
    public class Connection {
        public Connection(bool isInput) {
            this.IsInput = isInput;
        }

        public bool IsInput { get; private set; }
    }
}

