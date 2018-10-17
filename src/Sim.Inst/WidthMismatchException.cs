/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Sim.Inst {
    public class WidthMismatchException : Exception {
        public static void Check(int left, int right) {
            if (left != right) {
                String m = string.Format("widths must match: left {0}, right {1}",
                                         left, right);
                throw new WidthMismatchException(m);
            }
        }

        public WidthMismatchException(String msg) : base(msg) {
        }

        public WidthMismatchException() : this("widths must match") {
        }
    }
}

