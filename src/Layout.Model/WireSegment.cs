/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using System.Linq;

using Toves.Layout.Data;

namespace Toves.Layout.Model
{
    public class WireSegment
    {
        public WireSegment(Location src, Location dst) {
            if (src.X < dst.X || src.X == dst.X && src.Y < dst.Y) {
                End0 = src;
                End1 = dst;
            } else {
                End0 = dst;
                End1 = src;
            }
        }

        public Location End0 { get; private set; }

        public Location End1 { get; private set; }

        public override string ToString() {
            return string.Format("wire{0}-{1}", End0, End1);
        }

        public bool IsOnWire(Location query) {
            if (End0.X == End1.X) {
                return query.X == End0.X && End0.Y <= query.Y && query.Y <= End1.Y;
            } else {
                return query.Y == End0.Y && End0.X <= query.X && query.X <= End1.X;
            }
        }

        public bool IsCollinear(WireSegment other, int stepSize) {
            Location[] locs = { this.End0, this.End1, other.End0, other.End1 };
            int xMin = Math.Min(this.End0.X, other.End0.X);
            int xMax = Math.Max(this.End1.X, other.End1.X);
            int yMin = Math.Min(Math.Min(this.End0.Y, this.End1.Y), Math.Min(other.End0.Y, other.End1.Y));
            int yMax = Math.Max(Math.Max(this.End0.Y, this.End1.Y), Math.Max(other.End0.Y, other.End1.Y));
            bool sortByX = xMax - xMin >= yMax - yMin;
            if (sortByX) {
                locs = locs.OrderBy(loc => loc.X).ToArray();
            } else {
                locs = locs.OrderBy(loc => loc.Y).ToArray();
            }
            double tolerate = 0.5 * stepSize;
            if (locs[1] != locs[0] && !IsBetween(locs[1], locs[0], locs[3], sortByX, tolerate)) {
                return false;
            } else if (locs[2] != locs[3] && !IsBetween(locs[2], locs[0], locs[3], sortByX, tolerate)) {
                return false;
            } else {
                return true;
            }
        }

        private bool IsBetween(Location q, Location p0, Location p1, bool sortByX, double tolerate) {
            double dx = p1.X - p0.X;
            double dy = p1.Y - p0.Y;
            double error;
            if (sortByX) {
                error = (p0.Y + (dy / dx) * (q.X - p0.X)) - q.Y;
            } else {
                error = (p0.X + (dx / dy) * (q.Y - p0.Y)) - q.X;
            }
            return error >= -tolerate && error <= tolerate;
        }

        public IEnumerable<Location> GetLocationsOnWire(int stepSize, bool includeEndpoints) {
            if (includeEndpoints) {
                yield return End0;
            }
            if (End0.X == End1.X) {
                for (int y = End0.Y + stepSize; y < End1.Y; y += stepSize) {
                    yield return new Location(End0.X, y);
                }
            } else if (End0.Y == End1.Y) {
                for (int x = End0.X + stepSize; x < End1.X; x += stepSize) {
                    yield return new Location(x, End0.Y);
                }
            } else {
                int x = End0.X;
                int y = End0.Y;
                int x1 = End1.X;
                int y1 = End1.Y;
                int dx = stepSize * (x1 - x);
                int dy = stepSize * Math.Abs(y1 - y);
                int sx = stepSize;
                int sy = y < y1 ? stepSize : -stepSize;
                int err = dx - dy;
                while (true) {
                    int e2 = 2 * err;
                    if (e2 > -dy) {
                        err -= dy;
                        x += sx;
                    }
                    if (x == x1 && y == y1) {
                        break;
                    }
                    if (e2 < dx) {
                        err += dx;
                        y += sy;
                    }
                    if (x == x1 && y == y1) {
                        break;
                    }
                    yield return new Location(x, y);
                }
            }
            if (includeEndpoints) {
                yield return End1;
            }
        }
    }
}

