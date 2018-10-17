/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Layout.Model
{
    public class LayoutModifiedArgs : EventArgs
    {
        private ILayoutAccess access;

        public LayoutModifiedArgs(ILayoutAccess access)
        {
            this.access = access;
        }

        public ILayoutAccess Layout { get { return access; } }
    }
}

