/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Layout.Model;
using Toves.Util.Transaction;

namespace Toves.GuiGeneric.LayoutCanvas
{
    internal struct PortData {
        internal Component component;
        internal int portIndex;

        internal PortData(Component component, int portIndex)
        {
            this.component = component;
            this.portIndex = portIndex;
        }
    }

    internal class PointData {
        internal PortData[] ports = null;
        internal WireSegment[] wiresEnding = null;
        internal WireSegment[] wiresContaining = null;

        private static void Extend<T>(ref T[] data, T value) {
            if (data == null) {
                data = new T[] { value };
            } else {
                int index = data.Length;
                Array.Resize(ref data, index + 1);
                data[index] = value;
            }
        }

        public bool IsSolderPoint() {
            PortData[] ps = ports;
            WireSegment[] ws = wiresEnding;
            int a = ps == null ? 0 : ps.Length;
            int b = ws == null ? 0 : ws.Length;
            return a + b >= 3;
        }

        public bool IsSolderPoint(ICollection<Component> exclude) {
            PortData[] ps = ports;
            WireSegment[] ws = wiresEnding;
            int a = 0;
            if (ps != null) {
                foreach (PortData pd in ps) {
                    if (!exclude.Contains(pd.component)) {
                        a++;
                    }
                }
            }
            int b = ws == null ? 0 : ws.Length;
            return a + b >= 3;
        }

        public void AddPort(Component component, int portIndex)
        {
            Extend(ref ports, new PortData(component, portIndex));
        }

        public void AddWireEnding(WireSegment segment)
        {
            Extend(ref wiresEnding, segment);
        }

        public void AddWireContaining(WireSegment segment)
        {
            Extend(ref wiresContaining, segment);
        }
    }

    public class LayoutWiringPoints
    {
        private static WireSegment[] emptyWires = new WireSegment[0];

        private LayoutModel layout;
        private Dictionary<Location, PointData> wiringPoints;
        private List<Location> solderPoints;

        public LayoutWiringPoints(LayoutModel layout)
        {
            this.layout = layout;
            Update();
        }

        public IEnumerable<Location> SolderPoints {
            get { return solderPoints; }
        }

        public bool IsSolderPoint(Location loc, ICollection<Component> exclude) {
            PointData pd = wiringPoints[loc];
            return pd == null ? false : pd.IsSolderPoint(exclude);
        }

        public bool Contains(Location query)
        {
            return wiringPoints.ContainsKey(query);
        }

        public bool IsPortAt(Location query)
        {
            PointData value;
            bool found = wiringPoints.TryGetValue(query, out value);
            return found && value.ports != null && value.ports.Length > 0;
        }

        public IEnumerable<WireSegment> GetWiresEndingAt(Location query)
        {
            PointData value;
            bool found = wiringPoints.TryGetValue(query, out value);
            return found && value.wiresEnding != null ? value.wiresEnding : emptyWires;
        }

        public IEnumerable<WireSegment> GetWiresContaining(Location query)
        {
            PointData value;
            bool found = wiringPoints.TryGetValue(query, out value);
            return found && value.wiresContaining != null ? value.wiresContaining : emptyWires;
        }

        private PointData GetPointData(Dictionary<Location, PointData> source,
                                       Location query)
        {
            PointData value;
            bool found = source.TryGetValue(query, out value);
            if (!found) {
                value = new PointData();
                source.Add(query, value);
            }
            return value;
        }

        public void Update() {
            Dictionary<Location, PointData> pts = new Dictionary<Location, PointData>();
            Transaction xn = new Transaction();
            ILayoutAccess lo = xn.RequestReadAccess(layout);
            using (xn.Start()) {
                foreach (WireSegment wire in lo.Wires) {
                    GetPointData(pts, wire.End0).AddWireEnding(wire);
                    GetPointData(pts, wire.End1).AddWireEnding(wire);
                    foreach (Location loc in wire.GetLocationsOnWire(Constants.GRID_SIZE, false)) {
                        GetPointData(pts, loc).AddWireContaining(wire);
                    }
                }
                foreach (Component component in lo.Components) {
                    Location iloc = component.GetLocation(lo);
                    int i = -1;
                    foreach (ConnectionPoint port in component.Connections) {
                        i++;
                        Location loc = iloc.Translate(port.Dx, port.Dy);
                        GetPointData(pts, loc).AddPort(component, i);
                    }
                }
            }

            List<Location> newSolders = new List<Location>();
            foreach (KeyValuePair<Location, PointData> entry in pts) {
                if (entry.Value.IsSolderPoint()) {
                    newSolders.Add(entry.Key);
                }
            }

            wiringPoints = pts;
            solderPoints = newSolders;
        }
    }
}

