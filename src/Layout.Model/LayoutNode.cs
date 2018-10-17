/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using System.Text;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Util.Collections;

namespace Toves.Layout.Model
{
    public class LayoutNode
    {
        private static readonly bool Debug = false;

        public class Updater {
            private List<LayoutNode> curNodes = new List<LayoutNode>();
            private Dictionary<Location, LayoutNode> locNodes = new Dictionary<Location, LayoutNode>();
            private int nodeId = -1;

            public Updater()
            {
            }

            public IEnumerable<LayoutNode> Nodes {
                get { return curNodes.AsReadOnly(); }
            }

            public LayoutNode GetNode(Location query) {
                return locNodes[query];
            }

            public void LayoutChanged(ILayoutAccess layout)
            {
                List<LayoutNode> newNodes = new List<LayoutNode>();

                IEnumerable<LocationSet> curSetNodes = getCurrentLocationSets(layout);
                List<LayoutNode> changedNodes = determineNodes(curSetNodes, newNodes);
                Dictionary<Location, LayoutNode> locNodes = new Dictionary<Location, LayoutNode>();
                foreach (LayoutNode n in newNodes) {
                    foreach (Location loc in n.locs) {
                        locNodes[loc] = n;
                    }
                }
                this.curNodes = newNodes;
                this.locNodes = locNodes;
                if (Debug) {
                    if (changedNodes.Count == 0) {
                        Console.Error.WriteLine("no changes");
                    } else {
                        Console.Error.WriteLine("changes:");
                        foreach (LayoutNode n in changedNodes) {
                            Console.Error.WriteLine("  {0}: {1}->{2}", n.id, n.prev, n.locs);
                        }
                    }
                }
            }

            private IEnumerable<LocationSet> getCurrentLocationSets(ILayoutAccess layout) {
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

            private List<LayoutNode> determineNodes(IEnumerable<LocationSet> newSets, List<LayoutNode> newNodes)
            {
                Dictionary<LocationSet, LayoutNode> oldSets = new Dictionary<LocationSet, LayoutNode>();
                foreach (LayoutNode n in this.curNodes) {
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
                    return new List<LayoutNode>(0);
                }

                List<LayoutNode> changedNodes = new List<LayoutNode>(oldSets.Values);
                List<LocationSet> toReplace = new List<LocationSet>(oldSets.Keys);
                toReplace.Sort((a, b) => b.Count - a.Count);
                foreach (LocationSet replSrc in toReplace) {
                    LayoutNode replNode = oldSets[replSrc];
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
                    LayoutNode n = new LayoutNode(this.nodeId, newSet);
                    changedNodes.Add(n);
                    newNodes.Add(n);
                }
                return changedNodes;
            }
        }

        internal class LocationSet : IEnumerable<Location>
        {
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

            public override bool Equals(object right)
            {
                if (right is LocationSet) {
                    return this.Equals((LocationSet) right);
                } else {
                    return false;
                }
            }

            public bool Equals(LocationSet right)
            {
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

            public override string ToString()
            {
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

        internal int id;
        internal LocationSet prev = null;
        internal LocationSet locs;

        internal LayoutNode(int id, LocationSet locs)
        {
            this.id = id;
            this.locs = locs;
        }

        public override string ToString()
        {
            return String.Format("lnode{0}", id);
        }
    }
}

