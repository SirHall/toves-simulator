/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Sim.Inst;
using Toves.Util.Transaction;
using Toves.Util.Collections;

namespace Toves.Sim.Model {
    public class SimulationModel : Resource<ISimulationAccess> {
        private static readonly bool Debug = false;
        private static readonly bool DebugAfterUpdates = false;

        public sealed class Key {
            private SimulationModel model;

            internal Key(SimulationModel model) {
                if (model.key != null) {
                    throw new ArgumentException("key already created");
                }
                this.model = model;
            }

            public Engine Engine { get { return model.engine; } }
        }

        private class Access : ResourceAccess, ISimulationAccess {
            private SimulationModel sim;
            internal List<Instance> instancesAdded = null;
            internal List<Instance> instancesRemoved = null;
            internal List<Node> dirtyNodes = null;
            internal bool netsChanged = false;
            
            internal Access(SimulationModel sim, bool canWrite) : base(sim, canWrite) {
                this.sim = sim;
            }

            public SimulationModel Simulation {
                get { return sim; }
            }

            public IEnumerable<Node> Nodes {
                get {
                    CheckReadAccess();
                    return sim.nodes;
                }
            }
            
            public IEnumerable<Instance> Instances {
                get {
                    CheckReadAccess();
                    return sim.instances.AsReadOnly();
                }
            }
            
            public void AddLink(Link value) {
                CheckWriteAccess();
                if (Debug) {
                    Console.Error.WriteLine("{0}: add link {1}", sim, value);
                }                          
                Node n0 = value.Source;
                Node n1 = value.Destination;
                sim.nodes.Add(n0);
                sim.nodes.Add(n1);
                n0.AddLink(sim.key, value);
                n1.AddLink(sim.key, value);
                netsChanged = true;

                if (dirtyNodes == null) {
                    dirtyNodes = new List<Node>();
                }
                dirtyNodes.Add(n0);
                dirtyNodes.Add(n1);
            }
            
            public void RemoveLink(Link value) {
                CheckWriteAccess();
                if (Debug) {
                    Console.Error.WriteLine("{0}: remove link {1}", sim, value);
                }                          
                Node n0 = value.Source;
                Node n1 = value.Destination;
                n0.RemoveLink(sim.key, value);
                n1.RemoveLink(sim.key, value);
                if (n0.UseCount == 0) {
                    sim.nodes.Remove(n0);
                }
                if (n1.UseCount == 0) {
                    sim.nodes.Remove(n1);
                }
                netsChanged = true;

                if (dirtyNodes == null) {
                    dirtyNodes = new List<Node>();
                }
                dirtyNodes.Add(n0);
                dirtyNodes.Add(n1);
            }
            
            public void AddInstance(Instance value) {
                CheckWriteAccess();
                if (Debug) {
                    Console.Error.WriteLine("{0}: add instance {1} [ports {2}]", sim, value,
                                            ",".JoinObjectStrings(value.Ports));
                }                          

                sim.instances.Add(value);
                foreach (Port p in value.Ports) {
                    p.AddToUseCount(sim.key, 1);
                    sim.nodes.Add(p);
                }
                sim.engine.AddDirtyInstance(value);
                netsChanged = true;
                if (instancesAdded == null) {
                    instancesAdded = new List<Instance>();
                }
                instancesAdded.Add(value);
            }
            
            public void RemoveInstance(Instance value) {
                CheckWriteAccess();
                if (Debug) {
                    Console.Error.WriteLine("{0}: remove link {1}", sim, value);
                }                          

                sim.instances.Remove(value);
                foreach (Port p in value.Ports) {
                    p.AddToUseCount(sim.key, -1);
                    if (p.UseCount == 0) {
                        sim.nodes.Remove(p);
                    }
                }
                netsChanged = true;
                if (instancesRemoved == null) {
                    instancesRemoved = new List<Instance>();
                }
                instancesRemoved.Add(value);
            }

            public Value Get(Node n) {
                CheckReadAccess();
                if (n == null) {
                    return Value.X;
                } else {
                    Subnet sub = n.Subnet;
                    Value val = sub == null ? null : sub.GetValue(sim.key);
                    if (n is Port) {
                        Port port = (Port) n;
                        if (port.IsOutput) {
                            Value drive = port.GetDrivenValue(sim.key);
                            val = val == null ? drive : val.Resolve(drive ?? Value.X);
                        }
                    }
                    return val ?? Value.X;
                }
            }
            
            public Value GetDriven(Port port) {
                CheckReadAccess();
                if (port == null) {
                    return Value.X;
                } else if (port.IsOutput) {
                    Value pVal = port.GetDrivenValue(sim.key);
                    return pVal ?? Value.X;
                } else {
                    throw new InvalidOperationException("port is not an output");
                }
            }

