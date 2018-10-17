/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Gdk;
using Gtk;
using System.Collections.Generic;
using Cairo;
using Toves.AbstractGui.Canvas;

namespace Toves.GuiImpl.GtkCanvas {
    public class GtkCanvas : DrawingArea, ICanvas {
        private ICanvasModel canvasModel;
        private GtkPointerEvent pointerEvent;

        public GtkCanvas() {
            this.canvasModel = null;
            this.pointerEvent = new GtkPointerEvent(this);

            this.ModifyBg(StateType.Normal, new Gdk.Color(0xFF, 0xFF, 0xFF));

            this.AddEvents((int) EventMask.ButtonPressMask);
            this.AddEvents((int) EventMask.PointerMotionMask);
            this.AddEvents((int) EventMask.ButtonReleaseMask);
            this.AddEvents((int) EventMask.KeyPressMask);

            this.CanFocus = true;
            this.CanDefault = true;
            this.GrabFocus();
        }

        public ICanvasModel CanvasModel {
            get { return canvasModel; }
            set {
                ICanvasModel oldModel = canvasModel;
                if (oldModel != value) {
                    canvasModel = value;
                    if (oldModel != null) {
                        oldModel.Canvas = null;
                        oldModel.Dispose();
                    }
                    if (value != null) {
                        value.Canvas = this;
                    }
                    this.QueueDraw();
                }
            }
        }

        public int RawWidth {
            get { return (int) (0.5 + this.Allocation.Width); }
        }

        public int RawHeight {
            get { return (int) (0.5 + this.Allocation.Width); }
        }

        public void RepaintCanvas() {
            this.QueueDraw();
        }

        protected override bool OnButtonPressEvent(EventButton evnt) {
            DispatchPointerEvent(PointerEventType.GestureStart,
                                    evnt.X, evnt.Y, evnt.State);
            base.OnButtonPressEvent(evnt);
            return true;
        }

        protected override bool OnMotionNotifyEvent(EventMotion evnt) {
            DispatchPointerEvent(PointerEventType.GestureMove,
                                        evnt.X, evnt.Y, evnt.State);
            base.OnMotionNotifyEvent(evnt);
            return true;
        }

        protected override bool OnButtonReleaseEvent(EventButton evnt) {
            DispatchPointerEvent(PointerEventType.GestureMove,
                                 evnt.X, evnt.Y, evnt.State);
            DispatchPointerEvent(PointerEventType.GestureEnd,
                                        evnt.X, evnt.Y, evnt.State);
            base.OnButtonReleaseEvent(evnt);
            return true;
        }

        private void DispatchPointerEvent(PointerEventType type, double ex,
                                          double ey, ModifierType mods) {
            GtkPointerEvent outEvent = pointerEvent;
            if (outEvent.Update(type, ex, ey, mods)) {
                CanvasModel.HandlePointerEvent(outEvent);
            }
        }

        protected override bool OnKeyPressEvent(EventKey evnt) {
            KeyboardCode code;
            switch (evnt.Key) {
            case Gdk.Key.Up: code = KeyboardCode.ArrowUp; break;
            case Gdk.Key.Down: code = KeyboardCode.ArrowDown; break;
            case Gdk.Key.Left: code = KeyboardCode.ArrowLeft; break;
            case Gdk.Key.Right: code = KeyboardCode.ArrowRight; break;
            case Gdk.Key.BackSpace: code = KeyboardCode.Backspace; break;
            default:
                if (evnt.KeyValue >= 32 && evnt.KeyValue < 127) {
                    code = KeyboardCode.NormalChar;
                } else {
                    code = KeyboardCode.Unknown;
                }
                break;
            }
            IKeyEvent keyEvent = new GtkKeyEvent(this, evnt.KeyValue, code);
            CanvasModel.HandleKeyPressEvent(keyEvent);
            base.OnKeyPressEvent(evnt);
            return true;
        }

        protected override bool OnExposeEvent(EventExpose args) {
            base.OnExposeEvent(args);
            Context context = CairoHelper.Create(args.Window);

            IPaintbrush pb = new GtkPaintbrush(this, context);
            CanvasModel.Paint(pb);
            (context.Target as IDisposable).Dispose();                               
            (context as IDisposable).Dispose();
            return true;
        }
    }
}