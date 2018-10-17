/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Layout.Comp;
using Toves.Layout.Data;

namespace Toves.Proj.Module {
    public interface IPresentation {
        Bounds OffsetBounds { get; }
        IEnumerable<ConnectionPoint> Connections { get; }
        IEnumerable<ConnectionPoint> UpdateConnections(IEnumerable<Connection> connections);
        bool Contains(int offsetX, int offsetY);
        void Paint(IComponentPainter painter);
    }
}