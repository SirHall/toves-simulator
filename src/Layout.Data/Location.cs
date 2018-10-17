/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;

namespace Toves.Layout.Data {
    public struct Location {
        private class XComparerImpl : IComparer<Location> {
            public int Compare(Location a, Location b) {
                int x = a.x.CompareTo(b.x);
                if (x != 0) {
                    return x;
                }
                int y = a.y.CompareTo(b.y);
                return y;
            }
        }

        private class YComparerImpl : IComparer<Location> {
            public int Compare(Location a, Location b) {
                int y = a.y.CompareTo(b.y);
                if (y != 0) {
                    return y;
                }
                int x = a.x.CompareTo(b.x);
                return x;
            }
        }

        public static readonly IComparer<Location> XComparer = new XComparerImpl();

        public static readonly IComparer<Location> YComparer = new YComparerImpl();

        private int x;
        private int y;

        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X { get { return x; } }

        public int Y { get { return y; } }

        public Location Translate(int dx, int dy)
        {
            if (dx == 0 && dy == 0) {
                return this;
            } else {
                return new Location(x + dx, y + dy);
            }
        }

        public int GetDistance2(Location query)
        {
            int dx = this.x - query.x;
            int dy = this.y - query.y;
            return dx * dx + dy * dy;
        }

        public bool InRectangle(int x0, int y0, int width, int height)
        {
            int dx = this.x - x0;
            int dy = this.y - y0;
            return dx >= 0 && dy >= 0 && dx < width && dy < height;
        }

        public bool InCircle(int cx, int cy, int r)
        {
            int dx = this.x - cx;
            int dy = this.y - cy;
            return dx >= -r && dx < r && dy >= -r && dy < r
                && dx * dx + dy * dy < r * r;
        }


        public override string ToString()
        {
            return this.x + "," + this.y;
        }

        public override bool Equals(object right)
        {
            if (right is Location) {
                Location that = (Location) right;
                return this.x == that.x && this.y == that.y;
            } else {
                return false;
            }
        }

        public bool Equals(Location right)
        {
            return this.x == right.x && this.y == right.y;
        }

        public override int GetHashCode()
        {
            return this.x * 31 + this.y;
        }

        public static bool operator==(Location a, Location b) {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator!=(Location a, Location b) {
            return a.x != b.x || a.y != b.y;
        }
    }
}

