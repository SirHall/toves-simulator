/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using System.Threading;
using Toves.Util.Collections;
using Toves.Sim.Inst;

namespace Toves.Sim.Model {
    public class Engine {
        private static readonly bool Debug = false;

        private struct SetEvent : IComparable<SetEvent> {
            internal Port port;
            internal Value value;
            internal long when;
            
            internal SetEvent(Port port, Value value, long when) {
                this.port = port;
                this.value = value;
                this.when = when;
            }
            
            public int CompareTo(SetEvent other) {
                long diff = this.when - other.when; // if overflow occurs here, it is OK
                return diff < 0 ? -1 : diff > 0 ? 1 : 0;
            }
        }

        private long curTime = 0;
        private PriorityQueue<SetEvent> events = new PriorityQueue<SetEvent>();
        private HashSet<Subnet> dirtyNets = new HashSet<Subnet>();
        private HashSet<Instance> dirtyInstances = new HashSet<Instance>();
        private bool anyDirty = false;

        public Engine() {
        }

        public bool IsStepPending() {
            return anyDirty;
        }

        public void QueueSet(Port port, Value value, int delay) {
            if (delay <= 0) {
                throw new ArgumentException("delay must be at least 1");
            }
            events.Enqueue(new SetEvent(port, value, curTime + delay));
            anyDirty = true;
        }

        public void AddDirtyInstance(Instance instance) {
            dirtyInstances.Add(instance);
            anyDirty = true;
            if (Debug) {
                Console.WriteLine("  marked instance {0} [{1}]", instance, instance.GetType().Name);
            }
        }

        public void AddDirtyNet(Subnet net) {
            dirtyNets.Add(net);
            anyDirty = true;
            if (Debug) {
                Console.WriteLine("  marked net {0}", net);
            }
        }

        public void Step(SimulationModel.Key key, ISimulationAccess access) {
            long now = curTime + 1;
            curTime = now;
            if (Debug) {
                Console.WriteLine("step simulation {0}", now);
            }

            ClearDirty(key, access);

            while (events.Count > 0 && events.Peek().when == now) {
                SetEvent evnt = events.Dequeue();
                Port p = evnt.port;
                Value cur = p.GetDrivenValue(key);
                if (Debug) {
                    Console.WriteLine("  {0}: drive {1} onto {2}, mark {3}", evnt.when, evnt.value, evnt.port, p.Subnet);
                }
                if (!cur.Equals(evnt.value)) {
                    p.SetDrivenValue(key, evnt.value);
                    dirtyNets.Add(p.Subnet);
                }
            }

            ClearDirty(key, access);

            anyDirty = events.Count > 0 || dirtyNets.Count > 0 || dirtyInstances.Count > 0;
        }

        private void ClearDirty(SimulationModel.Key key, ISimulationAccess access) {
            HashSet<Subnet> nets = dirtyNets;
            HashSet<Instance> instances = dirtyInstances;
            if (nets != null && nets.Count > 0) {
                dirtyNets = new HashSet<Subnet>();
                foreach (Subnet net in nets) {
                    if (Debug) {
                        Console.Write("  net {0} /", net);
                    }
                    Value v = null;
                    foreach (Port p in net.Drivers) {
                        Value u = p.GetDrivenValue(key);
                        if (Debug) {
                            Console.Write("{0}/", u);
                        }
                        v = v == null ? u : v.Resolve(u);
                    }
                    if (Debug) {
                        Console.WriteLine("-> {0}", v);
                    }
                    if (v == null) {
                        int w = net.Width;
                        v = Value.Create(Value.U, w == 0 ? 1 : w);
                    }
                    net.SetValue(key, v);
                    foreach (Port p in net.Readers) {
                        instances.Add(p.Instance);
                    }
                }
            }

            if (instances != null && instances.Count > 0) {
                InstanceEvent evnt = new InstanceEvent(InstanceEvent.Types.InstanceDirty);
                InstanceState iState = new InstanceState(access, null);
                dirtyInstances = new HashSet<Instance>();
                foreach (Instance instance in instances) {
                    if (Debug) {
                        Console.WriteLine("  propagate instance {0}", instance);
                    }
                    iState.Instance = instance;
                    instance.HandleEvent(evnt, iState);
                }
            }
        }
    }
}

