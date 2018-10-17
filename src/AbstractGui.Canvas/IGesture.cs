/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.AbstractGui.Canvas {
    public interface IGesture {
        void GestureStart(IPointerEvent evnt);

        void GestureMove(IPointerEvent evnt);

        void GestureComplete(IPointerEvent evnt);

        void GestureCancel(IPointerEvent evnt);

        void Paint(IPaintbrush pb);
    }
}