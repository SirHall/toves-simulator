/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections;
using System.Collections.Generic;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Proj.Module;
using Toves.Sim.Inst;
using Toves.Util.Transaction;

namespace Toves.Layout.Model {
    public class LayoutKey {
        internal LayoutKey() { }
    }

    public class LayoutModel : Resource<ILayoutAccess>, IImplementation {
        private ProjectModule module = null;
        internal List<Component> components = new List<Component>();
        internal List<WireSegment> wires = new List<WireSegment>();
        internal LayoutConnections connections;
        internal LayoutNode.Updater nodes = new LayoutNode.Updater();
        private ResourceHelper resourceHelper = new ResourceHelper();

        public LayoutModel() {
            connections = new LayoutConnections();
        }

        public void UpdateModule(ProjectModule value) {
            if (this.module != null) {
                throw new InvalidOperationException("cannot update module after it is set");
            }
            this.module = value;
        }
        
        public IEnumerable<Connection> Connections { get { return connections.Connections; } }

        public ResourceHelper ResourceHelper { get { return resourceHelper; } }

        public event EventHandler<LayoutModifiedArgs> LayoutModifiedEvent;

        protected virtual void OnLayoutModified(ILayoutAccess access) {
            EventHandler<LayoutModifiedArgs> handler = LayoutModifiedEvent;
            if (handler != null) {
                handler(this, new LayoutModifiedArgs(access));
            }
        }

        internal void FireOnComponentModified(object sender, ComponentModifiedArgs evnt) {
            if (evnt.Type == ComponentModifiedArgs.Types.PortsChanged) {
                Transaction xn = new Transaction();
                ILayoutAccess layout = xn.RequestReadAccess(this);
                using (xn.Start()) {
                    nodes.LayoutChanged(layout);
                    OnLayoutModified(layout);
                }
            }
        }
        
        public ILayoutAccess CreateAccess(bool canWrite) {
            return new MyAccess(this, canWrite);
        }

        public void Hook(ResourceHookType hookType, IResourceAccess rawAccess) {
            ILayoutAccess access = rawAccess as ILayoutAccess;
            if (hookType == ResourceHookType.AfterWrite) {
                nodes.LayoutChanged(access);
            } else if (hookType == ResourceHookType.AfterDowngrade) {
                IEnumerable<Connection> conns = connections.UpdateConnections(access);
                OnLayoutModified(access);
                ProjectModule mod = this.module;
                if (mod != null && conns != null) {
                    // done after OnLayoutModified since that will add pin instances into simulation substates
                    mod.UpdateConnections(conns);
                }
            }
        }

        public Instance CreateInstance(ModuleComponent component) {
            return new LayoutInstance(component, this);
        }

        public void Propagate(IInstanceState state) {
        }

        public IEnumerable<ModuleComponent> GetModuleComponents() {
            List<ModuleComponent> result = new List<ModuleComponent>();
            Transaction xn = new Transaction();
            ILayoutAccess layout = xn.RequestReadAccess(this);
            using (xn.Start()) {
                foreach (Component c in layout.Components) {
                    if (c is ModuleComponent) {
                        result.Add((ModuleComponent) c);
                    }
                }
            }
            return result;
        }
    }

    internal class MyAccess : ResourceAccess, ILayoutAccess {
        private LayoutModel layout;
        private LayoutKey key = new LayoutKey();

        internal MyAccess(LayoutModel layout, bool canWrite) : base(layout, canWrite) {
            this.layout = layout;
        }

        public LayoutModel Layout {
            get { return layout; }
        }

        public IEnumerable<Component> Components {
            get {
                CheckReadAccess();
                return layout.components.AsReadOnly();
            }
        }

        public IEnumerable<WireSegment> Wires {
            get {
                CheckReadAccess();
                return layout.wires.AsReadOnly();
            }
        }
        
        public IEnumerable<LayoutNode> Nodes {
            get {
                CheckReadAccess();
                return layout.nodes.Nodes;
            }
        }
        
        public IEnumerable<Component> Pins {
            get {
                CheckReadAccess();
                return layout.connections.Pins;
            }
        }

        public LayoutNode FindNode(Location loc) {
            CheckReadAccess();
            return layout.nodes.GetNode(loc);
        }

        public Component AddComponent(Component prototype, int x, int y) {
            CheckWriteAccess();
            Component clone = prototype.Clone();
            clone.SetLocation(this, new Location(x, y));
            layout.components.Add(clone);
            clone.ComponentModifiedEvent += layout.FireOnComponentModified;
            return clone;
        }

        public void RemoveComponent(Component component) {
            CheckWriteAccess();
            component.ComponentModifiedEvent -= layout.FireOnComponentModified;
            layout.components.Remove(component);
        }

        public void MoveComponent(Component component, int dx, int dy) {
            CheckWriteAccess();
            Location curLoc = component.GetLocation(this);
            component.SetLocation(this, curLoc.Translate(dx, dy));
        }

        public void AddWire(Location end0, Location end1) {
            CheckWriteAccess();
            layout.wires.Add(new WireSegment(end0, end1));
        }

        public void RemoveWire(WireSegment wire) {
            CheckWriteAccess();
            layout.wires.Remove(wire);
        }
    }
}