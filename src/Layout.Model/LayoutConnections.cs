/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Layout.Wiring;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Layout.Model;
using Toves.Proj.Module;
using Toves.Util.Transaction;

namespace Toves.Layout.Model {
    public class LayoutConnections {
        private List<Pin> inputs = new List<Pin>();
        private List<PinOut> outputs = new List<PinOut>();

        public LayoutConnections() {
            this.Connections = new List<Connection>();
        }

        public IEnumerable<Component> Pins {
            get {
                foreach (Pin p in inputs) {
                    yield return p;
                }
                foreach (PinOut p in outputs) {
                    yield return p;
                }
            }
        }

        public IEnumerable<Connection> Connections { get; private set; }

        internal IEnumerable<Connection> UpdateConnections(ILayoutAccess layout) {
            List<Pin> newInputs = new List<Pin>();
            List<PinOut> newOutputs = new List<PinOut>();
            foreach (Component comp in layout.Components) {
                if (comp is Pin) {
                    newInputs.Add((Pin) comp);
                } else if (comp is PinOut) {
                    newOutputs.Add((PinOut) comp);
                }
            }
            newInputs.Sort((p0, p1) => Location.YComparer.Compare(p0.GetLocation(layout), p1.GetLocation(layout)));
            newOutputs.Sort((p0, p1) => Location.YComparer.Compare(p0.GetLocation(layout), p1.GetLocation(layout)));
            List<Connection> conns = new List<Connection>();
            foreach (Pin p in newInputs) {
                conns.Add(new Connection(true));
            }
            foreach (PinOut p in newOutputs) {
                conns.Add(new Connection(false));
            }

            inputs = newInputs;
            outputs = newOutputs;
            this.Connections = conns;
            return conns;
        }
    }
}