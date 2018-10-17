/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.AbstractGui.Canvas {
    public interface ICanvas {
        ICanvasModel CanvasModel { get; set; }

        int RawWidth { get; }

        int RawHeight { get; }

        void RepaintCanvas();
    }
}