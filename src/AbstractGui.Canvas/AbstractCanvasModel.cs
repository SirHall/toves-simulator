/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.AbstractGui.Canvas {
    internal class PointerEventAdapter : IPointerEvent {
        private IPointerEvent master;

        internal PointerEventAdapter(IPointerEvent master, int x, int y) {
            this.master = master;
            this.X = x;
            this.Y = y;
        }

        public PointerEventType Type { get { return master.Type; } }

        public int X { get; private set; }

        public int Y { get; private set; }

        public double RawX { get { return master.RawX; } }

        public double RawY { get { return master.RawY; } }

        public ICanvasModel Model { get { return master.Model; } }

        public bool IsModified(GestureModifier query) {
            return master.IsModified(query);
        }

        public void RepaintCanvas() {
            master.RepaintCanvas();
        }
    }

    public abstract class AbstractCanvasModel : ICanvasModel {
        private IGesture nullGesture = null;
        private IGesture gesture = null;
        private IPointerEvent cancelEvent = null;

        public AbstractCanvasModel() {
            Zoom = 0.25;
            CenterX = 5000; // coordinates after scaling
            CenterY = 5000;
            cancelEvent = new DummyPointerEvent(this,
                PointerEventType.GestureCancel);
        }

        public ICanvas Canvas { get; set; }
        
        public double Zoom { get; set; }

        public int CenterX { get; set; }

        public int CenterY { get; set; }

        public IGesture Gesture {
            get { return gesture; }
            set {
                IGesture oldValue = gesture;
                IGesture newValue = value ?? nullGesture;
                if (oldValue != newValue) {
                    gesture = newValue;
                    if (oldValue != null) {
                        oldValue.GestureCancel(cancelEvent);
                    }
                }
            }
        }

        public IGesture NullGesture {
            set {
                nullGesture = value;
                if (gesture == null) {
                    gesture = value;
                }
            }
        }
        
        public abstract void Dispose();

        public virtual void HandlePointerEvent(IPointerEvent evnt) {
            double scale = 1.0 / Zoom;
            IPointerEvent subEvnt = new PointerEventAdapter(evnt,
                (int) (0.5 + (evnt.RawX - Canvas.RawWidth / 2.0) * scale + CenterX),
               (int) (0.5 + (evnt.RawY - Canvas.RawHeight / 2.0) * scale + CenterY));

            IGesture gest = gesture;
            if (gest != null) {
                switch (evnt.Type) {
                case PointerEventType.GestureStart:
                    gest.GestureStart(subEvnt);
                    break;
                case PointerEventType.GestureMove:
                    gest.GestureMove(subEvnt);
                    break;
                case PointerEventType.GestureEnd:
                    gest.GestureComplete(subEvnt);
                    break;
                }
            }
        }

        public virtual void HandleKeyPressEvent(IKeyEvent evnt) { }

        protected abstract void PaintModel(IPaintbrush pb);

        public void RepaintCanvas() {
            ICanvas canvas = this.Canvas;
            if (canvas != null) {
                canvas.RepaintCanvas();
            }
        }

        public void Paint(IPaintbrush pb) {
            pb.TranslateCoordinates(Canvas.RawWidth / 2, Canvas.RawHeight / 2);
            pb.ScaleCoordinates(Zoom, Zoom);
            pb.TranslateCoordinates(-CenterX, -CenterY);

            using (IPaintbrush pbSub = pb.Create()) {
                PaintModel(pbSub);
            }
            IGesture gest = gesture;
            if (gest != null) {
                using (IPaintbrush pbSub = pb.Create()) {
                    gest.Paint(pbSub);
                }
            }
        }
    }
}