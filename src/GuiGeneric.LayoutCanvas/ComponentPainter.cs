/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

using Toves.AbstractGui.Canvas;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Sim.Inst;

namespace Toves.GuiGeneric.LayoutCanvas {
    public class ComponentPainter : PaintbrushAdapter, IComponentPainter {
        public ComponentPainter(IPaintbrush master, IInstanceState state)
                : base(master) {
            this.InstanceState = state;
        }

        public IPaintbrush Paintbrush {
            get { return this.BaseBrush; }
            set { this.BaseBrush = value; }
        }

        public ComponentInstance Instance {
            get {
                return InstanceState.Instance as ComponentInstance;
            }
        }

        public IInstanceState InstanceState { get; set; }

        public int GetColorFor(Value value) {
            return Constants.GetColorFor(value);
        }

        public void SetColorFor(Value value) {
            this.Color = GetColorFor(value);
        }

        public Value GetPortValue(int index) {
            return InstanceState.Get(index);
        }

        public void PaintPorts() {
            using (IPaintbrush pb = this.Create()) {
                int i = -1;
                foreach (ConnectionPoint p in Instance.Component.Connections) {
                    i++;
                    this.SetColorFor(InstanceState.Get(i));
                    this.FillCircle(p.Dx, p.Dy, 8);
                }
            }
        }

        public override IPaintbrush Create() {
            return new ComponentPainter(BaseBrush.Create(), this.InstanceState);
        }
    }
}