/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Sim.Inst;

namespace Toves.Layout.Wiring {
    public class Pin : ComponentSharedData {
        public Pin() {
            ShareOffsetBounds(new Bounds(-64, -32, 64, 64));
            this.Connections = new ConnectionPoint[] { ConnectionPoint.newPassive(0, 0) };
        }

        public override string Name { get { return "Input Pin"; } }

        public override bool Contains(int offsetX, int offsetY) {
            return OffsetBounds.Contains(offsetX, offsetY, 5);
        }

        public override void Propagate(IInstanceState state) { }

        public override void Paint(IComponentPainter painter) {
            Value value = painter.GetPortValue(0);

            painter.StrokeWidth = 10;
            painter.Color = 0;
            painter.StrokeRectangle(-64, -32, 64, 64);
            painter.FontSize = 48;
            painter.FontStyle = Toves.AbstractGui.Canvas.FontStyle.Bold;
            painter.Color = 0x0000ff;
            painter.DrawText(-32, -4, value.ToString(), TextAlign.Center | TextAlign.VCenter);
            painter.PaintPorts();
        }
    }
}