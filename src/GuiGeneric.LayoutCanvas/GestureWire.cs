/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Layout.Model;
using Toves.AbstractGui.Canvas;
using Toves.Sim.Inst;

namespace Toves.GuiGeneric.LayoutCanvas {
    public class GestureWire : IGesture {
        private LayoutCanvasModel layoutModel;
        private Location loc0;
        private Location loc1;
        private bool isModified = true;
        private bool horzFirst = true;

        public GestureWire(LayoutCanvasModel layoutModel, Location loc0) {
            this.layoutModel = layoutModel;
            this.loc0 = loc0;
            this.loc1 = loc0;
        }

        public void GestureStart(IPointerEvent evnt) {
        }

        public void GestureMove(IPointerEvent evnt) {
            Location pt = Constants.SnapToGrid(new Location(evnt.X, evnt.Y));
            bool mod = evnt.IsModified(GestureModifier.Alt);
            if (pt != loc1 || mod != isModified) {
                loc1 = pt;
                isModified = mod;
                if (loc0.X == loc1.X) {
                    horzFirst = false;
                } else if (loc0.Y == loc1.Y) {
                    horzFirst = true;
                }
                evnt.RepaintCanvas();
            }
        }

        private WireSegment[] GetSegments(Location loc0, Location loc1, bool isDiagonal) {
            if (isDiagonal) {
                return new WireSegment[] { new WireSegment(loc0, loc1) };
            } else {
                Location mid = horzFirst ? new Location(loc1.X, loc0.Y) : new Location(loc0.X, loc1.Y);
                return new WireSegment[] { new WireSegment(loc0, mid), new WireSegment(mid, loc1) };
            }
        }

        private static void AddWireSegment(ILayoutAccess lo, LayoutWiringPoints wiringData, WireSegment proposedSeg) {
            Location loc0 = proposedSeg.End0;
            Location loc1 = proposedSeg.End1;
            Dictionary<WireSegment, Location?> intersects = new Dictionary<WireSegment, Location?>();
            List<Location> breaks = new List<Location>();
            foreach (WireSegment w in wiringData.GetWiresContaining(loc0)) {
                intersects[w] = intersects.ContainsKey(w) ? (Location?) null : loc0;
            }
            foreach (WireSegment w in wiringData.GetWiresContaining(loc1)) {
                intersects[w] = intersects.ContainsKey(w) ? (Location?) null : loc1;
            }
            foreach (Location loc in proposedSeg.GetLocationsOnWire(Constants.GRID_SIZE, false)) {
                foreach (WireSegment w in wiringData.GetWiresEndingAt(loc)) {
                    intersects[w] = intersects.ContainsKey(w) ? (Location?) null : loc;
                }
                if (wiringData.IsPortAt(loc)) {
                    breaks.Add(loc);
                }
            }

            List<WireSegment> toRemove = new List<WireSegment>();
            List<WireSegment> toAdd = new List<WireSegment>();
            List<Location> endpoints = new List<Location>();
            endpoints.Add(loc0);
            endpoints.Add(loc1);
            foreach (WireSegment w in intersects.Keys) {
                Location? intersect = intersects[w];
                if (!intersect.HasValue) { // segment crosses both endpoints - delete subsegment
                    toRemove.Add(w);
                } else if (w.IsCollinear(proposedSeg, Constants.GRID_SIZE)) {
                    toRemove.Add(w);
                    endpoints.Add(w.End0);
                    endpoints.Add(w.End1);
                } else { // segment crosses over this one - maybe we should split it
                    Location at = intersect.Value;
                    breaks.Add(at);
                    if (at != w.End0 && at != w.End1) {
                        toRemove.Add(w);
                        toAdd.Add(new WireSegment(w.End0, at));
                        toAdd.Add(new WireSegment(at, w.End1));
                    }
                }
            }
            endpoints.Sort(Location.XComparer);
            breaks.Add(endpoints[0]);
            breaks.Add(endpoints[endpoints.Count - 1]);

            breaks.Sort(Location.XComparer);
            Location cur = breaks[0];
            foreach (Location loc in breaks) {
                if (cur != loc) {
                    toAdd.Add(new WireSegment(cur, loc));
                    cur = loc;
                }
            }

            foreach (WireSegment seg in toRemove) {
                lo.RemoveWire(seg);
            }
            foreach (WireSegment seg in toAdd) {
                lo.AddWire(seg.End0, seg.End1);
            }
        }

        public void GestureComplete(IPointerEvent evnt) {
            Location loc1 = Constants.SnapToGrid(new Location(evnt.X, evnt.Y));
            if (loc0 != loc1) {
                WireSegment[] proposedSegs = GetSegments(loc0, loc1, evnt.IsModified(GestureModifier.Alt));
                // Console.WriteLine("**** Adding Wire ****");
                layoutModel.Execute((ILayoutAccess lo) => {
                    foreach (WireSegment seg in proposedSegs) {
                        AddWireSegment(lo, layoutModel.WiringPoints, seg);
                        layoutModel.WiringPoints.Update();
                    }
                });
            }
            layoutModel.Gesture = null;
            evnt.RepaintCanvas();
        }

        public void GestureCancel(IPointerEvent evnt) {
        }

        public void Paint(IPaintbrush pb) {
            if (loc0 != loc1) {
                pb.Color = Constants.GetColorFor(Value.One);
                pb.StrokeWidth = Constants.WIRE_WIDTH;
                foreach (WireSegment seg in GetSegments(loc0, loc1, isModified)) {
                    pb.StrokeLine(seg.End0.X, seg.End0.Y, seg.End1.X, seg.End1.Y);
                }
            }
        }
    }
}