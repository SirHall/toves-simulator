/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.AbstractGui.Canvas {
    public class GesturePan : IGesture {
        private AbstractCanvasModel canvasModel;
        private int initX; // coordinates of center when gesture starts
        private int initY;
        private double startX; // coordinates where gesture starts
        private double startY;

        public GesturePan(AbstractCanvasModel canvasModel) {
            this.canvasModel = canvasModel;
        }

        public void GestureStart(IPointerEvent evnt) {
            initX = evnt.Model.CenterX;
            initY = evnt.Model.CenterY;
            startX = evnt.RawX;
            startY = evnt.RawY;
        }

        public void GestureMove(IPointerEvent evnt) {
            double scale = 1.0 / evnt.Model.Zoom;
            evnt.Model.CenterX = initX - (int) (0.5 + (evnt.RawX - startX) * scale);
            evnt.Model.CenterY = initY - (int) (0.5 + (evnt.RawY - startY) * scale);
            evnt.RepaintCanvas();
        }

        public void GestureComplete(IPointerEvent evnt) {
            GestureMove(evnt);
            canvasModel.Gesture = null;
        }

        public void GestureCancel(IPointerEvent evnt) {
            // do nothing
        }

        public void Paint(IPaintbrush pb) {
            // do nothing
        }
    }
}