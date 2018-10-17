/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Util.Transaction;

namespace Toves.Layout.Model {
    public interface ILayoutAccess : IResourceAccess {
        LayoutModel Layout { get; }

        IEnumerable<Component> Components { get; }

        IEnumerable<WireSegment> Wires { get; }

        IEnumerable<LayoutNode> Nodes { get; }

        IEnumerable<Component> Pins { get; }

        LayoutNode FindNode(Location loc);

        Component AddComponent(Component prototype, int x, int y);

        void RemoveComponent(Component component);

        void MoveComponent(Component component, int dx, int dy);

        void AddWire(Location loc0, Location loc1);

        void RemoveWire(WireSegment wire);
    }
}

