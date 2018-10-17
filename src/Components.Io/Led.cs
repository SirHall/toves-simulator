/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Sim.Inst;

namespace Toves.Components.Io {
    public class Led : ComponentSharedData {
        public Led() {
            ShareOffsetBounds(new Bounds(0, -32, 64, 64));
            this.Connections = new ConnectionPoint[] { ConnectionPoint.newInput(0, 0) };
        }
        
        public override string Name { get { return "LED"; } }

        public override bool Contains(int offsetX, int offsetY) {
            return OffsetBounds.Contains(offsetX, offsetY, 5)
                && (new Location(offsetX, offsetY).InCircle(32, 0, 37));
        }
        
        public override void Propagate(IInstanceState state) {
        }
        
        public override void Paint(IComponentPainter painter) {
            Value value = painter.GetPortValue(0);

            painter.StrokeWidth = 10;
            painter.Color = painter.GetColorFor(value);
            painter.FillCircle(32, 0, 32);
            painter.Color = 0;
            painter.StrokeCircle(32, 0, 32);
            painter.FontSize = 48;
            painter.FontStyle = Toves.AbstractGui.Canvas.FontStyle.Bold;
            painter.Color = 0x0000ff;
            painter.DrawText(32, -4, value.ToString(), TextAlign.Center | TextAlign.VCenter);
            painter.PaintPorts();
        }
    }
}