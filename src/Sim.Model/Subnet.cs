/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using System.Text;
using Toves.Sim.Inst;
using Toves.Util.Collections;

namespace Toves.Sim.Model {
    public class Subnet {
        private static readonly bool Debug = false;
        private static int lastIdAllocated = -1;
        private static readonly ICollection<Port> EmptyPortList = new List<Port>().AsReadOnly();

        private int id;
        private List<Node> nodes = new List<Node>();
        private ICollection<Port> drivers = EmptyPortList;
        private ICollection<Port> readers = EmptyPortList;
        private Value value;

        private Subnet(int id) {
            this.id = id;
        }

        public int Width { get; private set; }

        public IEnumerable<Port> Drivers { get { return drivers; } }
        
        public IEnumerable<Port> Readers { get { return readers; } }

        internal IEnumerable<Node> Nodes { get { return nodes; } }

        public override string ToString() {
            return String.Format("net{0}", id);
        }

        public Value GetValue(SimulationModel.Key key) {
            if (key == null) {
                throw new ArgumentException("key needed");
            }
            return value;
        }

        public void SetValue(SimulationModel.Key key, Value value) {
            if (key == null) {
                throw new ArgumentException("key needed");
            }
            this.value = value;
        }

        public static List<Subnet> UpdateNets(SimulationModel.Key key, ISimulationAccess model, List<Subnet> oldNets) {
            if (key == null) {
                throw new ArgumentException("key needed");
            }

            if (Debug) {
                Console.WriteLine("Update subnets");
            }

            foreach (Node node in model.Nodes) {
                node.SetTempSubnet(key, null);
            }

            List<Subnet> sortedNets = new List<Subnet>(oldNets);
            sortedNets.Sort((n0, n1) => n1.nodes.Count - n0.nodes.Count);
            List<Subnet> allNewNets = new List<Subnet>();
            foreach (Subnet oldNet in sortedNets) {
                List<Subnet> newNets = new List<Subnet>();
                int largestSize = 0;
                Subnet largestNet = null;
                foreach (Node node in oldNet.nodes) {
                    if (node.TempSubnet == null) {
                        Subnet net = FindSubnetFrom(key, node);
                        newNets.Add(net);
                        int netSize = net.nodes.Count;
                        if (netSize > largestSize) {
                            largestSize = netSize;
                            largestNet = net;
                        }
                    }
                }
                if (largestNet == null) {
                    // no nets identified, so nothing to do
                    if (Debug) {
                        Console.WriteLine("   {0} deleted", oldNet);
                    }
                } else if (newNets.Count == 1 && EqualsListsShallow(oldNet.nodes, newNets[0].nodes)) {
                    if (Debug) {
                        Console.WriteLine("   {0} unchanged: drivers {1} readers {2} nodes {3}", oldNet,
                                          ",".JoinObjectStrings(oldNet.drivers), ",".JoinObjectStrings(oldNet.readers),
                                          ",".JoinObjectStrings(oldNet.nodes));
                    }
                    allNewNets.Add(oldNet); // just keep the old net, since they're identical
                } else {
                    oldNet.nodes = largestNet.nodes;
                    foreach (Subnet rawNet in newNets) {
                        Subnet newNet = rawNet == largestNet ? oldNet : rawNet;
                        allNewNets.Add(newNet);
                        ProcessPortsAndSetSubnets(key, newNet);
                        key.Engine.AddDirtyNet(newNet);
                        if (Debug) {
                            Console.WriteLine("   {0} to {1}: drivers {2} readers {3} nodes {4}", oldNet, newNet,
                                              ",".JoinObjectStrings(newNet.drivers), ",".JoinObjectStrings(newNet.readers),
                                              ",".JoinObjectStrings(newNet.nodes));
                        }
                    }
                }
            }
            foreach (Node node in model.Nodes) {
                if (node.TempSubnet == null) {
                    Subnet net = FindSubnetFrom(key, node);
                    allNewNets.Add(net);
                    ProcessPortsAndSetSubnets(key, net);
                    if (Debug) {
                        Console.WriteLine("dflt net {0}: drivers {1}, readers {2}, nodes {3}", net,
                                          ",".JoinObjectStrings(net.drivers), ",".JoinObjectStrings(net.readers),
                                          ",".JoinObjectStrings(net.nodes));
                    }
                }
            }

            return allNewNets;
        }

        private static Subnet FindSubnetFrom(SimulationModel.Key key, Node start) {
            int id = lastIdAllocated + 1;
            lastIdAllocated = id;
            Subnet net = new Subnet(id);
            List<Node> found = net.nodes;
            start.SetTempSubnet(key, net);
            found.Add(start);
            for (int i = 0; i < found.Count; i++) {
                Node u = found[i];
                foreach (Node v in u.Neighbors) {
                    if (v.TempSubnet == null) {
                        v.SetTempSubnet(key, net);
                        found.Add(v);
                    }
                }
            }
            return net;
        }

        private static bool EqualsListsShallow(List<Node> a, List<Node> b) {
            int n = a.Count;
            if (b.Count == n) {
                for (int i = 0; i < n; i++) {
                    if (a[i] != b[i]) {
                        return false;
                    }
                }
                return true;
            } else {
                return false;
            }
        }

        private static void ProcessPortsAndSetSubnets(SimulationModel.Key key, Subnet net) {
            List<Port> readers = new List<Port>();
            List<Port> drivers = new List<Port>();
            foreach (Node node in net.nodes) {
                node.SetSubnet(key, net);
                Port port = node as Port;
                if (port != null) {
                    if (port.IsInput) {
                        readers.Add(port);
                    }
                    if (port.IsOutput) {
                        drivers.Add(port);
                    }
                }
            }
            readers.TrimExcess();
            drivers.TrimExcess();
            net.readers = readers;
            net.drivers = drivers;
        }
    }
}

