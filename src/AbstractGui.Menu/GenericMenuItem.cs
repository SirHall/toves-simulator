/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.AbstractGui.Menu {
    public class GenericMenuItem : GenericMenuElement {
        private Action<IGenericMenuCallback> action;

        public GenericMenuItem(string title, Action<IGenericMenuCallback> action) : base(title) {
            this.action = action;
        }

        public void Execute(IGenericMenuCallback callback) {
            action(callback);
        }
    }
}

