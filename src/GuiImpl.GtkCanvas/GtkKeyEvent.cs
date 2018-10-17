/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

using Toves.AbstractGui.Canvas;

namespace Toves.GuiImpl.GtkCanvas {
    public class GtkKeyEvent : IKeyEvent {
        public GtkKeyEvent(GtkCanvas canvas, uint keyChar, KeyboardCode keyCode) {
            this.Canvas = canvas;
            this.KeyChar = keyChar;
            this.KeyCode = keyCode;
        }

        public ICanvas Canvas { get; private set; }

        public uint KeyChar { get; private set; }

        public KeyboardCode KeyCode { get; private set; }

        public bool IsModified(GestureModifier query) {
            return false;
        }

        public void RepaintCanvas() {
            Canvas.RepaintCanvas();
        }
    }
}