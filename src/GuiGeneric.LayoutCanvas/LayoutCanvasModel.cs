/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;

using Toves.AbstractGui.Canvas;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Layout.Model;
using Toves.Proj.Module;
using Toves.Sim.Inst;
using Toves.Sim.Model;
using Toves.Util.Transaction;

namespace Toves.GuiGeneric.LayoutCanvas {
    public class LayoutCanvasModel : AbstractCanvasModel {
        private ICollection<Component> hidden = new HashSet<Component>();
        private RepaintThread repainter;
        private CanvasCallback callback;

        public LayoutCanvasModel(CanvasCallback callback) {
            this.callback = callback;
            this.repainter = new RepaintThread(this);
            this.NullGesture = new GestureNull(this);
        }

        public LayoutModel Layout { get; private set; }

        public LayoutSimulation LayoutSim { get; private set; }

        public LayoutWiringPoints WiringPoints { get; private set; }

        internal void RequestView(ProjectModule module, LayoutSimulation sim) {
            callback.SetView(module, sim);
        }

        public IEnumerable<Component> Hidden {
            get { return hidden; }
            set {
                hidden.Clear();
                if (value != null) {
                    foreach (Component component in value) {
                        hidden.Add(component);
                    }
                }
            }
        }

        public void SetView(LayoutModel value, LayoutSimulation layoutSim) {
            LayoutModel oldLayout = this.Layout;
            LayoutSimulation oldSim = this.LayoutSim;
            bool updated = false;
            if (value != oldLayout) {
                this.Layout = value;
                this.LayoutSim = layoutSim;
                repainter.SetSimulationModel(layoutSim.SimulationModel);
                WiringPoints = new LayoutWiringPoints(value);
                hidden = new HashSet<Component>();
                this.NullGesture = new GestureNull(this);
                if (oldLayout != null) {
                    oldLayout.LayoutModifiedEvent -= LayoutUpdated;
                }
                if (value != null) {
                    value.LayoutModifiedEvent += LayoutUpdated;
                }
                updated = true;
            } else if (oldSim != layoutSim) {
                updated = true;
                this.LayoutSim = layoutSim;
                repainter.SetSimulationModel(layoutSim.SimulationModel);
            }
            if (updated) {
                RepaintCanvas();
            }
        }

        public override void Dispose() {
            SetView(null, null);
        }

        public override void HandleKeyPressEvent(IKeyEvent evnt) {
        }

        private void LayoutUpdated(object sender, LayoutModifiedArgs evnt) {
            WiringPoints.Update();
            // solders.Update();
        }

        public void Execute(Action<ILayoutAccess> action) {
            Transaction xn = new Transaction();
            ILayoutAccess lo = xn.RequestWriteAccess(Layout);
            using (xn.Start()) {
                action(lo);
            }
        }

        protected override void PaintModel(IPaintbrush pb) {
            repainter.NotifyRepainted();
            LayoutModel layoutModel = this.Layout;
            LayoutSimulation layoutSim = this.LayoutSim;
            if (layoutModel == null) {
                return;
            }
            if (layoutSim.LayoutModel != layoutModel) {
                Console.Error.WriteLine("layoutSim and layoutModel do not match");
                return;
            }
            ComponentPainter ip = new ComponentPainter(pb, null);
            bool noHidden = hidden.Count == 0;
            Transaction xn = new Transaction();
            ISimulationAccess sim = xn.RequestReadAccess(layoutSim.SimulationModel);
            ILayoutAccess layout = xn.RequestReadAccess(layoutModel);
            using (xn.Start()) {
                using (IPaintbrush pbSub = pb.Create()) {
                    pbSub.StrokeWidth = Constants.WIRE_WIDTH;
                    foreach (WireSegment wire in layout.Wires) {
                        Value val0 = layoutSim.GetValueAt(layout, sim, wire.End0);
                        pbSub.Color = Constants.GetColorFor(val0);
                        pbSub.StrokeLine(wire.End0.X, wire.End0.Y, wire.End1.X, wire.End1.Y);
                    }
                }

                InstanceState state = new InstanceState(sim, null);
                ip.InstanceState = state;
                foreach (Component component in layout.Components) {
                    if (noHidden || !hidden.Contains(component)) {
                        Location loc = component.GetLocation(layout);
                        using (IPaintbrush pbSub = pb.Create()) {
                            pbSub.TranslateCoordinates(loc.X, loc.Y);
                            ip.Paintbrush = pbSub;
                            state.Instance = layoutSim.GetInstance(layout, component);
                            component.Paint(ip);
                        }
                    }
                }

                using (IPaintbrush pbSub = pb.Create()) {
                    foreach (Location loc in WiringPoints.SolderPoints) {
                        Value val = layoutSim.GetValueAt(layout, sim, loc);
                        pbSub.Color = Constants.GetColorFor(val);
                        if (noHidden || WiringPoints.IsSolderPoint(loc, hidden)) {
                            pbSub.FillCircle(loc.X, loc.Y, Constants.SOLDER_RADIUS);
                        }
                    }
                }
            }
        }
    }
}

