/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Sim.Inst;

namespace Toves.Components.Gates {
    public class OrGate : BasicGate {
        public override string Name { get { return "OR Gate"; } }

        public override bool Contains(int offsetX, int offsetY) {
            if (this.OffsetBounds.Contains(offsetX, offsetY, 5)) {
                Location offs = new Location(offsetX, offsetY);
                return offs.InCircle(-96, -72, 125)
                    && offs.InCircle(-96,  72, 125)
                    && !offs.InCircle(-179, 0, 91);
            } else {
                return false;
            }
        }

        public override void Propagate(IInstanceState state) {
            Value in1 = state.Get(1);
            Value in2 = state.Get(2);
            state.Set(0, in1.Or(in2), 1);
        }

        public override void Paint(IComponentPainter painter) {
            painter.StrokeWidth = 10;
            painter.StrokeArc( -96, -72, 120,  37,  53);
            painter.StrokeArc( -96,  72, 120, -90,  53);
            painter.StrokeArc(-179,   0,  96, -30,  60);
            painter.PaintPorts();
        }
    }
}