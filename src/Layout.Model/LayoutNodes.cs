/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using System.Text;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Util.Collections;

namespace Toves.Layout.Model {
    public class LayoutNodes {
        public class Node {
            internal int id;
            internal LocationSet prev = null;
            internal LocationSet locs;

            internal Node(int id, LocationSet locs)
            {
                this.id = id;
                this.locs = locs;
            }
        }

        internal class LocationSet : IEnumerable<Location> {
            private int hashCode;
            private List<Location> locs;

            public LocationSet(IEnumerable<Location> locs) {
                this.locs = new List<Location>(locs);
                this.locs.Sort(Location.XComparer);
                int hash = 0;
                foreach (Location loc in locs) {
                    hash += loc.GetHashCode();
                }
                this.hashCode = hash;
            }

            public int Count {
                get { return locs.Count; }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
                return locs.GetEnumerator();
            }

            public IEnumerator<Location> GetEnumerator() {
                return locs.GetEnumerator();
            }

            public override int GetHashCode() {
                return hashCode;
            }
            
            public override bool Equals(object right) {
                if (right is LocationSet) {
                    return this.Equals((LocationSet) right);
                } else {
                    return false;
                }
            }

            public bool Equals(LocationSet right) {
                if (this == right) {
                    return true;
                } else if (this.hashCode != right.hashCode) {
                    return false;
                } else if (this.locs.Count != right.locs.Count) {
                    return false;
                } else {
                    int n = this.locs.Count;
                    for (int i = 0; i < n; i++) {
                        if (!this.locs[i].Equals(right.locs[i])) {
                            return false;
                        }
                    }
                    return true;
                }
            }

            public override string ToString() {
                StringBuilder result = new StringBuilder();
                bool first = true;
                foreach (Location loc in locs) {
                    if (!first) {
                        result.Append("+");
                    }
                    first = false;
                    result.Append(loc.ToString());
                }
                return result.ToString();
            }
        }

        private List<Node> curNodes = new List<Node>();
        private Dictionary<Location, Node> locNodes = new Dictionary<Location, Node>();
        private int nodeId = -1;

        public LayoutNodes() {
        }

        public void LayoutChanged(ILayoutAccess layout) {
            List<Node> newNodes = new List<Node>();

            IEnumerable<LocationSet> curSetNodes = GetCurrentLocationSets(layout);
            List<Node> changedNodes = DetermineNodes(curSetNodes, newNodes);
            Dictionary<Location, Node> locNodes = new Dictionary<Location, Node>();
            foreach (Node n in newNodes) {
                foreach (Location loc in n.locs) {
                    locNodes[loc] = n;
                }
            }
            this.curNodes = newNodes;
            this.locNodes = locNodes;
            if (changedNodes.Count == 0) {
                Console.Error.WriteLine("no changes");
            } else {
                Console.Error.WriteLine("changes:");
                foreach (Node n in changedNodes) {
                    Console.Error.WriteLine("  {0}: {1}->{2}", n.id, n.prev, n.locs);
                }
            }
        }

        private IEnumerable<LocationSet> GetCurrentLocationSets(ILayoutAccess layout) {
            HashSet<Location> locs = new HashSet<Location>();
            foreach (WireSegment w in layout.Wires) {
                locs.Add(w.End0);
                locs.Add(w.End1);
            }
            foreach (Component c in layout.Components) {
                Location cloc = c.GetLocation(layout);
                foreach (ConnectionPoint p in c.Connections) {
                    locs.Add(cloc.Translate(p.Dx, p.Dy));
                }
            }

            UnionFind<Location> allNodes = new UnionFind<Location>(locs);
            foreach (WireSegment w in layout.Wires) {
                UnionFindNode<Location> e0 = allNodes[w.End0];
                UnionFindNode<Location> e1 = allNodes[w.End1];
                e0.Unite(e1);
            }

            IEnumerable<UnionFindNode<Location>> roots = allNodes.Roots;
            List<LocationSet> result = new List<LocationSet>();
            foreach (UnionFindNode<Location> root in roots) {
                IEnumerable<UnionFindNode<Location>> rootMembers = root.GetSetMembers();
                List<Location> setMembers = new List<Location>();
                foreach (UnionFindNode<Location> n in rootMembers) {
                    setMembers.Add(n.Value);
                }
                result.Add(new LocationSet(setMembers));
            }
            return result;
        }

        private List<Node> DetermineNodes(IEnumerable<LocationSet> newSets, List<Node> newNodes) {
            Dictionary<LocationSet, Node> oldSets = new Dictionary<LocationSet, Node>();
            foreach (Node n in this.curNodes) {
                oldSets[n.locs] = n;
            }
            HashSet<LocationSet> unmatched = new HashSet<LocationSet>();
            foreach (LocationSet newSet in newSets) {
                if (oldSets.ContainsKey(newSet)) {
                    newNodes.Add(oldSets[newSet]);
                    oldSets.Remove(newSet);
                } else {
                    unmatched.Add(newSet);
                }
            }

            if (oldSets.Count == 0 && unmatched.Count == 0) {
                return new List<Node>(0);
            }

            List<Node> changedNodes = new List<Node>(oldSets.Values);
            List<LocationSet> toReplace = new List<LocationSet>(oldSets.Keys);
            toReplace.Sort((a, b) => b.Count - a.Count);
            foreach (LocationSet replSrc in toReplace) {
                Node replNode = oldSets[replSrc];
                HashSet<Location> replLocs = new HashSet<Location>(replSrc);
                int maxCount = 0;
                LocationSet maxSet = null;
                foreach (LocationSet candidate in unmatched) {
                    int count = 0;
                    foreach (Location loc in candidate) {
                        if (replLocs.Contains(loc)) {
                            ++count;
                        }
                    }
                    if (count > maxCount) {
                        maxCount = count;
                        maxSet = candidate;
                    }
                }
                replNode.prev = replNode.locs;
                replNode.locs = maxSet;
                if (maxSet != null) {
                    newNodes.Add(replNode);
                    unmatched.Remove(maxSet);
                }
            }

            foreach (LocationSet newSet in unmatched) {
                this.nodeId++;
                Node n = new Node(this.nodeId, newSet);
                changedNodes.Add(n);
                newNodes.Add(n);
            }
            return changedNodes;
        }
    }
}

