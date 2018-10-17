/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Toves.Layout.Data;
using Toves.Sim.Inst;
using Toves.Util.Transaction;

namespace Toves.Layout.Comp {
    public abstract class Component {
        private static readonly ConnectionPoint[] EmptyConnections = new ConnectionPoint[0];

        private Location location;
        private ConnectionPoint[] connections = EmptyConnections;
        private ConcurrentDictionary<ComponentInstance, bool> instances = new ConcurrentDictionary<ComponentInstance, bool>();

        public abstract string Name { get; }

        public event EventHandler<ComponentModifiedArgs> ComponentModifiedEvent;

        public Location GetLocation(IResourceAccess access) {
            access.CheckReadAccess();
            return this.location;
        }

        public void SetLocation(IResourceAccess access, Location value) {
            access.CheckWriteAccess();
            this.location = value;
        }

        public IEnumerable<PortArgs> PortArgs {
            get {
                ConnectionPoint[] conns = connections;
                foreach (ConnectionPoint p in conns) {
                    yield return p;
                }
            }
        }

        public IEnumerable<ConnectionPoint> Connections {
            get {
                ConnectionPoint[] conns = connections;
                foreach (ConnectionPoint p in conns) {
                    yield return p;
                }
            }
            set {
                if (value == null) {
                    this.connections = EmptyConnections;
                } else {
                    List<ConnectionPoint> conns = new List<ConnectionPoint>();
                    foreach (ConnectionPoint p in value) {
                        conns.Add(p);
                    }
                    this.connections = conns.ToArray();
                }
                DispatchModifiedEvent(ComponentModifiedArgs.Types.PortsChanged);
            }
        }

        protected void DispatchModifiedEvent(ComponentModifiedArgs.Types type) {
            ComponentModifiedArgs evnt = null;
            foreach (ComponentInstance i in instances.Keys) {
                if (evnt == null) {
                    evnt = new ComponentModifiedArgs(type);
                }
                i.OnComponentModified(evnt);
            }
            EventHandler<ComponentModifiedArgs> handler = ComponentModifiedEvent;
            if (handler != null) {
                if (evnt == null) {
                    evnt = new ComponentModifiedArgs(type);
                }
                handler(this, evnt);
            }
        }

        public abstract Bounds OffsetBounds { get; }

        public abstract bool ShouldSnap { get; }

        public abstract bool Contains(int offsetX, int offsetY);

        public virtual Component Clone() {
            return (this.MemberwiseClone() as Component);
        }

        public virtual Instance CreateInstance() {
            return new ComponentInstance(this);
        }

        public void SetInstanceLive(ComponentInstance instance, bool isLive) {
            if (isLive) {
                instances.TryAdd(instance, true);
            } else {
                bool oldValue;
                instances.TryRemove(instance, out oldValue);
            }
        }

        public abstract void Propagate(IInstanceState state);

        public abstract void Paint(IComponentPainter painter);
    }
}