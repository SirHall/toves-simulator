/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.AbstractGui.Canvas {
    public interface IKeyEvent {
        ICanvas Canvas { get; }

        uint KeyChar { get; }

        KeyboardCode KeyCode { get; }

        bool IsModified(GestureModifier query);

        void RepaintCanvas();
    }
}