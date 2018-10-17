/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

using Toves.AbstractGui.Canvas;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Layout.Model;
using Toves.Sim.Inst;
using Toves.Util.Transaction;

namespace Toves.GuiGeneric.LayoutCanvas {
    public class GestureNull : IGesture {
        private LayoutCanvasModel layoutCanvas;
        private Location? current;

        public GestureNull(LayoutCanvasModel layoutModel) {
            this.layoutCanvas = layoutModel;
            this.current = null;
        }

        public void GestureStart(IPointerEvent evnt) {
            GestureStart(evnt, true);
        }

        public void GestureStartWithoutPoke(IPointerEvent evnt) {
            GestureStart(evnt, false);
        }
        
        private void GestureStart(IPointerEvent evnt, bool considerPoke) {
            IGesture newGesture = GetGesture(evnt, considerPoke);
            if (newGesture != null) {
                if (current != null) {
                    current = null;
                    evnt.RepaintCanvas();
                }
                layoutCanvas.Gesture = newGesture;
                newGesture.GestureStart(evnt);
            }
        }

        private IGesture GetGesture(IPointerEvent evnt, bool considerPoke) {
            if (layoutCanvas.WiringPoints == null) {
                return null;
            }
            Location eLoc = new Location(evnt.X, evnt.Y);
            Location snapLoc = Constants.SnapToGrid(eLoc);
            if (layoutCanvas.WiringPoints.Contains(snapLoc)) {
                int d2 = eLoc.GetDistance2(snapLoc);
                if (d2 < 16 * 16) {
                    return new GestureWire(layoutCanvas, snapLoc);
                }
            }

            Instance found = null;
            Transaction xn = new Transaction();
            ILayoutAccess lo = xn.RequestReadAccess(layoutCanvas.Layout);
            using (xn.Start()) {
                foreach (Component component in lo.Components) {
                    Location iloc = component.GetLocation(lo);
                    if (component.Contains(eLoc.X - iloc.X, eLoc.Y - iloc.Y)) {
                        found = layoutCanvas.LayoutSim.GetInstance(lo, component);
                    }
                }
            }
            ComponentInstance foundComp = found as ComponentInstance;
            if (foundComp != null) {
                if (considerPoke && foundComp.Component is Pokeable) {
                    return new GesturePoke(layoutCanvas, foundComp);
                } else {
                    return new GestureTranslate(layoutCanvas, foundComp);
                }
            } else {
                return new GesturePan(layoutCanvas);
            }
        }


        public void GestureMove(IPointerEvent evnt) {
            if (layoutCanvas.WiringPoints != null) {
                Location eLoc = new Location(evnt.X, evnt.Y);
                Location snapLoc = Constants.SnapToGrid(eLoc);
                if (layoutCanvas.WiringPoints.Contains(snapLoc)) {
                    if (current != snapLoc) {
                        current = snapLoc;
                        evnt.RepaintCanvas();
                    }
                } else {
                    if (current != null) {
                        current = null;
                        evnt.RepaintCanvas();
                    }
                }
            }
        }

        public void GestureComplete(IPointerEvent evnt) {
            GestureMove(evnt);
        }

        public void GestureCancel(IPointerEvent evnt) {
            this.current = null;
        }

        public void Paint(IPaintbrush pb) {
            if (current != null) {
                pb.Color = Constants.GetColorFor(Value.One);
                pb.StrokeCircle(current.Value.X, current.Value.Y, Constants.WIRING_READY_RADIUS);
            }
        }
    }
}

