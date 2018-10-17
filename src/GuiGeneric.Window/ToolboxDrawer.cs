/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;

namespace Toves.GuiGeneric.Window {
    public class ToolboxDrawer : ToolboxItem {
        private string name;
        private IEnumerable<object> contents;

        public ToolboxDrawer(String name, IEnumerable<object> contents) {
            this.name = name;
            this.contents = contents;
        }

        public override string Name { get { return name; } }

        public IEnumerable<object> GetContents() {
            return contents;
        }
    }
}