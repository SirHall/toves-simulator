/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Sim.Inst;

namespace Toves.Components.Gates {
    public abstract class BasicGate : ComponentSharedData {
        public BasicGate() {
            this.ShareOffsetBounds(new Bounds(-96, -48, 96, 96));
            this.Connections = new ConnectionPoint[] {
                ConnectionPoint.newOutput(0, 0), ConnectionPoint.newInput(-96, -32),
                ConnectionPoint.newInput(-96, 32) };
        }

        public abstract override bool Contains(int offsetX, int offsetY);

        public abstract override void Propagate(IInstanceState state);

        public abstract override void Paint(IComponentPainter painter);
    }
}