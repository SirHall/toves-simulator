/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Sim.Model {
    public class Link {
        private Node source;
        private Node destination;

        public Link(Node source, Node destination) {
            this.source = source;
            this.destination = destination;
        }

        public Node Source { get { return source; } }

        public Node Destination { get { return destination; } }

        public override int GetHashCode() {
            return source.GetHashCode() * 31 + destination.GetHashCode();
        }
        
        public bool Equals(Link obj) {
            return this.source == obj.source && this.destination == obj.destination;
        }
        
        public override bool Equals(object obj) {
            if (obj is Link) {
                return this.Equals((Link) obj);
            } else {
                return false;
            }
        }

        public override string ToString() {
            return string.Format("{0}-{1}", source, destination);
        }
    }
}