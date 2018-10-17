/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Layout.Comp;

namespace Toves.Proj.Module {
    public class ProjectModule {
        private object key;
        private String name;
        private List<WeakReference> components = new List<WeakReference>();

        internal ProjectModule(object key, String name, IImplementation implementation) {
            if (key == null) {
                throw new InvalidOperationException("key required");
            }
            this.key = key;
            this.name = name;
            this.Implementation = implementation;
            this.Presentation = new DefaultPresentation(this);
            implementation.UpdateModule(this);
        }

        public IPresentation Presentation { get; private set; }

        public IImplementation Implementation { get; private set; }

        internal void AddComponent(ModuleComponent value) {
            components.Add(new WeakReference(value));
        }

        private IEnumerable<ModuleComponent> ModuleComponents {
            get {
                for (int i = components.Count - 1; i >= 0; i--) {
                    ModuleComponent comp = components[i].Target as ModuleComponent;
                    if (comp == null) {
                        components.RemoveAt(i);
                    } else {
                        yield return comp;
                    }
                }
            }
        }

        public void UpdateConnections(IEnumerable<Connection> connections) {
            IEnumerable<ConnectionPoint> conns = Presentation.UpdateConnections(connections);
            foreach (ModuleComponent comp in ModuleComponents) {
                comp.Connections = conns;
            }
        }
        
        public bool HasDescendent(IImplementation query) {
            if (query == this.Implementation) {
                return true;
            }
            Dictionary<ProjectModule, bool> visited = new Dictionary<ProjectModule, bool>();
            Queue<ProjectModule> toVisit = new Queue<ProjectModule>();
            toVisit.Enqueue(this);
            visited[this] = true;
            while (toVisit.Count > 0) {
                ProjectModule cur = toVisit.Dequeue();
                foreach (ModuleComponent comp in cur.Implementation.GetModuleComponents()) {
                    ProjectModule sub = comp.Module;
                    if (!visited.ContainsKey(sub)) {
                        if (sub.Implementation == query) {
                            return true;
                        }
                        visited[sub] = true;
                        toVisit.Enqueue(sub);
                    }
                }
            }
            return false;
        }

        internal string GetName(object key) {
            if (this.key != key) {
                throw new InvalidOperationException("unrecognized key");
            }
            return this.name;
        }
        
        internal void SetName(object key, string value) {
            if (this.key != key) {
                throw new InvalidOperationException("unrecognized key");
            }
            this.name = value;
        }
    }
}

