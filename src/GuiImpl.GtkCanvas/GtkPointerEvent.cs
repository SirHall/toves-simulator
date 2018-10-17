/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Gdk;
using Toves.AbstractGui.Canvas;

namespace Toves.GuiImpl.GtkCanvas {
    public class GtkPointerEvent : IPointerEvent {
        private GtkCanvas canvas;
        private ModifierType mods;

        public GtkPointerEvent(GtkCanvas canvas) {
            this.canvas = canvas;
        }

        public PointerEventType Type { get; private set; }

        public double RawX { get; private set; }

        public double RawY { get; private set; }

        public int X { get { return (int) RawX; } }

        public int Y { get { return (int) RawY; } }

        public bool IsModified(GestureModifier query) {
            ModifierType m;
            switch (query) {
            case GestureModifier.Button1:
                m = ModifierType.Button1Mask;
                break;
            case GestureModifier.Button2:
                m = ModifierType.Button2Mask;
                break;
            case GestureModifier.Button3:
                m = ModifierType.Button3Mask;
                break;
            case GestureModifier.Shift:
                m = ModifierType.ShiftMask;
                break;
            case GestureModifier.Control:
                m = ModifierType.ControlMask;
                break;
            case GestureModifier.Alt:
                m = ModifierType.Mod1Mask;
                break;
            default:
                return false;
            }
            return (mods & m) != 0;
        }

        public ICanvasModel Model { get { return canvas.CanvasModel; } }

        public bool Update(PointerEventType type,
                           double rx, double ry, ModifierType state) {
            if (type == Type && rx == RawX && ry == RawY && mods == state) {
                return false;
            } else {
                Type = type;
                RawX = rx;
                RawY = ry;
                mods = state;
                return true;
            }
        }

        public void RepaintCanvas()
        {
            canvas.QueueDraw();
        }
    }
}