/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Sim.Inst {
    public class ValueLong : Value {
        private static ValueKey key = null;
        
        private readonly byte width;
        private readonly long value;

        private ValueLong() { }
        
        public ValueLong(ValueKey key, int width, long value) {
            if (key == null) {
                throw new InvalidOperationException("need key");
            }
            this.width = (byte) width;
            this.value = value & GetMask(width);
        }

        private long GetMask(int width) {
            if (width == 64) {
                return -1;
            } else {
                return (1 << width) - 1;
            }
        }
        
        public override String ToString() {
            long v = value;
            String[] strs = new String[width];
            for (int i = 0; i < strs.Length; i++) {
                strs[i] = ((v >> i) & 1).ToString();
            }
            return string.Join("", strs);
        }
        
        public override bool Equals(object other) {
            ValueLong o = other as ValueLong;
            return o != null && this.width == o.width && this.value == o.value;
        }

        public override int GetHashCode() {
            return (int) this.value;
        }

        public override int Width {
            get { return width; }
        } 
        
        public override Value this[int index] {
            get { return ((value >> index) & 1) != 0 ? Value.One : Value.Zero; }
        }

        public override Value Pull3 {
            get { return this; }
        }
        
        public override Value Pull4 {
            get { return this; }
        }
        
        public override Value Resolve(Value other) {
            WidthMismatchException.Check(width, other.Width);
            ValueLong o = other as ValueLong;
            if (o == null) {
                return other.Resolve(this);
            } else if (o.value == this.value) {
                return this;
            } else {
                Value[] vs = new Value[width];
                long src = this.value;
                long diff = this.value ^ o.value;
                for (int i = 0; i < vs.Length; i++) {
                    if (((diff >> i) & 1) != 0) {
                        vs[i] = Value.X;
                    } else {
                        vs[i] = ((src >> i) & 1) != 0 ? Value.One : Value.Zero;
                    }
                }
                return new ValueMultiBit(key, vs);
            }
        }

        public override Value Not {
            get { return new ValueLong(key, width, ~value); }
        }
        
        public override Value And(Value other) {
            WidthMismatchException.Check(width, other.Width);
            ValueLong o = other as ValueLong;
            if (o == null) {
                return other.Resolve(this);
            } else if (o.value == this.value) {
                return this;
            } else {
                return new ValueLong(key, width, value & o.value);
            }
        }
        
        public override Value Or(Value other) {
            WidthMismatchException.Check(width, other.Width);
            ValueLong o = other as ValueLong;
            if (o == null) {
                return other.Resolve(this);
            } else if (o.value == this.value) {
                return this;
            } else {
                return new ValueLong(key, width, value | o.value);
            }
        }
    }
}
