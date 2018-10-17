/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Layout.Model;
using Toves.Util.Collections;

namespace Toves.GuiGeneric.LayoutCanvas
{
    public static class WireTools
    {
        public static void CheckForMerges(ILayoutAccess lo, LayoutWiringPoints wireData, IEnumerable<Location> toCheck) {
            HashSet<Location> done = new HashSet<Location>();
            HashSet<WireSegment> toMerge = new HashSet<WireSegment>();
            List<WireSegment> w0s = new List<WireSegment>();
            List<WireSegment> w1s = new List<WireSegment>();
            foreach (Location loc in toCheck) {
                if (!done.Contains(loc) && !wireData.IsPortAt(loc)) {
                    done.Add(loc);
                    WireSegment w0 = null;
                    WireSegment w1 = null;
                    bool moreFound = false;
                    foreach (WireSegment w in wireData.GetWiresEndingAt(loc)) {
                        if (w0 == null) {
                            w0 = w;
                        } else if (w1 == null) {
                            w1 = w;
                        } else {
                            moreFound = true;
                            break;
                        }
                    }
                    if (!moreFound && w1 != null && w0.IsCollinear(w1, Constants.GRID_SIZE)) {
                        w0s.Add(w0);
                        w1s.Add(w1);
                        toMerge.Add(w0);
                        toMerge.Add(w1);
                    }
                }
            }

            UnionFind<WireSegment> uf = new UnionFind<WireSegment>(toMerge);
            for (int i = 0; i < w0s.Count; i++) {
                uf[w0s[i]].Unite(uf[w1s[i]]);
            }
            foreach (UnionFindNode<WireSegment> root in uf.Roots) {
                List<Location> locs = new List<Location>();
                foreach (UnionFindNode<WireSegment> member in root.GetSetMembers()) {
                    WireSegment w = member.Value;
                    lo.RemoveWire(w);
                    locs.Add(w.End0);
                    locs.Add(w.End1);
                }
                locs.Sort(Location.XComparer);
                lo.AddWire(locs[0], locs[locs.Count - 1]);
            }
        }

        public static void CheckForSplits(ILayoutAccess lo, LayoutWiringPoints wireData, IEnumerable<Component> toCheck) {
            Dictionary<WireSegment, List<Location>> breaks = new Dictionary<WireSegment, List<Location>>();
            foreach (Component comp in toCheck) {
                foreach (ConnectionPoint p in comp.Connections) {
                    Location pLoc = comp.GetLocation(lo).Translate(p.Dx, p.Dy);
                    foreach (WireSegment w in wireData.GetWiresContaining(pLoc)) {
                        List<Location> wLocs;
                        bool found = breaks.TryGetValue(w, out wLocs);
                        if (!found) {
                            wLocs = new List<Location>();
                            breaks[w] = wLocs;
                        }
                        wLocs.Add(pLoc);
                    }
                }
            }

            foreach (WireSegment w in breaks.Keys) {
                lo.RemoveWire(w);
                List<Location> result = breaks[w];
                result.Add(w.End0);
                result.Add(w.End1);
                result.Sort(Location.XComparer);
                Location p0 = result[0];
                foreach (Location p1 in result) {
                    if (p0 != p1) {
                        lo.AddWire(p0, p1);
                        p0 = p1;
                    }
                }
            }
        }
    }
}

