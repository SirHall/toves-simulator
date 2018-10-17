/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;

namespace Toves.AbstractGui.Menu {
    public class GenericMenu : GenericMenuElement {
        private List<GenericMenuElement> elements = new List<GenericMenuElement>();

        public GenericMenu(string title) : base(title) {
        }

        protected void AddItem(GenericMenuElement toAdd) {
            elements.Add(toAdd);
        }

        public IEnumerable<GenericMenuElement> Elements {
            get { return elements; }
        }
    }
}

