/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.AbstractGui.Menu {
    public abstract class GenericMenuElement {
        public GenericMenuElement(String title) {
            this.Title = title;
        }

        public virtual String Title { get; private set; }
    }
}

