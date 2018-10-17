/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Toves.Sim.Model {
    public class Node {
        private static List<Link> EmptyLinks = new List<Link>();
        private static ReadOnlyCollection<Link> EmptyLinksView = EmptyLinks.AsReadOnly();
        private static int lastIdAllocated = -1;

        private int id;
        private List<Link> links = EmptyLinks;
        private ReadOnlyCollection<Link> linksView = EmptyLinksView;
        private int baseUseCount = 0;

        public Node() {
            int id = lastIdAllocated + 1;
            this.id = id;
            lastIdAllocated = id;
        }

        public override string ToString() {
            return string.Format("node{0}", id);
        }

        public ReadOnlyCollection<Link> Links { get { return linksView; } }

        public IEnumerable<Node> Neighbors {
            get {
                foreach (Link link in Links) {
                    if (link.Source == this) {
                        yield return link.Destination;
                    } else {
                        yield return link.Source;
                    }
                }
            }
        }

        public int UseCount { get { return links.Count + baseUseCount; } }

        public Subnet Subnet { get; private set; }

        public Subnet TempSubnet { get; private set; }

        public void AddToUseCount(SimulationModel.Key key, int delta) {
            if (key == null) {
                throw new InvalidOperationException("need key");
            } else {
                int newVal = baseUseCount + delta;
                if (newVal < 0) {
                    baseUseCount = 0;
                    throw new ArgumentException("count cannot go negative");
                } else {
                    baseUseCount = newVal;
                }
            }
        }

        public void AddLink(SimulationModel.Key key, Link value) {
            if (key == null) {
                throw new InvalidOperationException("need key");
            } else {
                List<Link> links = this.links;
                if (links == EmptyLinks) {
                    links = new List<Link>();
                    this.links = links;
                    this.linksView = links.AsReadOnly();
                }
                links.Add(value);
            }
        }

        public bool RemoveLink(SimulationModel.Key key, Link value) {
            if (key == null) {
                throw new InvalidOperationException("need key");
            } else {
                List<Link> links = this.links;
                bool removed = links.Remove(value);
                if (removed) {
                    if (links.Count == 0) {
                        this.links = EmptyLinks;
                        this.linksView = EmptyLinksView;
                    }
                }
                return removed;
            }
        }

        public void SetSubnet(SimulationModel.Key key, Subnet value) {
            if (key == null) {
                throw new InvalidOperationException("need key");
            } else {
                this.Subnet = value;
            }
        }
        
        public void SetTempSubnet(SimulationModel.Key key, Subnet value) {
            if (key == null) {
                throw new InvalidOperationException("need key");
            } else {
                this.TempSubnet = value;
            }
        }
    }
}

