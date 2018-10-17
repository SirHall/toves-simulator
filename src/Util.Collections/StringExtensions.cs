/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections;
using System.Text;

namespace Toves.Util.Collections {
    public static class StringExtensions {
        public static String JoinObjectStrings(this String sep, IEnumerable data) {
            StringBuilder result = new StringBuilder();
            bool first = true;
            foreach (object o in data) {
                if (first) {
                    first = false;
                } else {
                    result.Append(sep);
                }
                result.Append(o.ToString());
            }
            return result.ToString();
        }
    }
}