            public void Set(Port port, Value value, int delay) {
                CheckWriteAccess();
                if (port != null && port.IsOutput) {
                    sim.engine.QueueSet(port, value, delay);
                } else {
                    throw new InvalidOperationException("port is not an output");
                }
            }

            public void MarkInstanceDirty(Instance value) {
                CheckWriteAccess();
                sim.engine.AddDirtyInstance(value);
            }

            public bool IsStepPending() {
                CheckReadAccess();
                return sim.engine.IsStepPending();
            }

            public void StepSimulation() {
                CheckWriteAccess();
                sim.engine.Step(sim.key, this);
            }
        }

        private Key key = null;
        private Engine engine = new Engine();
        private List<Instance> instances = new List<Instance>();
        private HashSet<Node> nodes = new HashSet<Node>();
        private List<Subnet> subnets = new List<Subnet>();
        private ResourceHelper resourceHelper = new ResourceHelper();

        public SimulationModel() {
            this.key = new Key(this);
        }

        public ResourceHelper ResourceHelper { get { return resourceHelper; } }

        public event EventHandler<SimulationModelModifiedArgs> SimulationModelModifiedEvent;
        
        protected virtual void OnSimulationModelModified() {
            EventHandler<SimulationModelModifiedArgs> handler = SimulationModelModifiedEvent;
            if (handler != null) {
                handler(this, new SimulationModelModifiedArgs());
            }
        }

        public void Hook(ResourceHookType hookType, IResourceAccess rawAccess) {
            Access access = rawAccess as Access;
            if (hookType == ResourceHookType.AfterWrite) {
                while (true) {
                    bool netsChanged = access.netsChanged;
                    List<Node> dirtyNodes = access.dirtyNodes;
                    List<Instance> instancesAdded = access.instancesAdded;
                    List<Instance> instancesRemoved = access.instancesRemoved;
                    if (!netsChanged && dirtyNodes == null && instancesAdded == null && instancesRemoved == null) {
                        break;
                    }
                    access.netsChanged = false;
                    access.dirtyNodes = null;
                    access.instancesAdded = null;
                    access.instancesRemoved = null;

                    if (netsChanged) {
                        this.subnets = Subnet.UpdateNets(key, access, subnets);
                    }
                    if (dirtyNodes != null) {
                        foreach (Node n in dirtyNodes) {
                            engine.AddDirtyNet(n.Subnet);
                        }
                    }
                    if (instancesAdded != null) {
                        InstanceEvent evnt = new SimulationInstanceEvent(InstanceEvent.Types.InstanceAdded, this);
                        InstanceEvent evnt2 = new SimulationInstanceEvent(InstanceEvent.Types.InstanceDirty, this);
                        InstanceState state = new InstanceState(access, null);
                        foreach (Instance i in instancesAdded) {
                            state.Instance = i;
                            i.HandleEvent(evnt, state);
                            i.HandleEvent(evnt2, state);
                        }
                    }
                    if (instancesRemoved != null) {
                        InstanceEvent evnt = new SimulationInstanceEvent(InstanceEvent.Types.InstanceRemoved, this);
                        InstanceState state = new InstanceState(access, null);
                        foreach (Instance i in instancesRemoved) {
                            state.Instance = i;
                            i.HandleEvent(evnt, state);
                        }
                    }
                }
            } else if (hookType == ResourceHookType.AfterDowngrade) {
                OnSimulationModelModified();
                
                if (DebugAfterUpdates) {
                    Console.WriteLine("Simulation model update complete");
                    foreach (Instance i in instances) {
                        Console.WriteLine("  {0} ports {1}", i, ", ".JoinObjectStrings(i.Ports));
                    }
                    foreach (Node u in nodes) {
                        Port p = u as Port;
                        if (p == null || p.Type == PortType.Passive) {
                            Console.WriteLine("  {0} neighbors {1}", u, ", ".JoinObjectStrings(u.Neighbors));
                        }
                    }
                    foreach (Subnet n in subnets) {
                        HashSet<Node> nNodes = new HashSet<Node>(n.Nodes);
                        List<string> driverStrs = new List<string>();
                        foreach (Port u in n.Drivers) {
                            driverStrs.Add(string.Format("{0}/{1}", u, u.GetDrivenValue(key)));
                            nNodes.Remove(u);
                        }
                        foreach (Node u in n.Readers) {
                            nNodes.Remove(u);
                        }
                        Console.WriteLine("  {0} val {1} drivers {2} readers {3} nodes {4}", n, n.GetValue(key),
                                          ", ".JoinObjectStrings(driverStrs),
                                          ", ".JoinObjectStrings(n.Readers),
                                          ", ".JoinObjectStrings(nNodes));
                    }
                }
            }
        }

        public ISimulationAccess CreateAccess(bool canWrite) {
            return new Access(this, canWrite);
        }
    }
}

