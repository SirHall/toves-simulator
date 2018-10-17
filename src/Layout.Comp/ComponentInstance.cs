/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Sim.Inst;

namespace Toves.Layout.Comp {
    public class ComponentInstance : Instance {
        public ComponentInstance(Component component) : base(component.PortArgs) {
            this.Component = component;
        }

        public Component Component { get; private set; }

        public void OnComponentModified(ComponentModifiedArgs evnt) {
            ComponentModifiedArgs.Types type = evnt.Type;
            if (type == ComponentModifiedArgs.Types.PortsChanged) {
                this.UpdatePorts(Component.PortArgs);
            }
        }

        public override sealed void HandleEvent(InstanceEvent evnt, IInstanceState state) {
            InstanceEvent.Types type = evnt.Type;
            if (type == InstanceEvent.Types.InstanceDirty) {
                Component.Propagate(state);
            } else {
                switch (type) {
                case InstanceEvent.Types.InstanceAdded:
                    Component.SetInstanceLive(this, true);
                    break;
                case InstanceEvent.Types.InstanceRemoved:
                    Component.SetInstanceLive(this, false);
                    break;
                }
                HandleEventHook(evnt, state);
            }
        }

        protected virtual void HandleEventHook(InstanceEvent evnt, IInstanceState state) { }

        public override string ToString() {
            string typeName;
            if (this.GetType() == typeof(ComponentInstance)) {
                typeName = "";
            } else {
                typeName = string.Format("[{0}]", this.GetType().Name);
            }
            return string.Format("inst:{0}{1}", Component.GetType().Name, typeName);
        }
    }
}