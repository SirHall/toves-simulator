/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;

namespace Toves.Util.Collections {
    public class UnionFindNode<T> {
        private HashSet<UnionFindNode<T>> allRoots;
        private UnionFindNode<T> parent = null;
        private List<UnionFindNode<T>> subNodes = null;

        internal UnionFindNode(HashSet<UnionFindNode<T>> allRoots, T value) {
            this.allRoots = allRoots;
            this.Value = value;
        }

        public T Value { get; private set; }

        private UnionFindNode<T> Find() {
            UnionFindNode<T> ret = this.parent;
            if (ret == null) {
                return this;
            } else {
                UnionFindNode<T> next = ret.parent;
                while (next != null) {
                    ret = next;
                    next = ret.parent;
                }
                this.parent = ret;
                return ret;
            }
        }

        public bool IsInSameSet(UnionFindNode<T> other) {
            return this.Find() == other.Find();
        }

        public IEnumerable<UnionFindNode<T>> GetSetMembers() {
            UnionFindNode<T> a = this.Find();
            if (a.subNodes == null) {
                List<UnionFindNode<T>> result = new List<UnionFindNode<T>>();
                result.Add(a);
                a.subNodes = result;
                return result;
            } else {
                return a.subNodes;
            }
        }

        public void Unite(UnionFindNode<T> other) {
            UnionFindNode<T> a = this.Find();
            UnionFindNode<T> b = other.Find();
            if (a != b) {
                // need to unite a and b - a will be parent of b
                int aSize = a.subNodes == null ? 1 : a.subNodes.Count;
                int bSize = b.subNodes == null ? 1 : b.subNodes.Count;
                if (aSize < bSize) { // a is smaller, so it should be child
                    UnionFindNode<T> t = a;
                    a = b;
                    b = t;
                }
                b.parent = a;
                if (a.subNodes == null) {
                    a.subNodes = new List<UnionFindNode<T>>();
                    a.subNodes.Add(a);
                }
                if (b.subNodes == null) {
                    a.subNodes.Add(b);
                } else {
                    foreach (UnionFindNode<T> bn in b.subNodes) {
                        a.subNodes.Add(bn);
                    }
                    b.subNodes = null;
                }
                allRoots.Remove(b);
            }
        }
    }
}