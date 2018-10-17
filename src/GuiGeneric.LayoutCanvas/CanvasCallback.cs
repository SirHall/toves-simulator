/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Layout.Model;
using Toves.Proj.Module;

namespace Toves.GuiGeneric.LayoutCanvas {
    public interface CanvasCallback {
        void SetView(ProjectModule module, LayoutSimulation sim);
    }
}

