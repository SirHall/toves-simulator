/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Layout.Model;
using Toves.Sim.Inst;
using Toves.Sim.Model;
using Toves.Util.Transaction;

namespace Toves.Layout.Model {
    public class LayoutSimulation {
        private static readonly bool Debug = false;

        private SimulationModel simModel;
        private bool activated = false;
        private Dictionary<LayoutNode, Node> nodeMap = new Dictionary<LayoutNode, Node>();
        private Dictionary<Component, Instance> instanceMap = new Dictionary<Component, Instance>();

        public LayoutSimulation(SimulationModel simModel, LayoutModel model) {
            this.simModel = simModel;
            this.LayoutModel = model;
            model.LayoutModifiedEvent += OnUpdate;

            Configure(null);
        }

        public LayoutModel LayoutModel { get; private set; }

        public SimulationModel SimulationModel { get { return simModel; } }

        public Value GetValueAt(ILayoutAccess layout, ISimulationAccess sim, Location loc) {
            layout.CheckReadAccess();
            sim.CheckReadAccess();
            LayoutNode sup = layout.FindNode(loc);
            if (nodeMap.ContainsKey(sup)) {
                return sim.Get(nodeMap[sup]);
            } else {
                return Value.X;
            }
        }

        public Instance GetInstance(ILayoutAccess layout, Component component) {
            layout.CheckReadAccess();
            return instanceMap[component];
        }

        public void SetActivated(bool value) {
            if (value != activated) {
                if (value) {
                    Configure(null);
                } else {
                    Clear();
                }
            }
        }

        private void OnUpdate(object sender, LayoutModifiedArgs args) {
            Configure(args.Layout);
        }

        private void Configure(ILayoutAccess layout) {
            activated = true;
            Transaction xn = new Transaction();
            ISimulationAccess sim = xn.RequestWriteAccess(simModel);
            if (layout == null) {
                layout = xn.RequestReadAccess(this.LayoutModel);
            }
            using (xn.Start()) {
                foreach (LayoutNode node in layout.Nodes) {
                    if (!nodeMap.ContainsKey(node)) {
                        nodeMap[node] = new Node();
                    }
                }
                foreach (Component comp in layout.Components) {
                    if (!instanceMap.ContainsKey(comp)) {
                        Instance instance = comp.CreateInstance();
                        instanceMap[comp] = instance;
                        sim.AddInstance(instance);
                    }
                }
                if (Debug) {
                    Console.WriteLine("Circuit edited");
                }
                foreach (Component comp in layout.Components) {
                    Location compLoc = comp.GetLocation(layout);
                    String cstr = null;
                    Instance instance = instanceMap[comp];
                    IEnumerator<ConnectionPoint> conns = comp.Connections.GetEnumerator();
                    IEnumerator<Port> ports = instance.Ports.GetEnumerator();
                    int i = -1;
                    while (conns.MoveNext() && ports.MoveNext()) {
                        i++;
                        ConnectionPoint conn = conns.Current;
                        Port port = ports.Current;
                        Location connLoc = compLoc.Translate(conn.Dx, conn.Dy);
                        if (Debug) {
                            if (i == 0) {
                                cstr = String.Format("{0}[{1}]", comp.GetType().Name, compLoc.ToString());
                            } else {
                                cstr = String.Format("{0," + cstr.Length + "}", "");
                            }
                            Console.WriteLine("  {0}.{1}: {2} [{3}]", cstr, i, layout.FindNode(connLoc), connLoc);
                        }
                        LinkPortTo(sim, port, nodeMap[layout.FindNode(connLoc)]);
                    }
                }
            }
        }

        private void LinkPortTo(ISimulationAccess sim, Port port, Node nDesired) {
            Node nCurrent = null;
            List<Node> nRemove = null;
            foreach (Node n in port.Neighbors) {
                if (n is Port) {
                    // ignore - this is a link to the submodule's circuit
                } else if (nCurrent == null) {
                    nCurrent = n;
                } else {
                    if (nRemove == null) {
                        nRemove = new List<Node>();
                    }
                    nRemove.Add(n);
                }
            }
            if (nRemove != null) {
                foreach (Node n in nRemove) {
                    if (Debug) {
                        Console.WriteLine("    remove link {0}-{0}", port, n);
                    }
                    sim.RemoveLink(new Link(port, n));
                }
            }
            if (nDesired != nCurrent) {
                if (nCurrent != null) {
                    if (Debug) {
                        Console.WriteLine("    remove link {0}-{1}", port, nCurrent);
                    }
                    sim.RemoveLink(new Link(port, nCurrent));
                }
                if (nDesired != null) {
                    if (Debug) {
                        Console.WriteLine("    add link {0}-{1}", port, nDesired);
                    }
                    sim.AddLink(new Link(port, nDesired));
                }
            }
        }
        
        private void Clear() {
            Transaction xn = new Transaction();
            ISimulationAccess sim = xn.RequestWriteAccess(simModel);
            using (xn.Start()) {
                foreach (Instance i in instanceMap.Values) {
                    sim.RemoveInstance(i);
                }
                foreach (Node u in nodeMap.Values) {
                    foreach (Link link in u.Links) {
                        sim.RemoveLink(link);
                    }
                }
            }
        }

    }
}

