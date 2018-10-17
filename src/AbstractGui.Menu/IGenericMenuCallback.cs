/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.AbstractGui.Menu {
    public interface IGenericMenuCallback {
        void Notify(string title, string message);
        string RequestString(string title, string prompt);
    }
}

