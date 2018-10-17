/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Proj.Module;
using Toves.Sim.Inst;
using Toves.Sim.Model;
using Toves.Util.Transaction;

namespace Toves.Layout.Model {
    internal class LayoutInstance : ModuleInstance {
        private LayoutModel subLayoutModel;
        private LayoutSimulation subSimulation = null;

        public LayoutInstance(ModuleComponent component, LayoutModel layoutModel) : base(component) {
            this.subLayoutModel = layoutModel;
        }

        public override object SimulationState { get { return subSimulation; } }

        protected override void HandleEventHook(InstanceEvent evnt, IInstanceState state) {
            SimulationModel simModel = null;
            if (evnt is SimulationInstanceEvent) {
                simModel = ((SimulationInstanceEvent) evnt).SimulationModel;
            }
            if (evnt.Type == InstanceEvent.Types.InstanceAdded) {
                Transaction xn = new Transaction();
                ILayoutAccess subLayout = xn.RequestReadAccess(subLayoutModel);
                ISimulationAccess sim = xn.RequestWriteAccess(simModel);
                using (xn.Start()) {
                    subSimulation = new LayoutSimulation(simModel, subLayoutModel);
                    subSimulation.SetActivated(true);
                    RenewPortLinks(subLayout, sim);
                }
            } else if (evnt.Type == InstanceEvent.Types.InstancePortsChanged) {
                if (simModel == null) {
                    simModel = subSimulation.SimulationModel;
                }
                if (subLayoutModel == null || simModel == null) {
                    return;
                }
                Transaction xn = new Transaction();
                ILayoutAccess subLayout = xn.RequestReadAccess(subLayoutModel);
                ISimulationAccess sim = xn.RequestWriteAccess(simModel);
                using (xn.Start()) {
                    RenewPortLinks(subLayout, sim);
                }
            } else if (evnt.Type == InstanceEvent.Types.InstanceRemoved) {
                subSimulation.SetActivated(false);
            }
        }

        private void RenewPortLinks(ILayoutAccess subLayout, ISimulationAccess sim) {
            IEnumerator<Port> ports = this.Ports.GetEnumerator();
            IEnumerator<Component> pins = subLayout.Pins.GetEnumerator();
            while (ports.MoveNext() && pins.MoveNext()) {
                Port compPort = ports.Current;
                Component pin = pins.Current;
                Instance pinInstance = subSimulation.GetInstance(subLayout, pin);
                if (pinInstance != null) {
                    Port pinPort = null;
                    foreach (Port tempPinPort in pinInstance.Ports) {
                        pinPort = tempPinPort;
                        break;
                    }
                    List<Link> toRemove = new List<Link>(2);
                    foreach (Node nbr in pinPort.Neighbors) {
                        if (nbr is Port && nbr != compPort) {
                            toRemove.Add(new Link(pinPort, nbr));
                        }
                    }
                    foreach (Node nbr in compPort.Neighbors) {
                        if (nbr is Port && nbr != pinPort) {
                            toRemove.Add(new Link(compPort, nbr));
                        }
                    }
                    foreach (Link outLink in toRemove) {
                        sim.RemoveLink(outLink);
                    }
                    sim.AddLink(new Link(compPort, pinPort));
                }
            }
        }
    }
}