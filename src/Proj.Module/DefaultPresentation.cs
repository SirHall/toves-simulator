/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Layout.Comp;
using Toves.Layout.Data;

namespace Toves.Proj.Module {
    public class DefaultPresentation : IPresentation {
        private Bounds bounds;
        private ConnectionPoint[] connections;

        public DefaultPresentation(ProjectModule module) {
            UpdateConnections(module.Implementation.Connections);
        }

        public Bounds OffsetBounds {
            get { return bounds; }
        }

        public IEnumerable<ConnectionPoint> Connections {
            get { return connections; }
        }

        public bool Contains(int offsetX, int offsetY) {
            return bounds.Contains(offsetX, offsetY);
        }

        public IEnumerable<ConnectionPoint> UpdateConnections(IEnumerable<Connection> newConnections) {
            int inCount = 0;
            int outCount = 0;
            foreach (Connection conn in newConnections) {
                if (conn.IsInput) {
                    inCount++;
                } else {
                    outCount++;
                }
            }
            int gridHeight = Math.Max(1, Math.Max(inCount, outCount));
            int height = 32 * (gridHeight + 1);
            int yOffs = -32 * ((gridHeight / 2) + 1);
            Bounds newBounds = new Bounds(-96, yOffs, 96, height);
            ConnectionPoint[] newConns = new ConnectionPoint[inCount + outCount];
            int inOffs = yOffs + 32 * (1 + (gridHeight - inCount) / 2);
            int outOffs = yOffs + 32 * (1 + (gridHeight - outCount) / 2);
            for (int i = 0; i < newConns.Length; i++) {
                if (i < inCount) {
                    newConns[i] = ConnectionPoint.newPassive(-96, inOffs + 32 * i);
                } else {
                    newConns[i] = ConnectionPoint.newPassive(0, outOffs + 32 * (i - inCount));
                }
            }
            bounds = newBounds;
            connections = newConns;
            return newConns;
        }

        public void Paint(IComponentPainter painter) {
            Bounds bds = bounds;
            painter.StrokeWidth = 10;
            painter.Color = 0x888888;
            painter.StrokeArc(bds.X + bds.Width / 2, bds.Y, 16, 0, 180);
            painter.Color = 0;
            painter.StrokeRectangle(bds.X, bds.Y, bds.Width, bds.Height);
            painter.PaintPorts();
        }
    }
}