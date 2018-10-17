/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;

namespace Toves.GuiGeneric.Window {
    public class ToolboxChangedArgs : EventArgs {
        public enum ChangeTypes {
            ItemAdded,
            ItemRemoved,
            ItemRenamed,
            ItemUnselected,
        }

        public ToolboxChangedArgs(ChangeTypes changeType, ToolboxItem changedItem) {
            this.ChangeType = changeType;
            this.ChangedItem = changedItem;
        }

        public ChangeTypes ChangeType { get; private set; }

        public ToolboxItem ChangedItem { get; private set; }
    }
}

