/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.AbstractGui.Canvas;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Layout.Model;
using Toves.Sim.Inst;
using Toves.Util.Transaction;

namespace Toves.GuiGeneric.LayoutCanvas {
    public class GestureTranslate : IGesture {
        private LayoutCanvasModel layoutCanvas;
        private ComponentInstance moving;
        private Location movingLocation;
        private int initX = 0;
        private int initY = 0;
        private int curDx = 0;
        private int curDy = 0;
        private bool curValid = true;

        public GestureTranslate(LayoutCanvasModel layoutCanvas,
                                ComponentInstance moving) {
            this.layoutCanvas = layoutCanvas;
            this.moving = moving;

            Transaction xn = new Transaction();
            ILayoutAccess layout = xn.RequestReadAccess(layoutCanvas.Layout);
            using (xn.Start()) {
                this.movingLocation = moving.Component.GetLocation(layout);
            }
        }

        private bool Update(IPointerEvent evnt) {
            if (moving == null) return false;
            int dx = evnt.X - initX;
            int dy = evnt.Y - initY;
            Location loc = movingLocation;
            if (moving.Component.ShouldSnap) {
                Location newLoc = loc.Translate(dx, dy);
                newLoc = Constants.SnapToGrid(newLoc);
                dx = newLoc.X - loc.X;
                dy = newLoc.Y - loc.Y;
            }

            if (dx == curDx && dy == curDy) {
                return false;
            } else {
                curDx = dx;
                curDy = dy;
                curValid = true;
                return true;
            }
        }

        public void GestureStart(IPointerEvent evnt) {
            if (moving != null) {
                initX = evnt.X;
                initY = evnt.Y;
                layoutCanvas.Hidden = new Component[] { moving.Component };
                Update(evnt);
                evnt.RepaintCanvas();
            }
        }

        public void GestureMove(IPointerEvent evnt) {
            if (Update(evnt)) {
                evnt.RepaintCanvas();
            }
        }

        public void GestureComplete(IPointerEvent evnt) {
            ComponentInstance toMove = moving;
            if (toMove != null) {
                moving = null;
                layoutCanvas.Execute((ILayoutAccess lo) => {
                    Component comp = toMove.Component;
                    List<Location> toCheck = new List<Location>();
                    foreach (ConnectionPoint conn in comp.Connections) {
                        toCheck.Add(movingLocation.Translate(conn.Dx, conn.Dy));
                    }

                    if (curValid) {
                        lo.MoveComponent(toMove.Component, curDx, curDy);
                        layoutCanvas.WiringPoints.Update();
                        WireTools.CheckForMerges(lo, layoutCanvas.WiringPoints, toCheck);
                        layoutCanvas.WiringPoints.Update();
                        WireTools.CheckForSplits(lo, layoutCanvas.WiringPoints, new Component[] { comp });
                    } else {
                        lo.RemoveComponent(toMove.Component);
                        layoutCanvas.WiringPoints.Update();
                        WireTools.CheckForMerges(lo, layoutCanvas.WiringPoints, toCheck);
                    }
                });
                layoutCanvas.Hidden = null;
                layoutCanvas.Gesture = null;
                evnt.RepaintCanvas();
            }
        }

        public void GestureCancel(IPointerEvent evnt) {
            moving = null;
            curValid = false;
            layoutCanvas.Hidden = null;
            evnt.RepaintCanvas();
        }

        public void Paint(IPaintbrush pb) {
            ComponentInstance cur = moving;
            if (cur != null) {
                if (curValid) {
                    pb.Color = Constants.COLOR_GHOST;
                } else {
                    pb.Color = Constants.COLOR_DEAD;
                }
                Location loc = movingLocation;
                pb.TranslateCoordinates(loc.X + curDx, loc.Y + curDy);
                cur.Component.Paint(new ComponentPainter(pb, new DummyInstanceState(cur)));
            }
        }
    }
}