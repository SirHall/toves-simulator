/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.AbstractGui.Canvas {
    public class GestureCompleteArgs : EventArgs {
        public GestureCompleteArgs(bool success) {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}