/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.AbstractGui.Canvas {
    public class DummyPointerEvent : IPointerEvent {
        public DummyPointerEvent(ICanvasModel model, PointerEventType type) {
            this.Model = model;
            this.Type = type;
        }

        public PointerEventType Type { get; private set; }

        public int X { get { return 0; } }

        public int Y { get { return 0; } }

        public double RawX { get { return 0; } }

        public double RawY { get { return 0; } }

        public ICanvasModel Model { get; private set; }

        public bool IsModified(GestureModifier query) { return false; }

        public void RepaintCanvas() {
            Model.RepaintCanvas();
        }
    }
}