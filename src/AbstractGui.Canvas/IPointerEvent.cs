/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.AbstractGui.Canvas {
    public enum PointerEventType {
        GestureStart,
        GestureMove,
        GestureEnd,
        GestureCancel
    }

    public enum GestureModifier {
        None,
        Button1,
        Button2,
        Button3,
        Shift,
        Control,
        Alt
    }

    public interface IPointerEvent {
        PointerEventType Type { get; }

        int X { get; }

        int Y { get; }

        double RawX { get; }

        double RawY { get; }

        ICanvasModel Model { get; }

        bool IsModified(GestureModifier query);

        void RepaintCanvas();
    }
}