/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.AbstractGui.Canvas {
    public interface ICanvasModel : IDisposable {
        ICanvas Canvas { get; set; }
        
        double Zoom { get; set; }

        int CenterX { get; set; }

        int CenterY { get; set; }

        void HandlePointerEvent(IPointerEvent evnt);

        void HandleKeyPressEvent(IKeyEvent evnt);

        void RepaintCanvas();

        void Paint(IPaintbrush brush);
    }
}