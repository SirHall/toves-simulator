/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Layout.Data;
using Toves.Sim.Inst;

namespace Toves.Layout.Comp {
    internal class SharedData {
        internal bool shouldSnap = true;
        internal Bounds offsetBounds = new Bounds(-16, -16, 32, 32);
    }

    public abstract class ComponentSharedData : Component {
        private SharedData shared;

        public ComponentSharedData() {
            shared = new SharedData();
        }

        protected void ShareShouldSnap(bool value) {
            shared.shouldSnap = value;
        }

        protected void ShareOffsetBounds(Bounds value) {
            shared.offsetBounds = value;
        }

        public override Bounds OffsetBounds { get { return shared.offsetBounds; } }
        
        public override bool ShouldSnap { get { return shared.shouldSnap; } }

        public override bool Contains(int offsetX, int offsetY) {
            return shared.offsetBounds.Contains(offsetX, offsetY, 5);
        }

        public abstract override void Propagate(IInstanceState state);

        public abstract override void Paint(IComponentPainter painter);
    }
}

