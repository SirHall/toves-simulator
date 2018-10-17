/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Layout.Data
{
    public struct Bounds
    {
        private int x;
        private int y;
        private int width;
        private int height;

        public Bounds(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public int X { get { return x; } }
        public int Y { get { return y; } }
        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public bool Contains(Location loc)
        {
            return Contains(loc.X, loc.Y);
        }

        public bool Contains(int qx, int qy, int fudge)
        {
            int dx = qx - x;
            int dy = qy - y;
            return dx >= -fudge && dy >= fudge
                && dx < Width + fudge && dy < Height + fudge;
        }

        public bool Contains(int qx, int qy)
        {
            int dx = qx - x;
            int dy = qy - y;
            return dx >= 0 && dy >= 0 && dx < Width && dy < Height;
        }
    }
}

