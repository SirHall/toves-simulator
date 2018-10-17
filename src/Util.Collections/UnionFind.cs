/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;

namespace Toves.Util.Collections {
    public class UnionFind<T> {
        private List<UnionFindNode<T>> nodes = new List<UnionFindNode<T>>();
        private Dictionary<T, UnionFindNode<T>> nodeMap = new Dictionary<T, UnionFindNode<T>>();
        private HashSet<UnionFindNode<T>> roots = new HashSet<UnionFindNode<T>>();

        public UnionFind(IEnumerable<T> values) {
            foreach (T value in values) {
                UnionFindNode<T> n = new UnionFindNode<T>(roots, value);
                nodes.Add(n);
                nodeMap[value] = n;
                roots.Add(n);
            }
        }

        public IEnumerable<UnionFindNode<T>> Nodes {
            get { return nodes; }
        }
        
        public IEnumerable<UnionFindNode<T>> Roots {
            get { return roots; }
        }

        public UnionFindNode<T> this[T value] {
            get { return nodeMap[value]; }
        }
    }
}