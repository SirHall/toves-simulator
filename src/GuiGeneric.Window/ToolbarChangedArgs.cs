/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.GuiGeneric.Window {
    public class ToolbarChangedArgs : EventArgs {
        public ToolbarChangedArgs(int oldValue, int newValue) {
            OldValue = oldValue;
            Value = newValue;
        }

        public int OldValue { get; private set; }
        public int Value { get; private set; }
    }
}