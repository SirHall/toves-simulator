/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Sim.Inst;

namespace Toves.Components.Gates {
    public class NotGate : ComponentSharedData {
        public NotGate() {
            ShareOffsetBounds(new Bounds(-64, -20, 64, 40));
            this.Connections = new ConnectionPoint[] {
                ConnectionPoint.newOutput(0, 0), ConnectionPoint.newInput(-64, 0) };
        }

        public override string Name { get { return "NOT Gate"; } }

        public override bool Contains(int offsetX, int offsetY) {
            if (OffsetBounds.Contains(offsetX, offsetY, 5)) {
                Location offs = new Location(offsetX, offsetY);
                return offs.InCircle(-10, 0, 15)
                    || Math.Abs(offsetY) <= 25 * (-15 - offsetX) / 41.0;
            } else {
                return false;
            }
        }

        public override void Propagate(IInstanceState state) {
            Value incoming = state.Get(1);
            state.Set(0, incoming.Not, 1);
        }

        public override void Paint(IComponentPainter painter) {
            painter.StrokeWidth = 10;
            painter.StrokePolygon(new int[] { -64, -20, -64 },
                new int[] { -20, 0, 20 });
            painter.StrokeCircle(-10, 0, 10);
            painter.PaintPorts();
        }
    }
}

