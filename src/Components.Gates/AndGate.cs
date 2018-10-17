/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Sim.Inst;

namespace Toves.Components.Gates {
    public class AndGate : BasicGate {
        public override string Name { get { return "AND Gate"; } }

        public override bool Contains(int offsetX, int offsetY) {
            if (this.OffsetBounds.Contains(offsetX, offsetY, 5)) {
                if (offsetX < -48) {
                    return true;
                } else {
                    return new Location(offsetX, offsetY).InCircle(-48, 0, 53);
                }
            } else {
                return false;
            }
        }

        public override void Propagate(IInstanceState state) {
            Value in1 = state.Get(1);
            Value in2 = state.Get(2);
            state.Set(0, in1.And(in2), 1);
        }

        public override void Paint(IComponentPainter painter) {
            painter.StrokeWidth = 10;
            painter.StrokeArc(-48, 0, 48, -90, 180);
            painter.StrokeLines(
                new int[] { -48, -96, -96, -48 },
                new int[] { -48, -48,  48,  48 });
            painter.PaintPorts();
        }
    }
}

