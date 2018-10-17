/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Sim.Inst {
    public class ValueKey {
        internal ValueKey() { }
    }

    public abstract class Value {
        private static ValueKey key = new ValueKey();

        public static Value U = ValueOneBit._U;
        public static Value X = ValueOneBit._X;
        public static Value Zero = ValueOneBit._Zero;
        public static Value One = ValueOneBit._One;
        public static Value Z = ValueOneBit._Z;
        public static Value W = ValueOneBit._W;
        public static Value L = ValueOneBit._L;
        public static Value H = ValueOneBit._H;
        public static Value DontCare = ValueOneBit._DontCare;

        public static Value Create(Value[] bits) {
            if (bits.Length <= 1) {
                if (bits.Length == 0) {
                    throw new ArgumentException("must have at least one bit");
                } else if (bits[0].Width != 1) {
                    throw new ArgumentException("each array entry must be a one-bit value");
                } else {
                    return bits[0];
                }
            }
            int width = bits.Length;
            bool all01 = true;
            long mask = width <= 64 ? (1L << (width - 1)) : 0;
            long longValue = 0;
            foreach (Value v in bits) {
                if (v.Width != 1) {
                    throw new ArgumentException("each array entry must be a one-bit value");
                }
                if (v == Value.Zero) {
                    mask >>= 1;
                } else if (v == Value.One) {
                    longValue |= mask;
                    mask >>= 1;
                } else {
                    all01 = false;
                }
            }
            if (all01 && width <= 64) {
                return new ValueLong(key, width, longValue);
            } else {
                return new ValueMultiBit(key, bits);
            }
        }

        public static Value Create(Value bit, int width) {
            if (bit.Width != 1) {
                throw new ArgumentException("argument must be one-bit value");
            } else if (width <= 0) {
                throw new ArgumentException("width must be positive");
            } else if (width == 1) {
                return bit;
            } else if (bit == Value.Zero) {
                return new ValueLong(key, width, 0);
            } else if (bit == Value.One) {
                return new ValueLong(key, width, -1L);
            } else {
                Value[] vs = new Value[width];
                for (int i = 0; i < width; i++) {
                    vs[i] = bit;
                }
                return new ValueMultiBit(key, vs);
            }
        }

        public static Value FromInt(int value, int width) {
            if (width == 1) {
                return (value & 1) == 0 ? Value.Zero : Value.One;
            } else if (width > 1 && width <= 64) {
                return new ValueLong(key, width, value);
            } else {
                throw new ArgumentException();
            }
        }

        public Value() { }

        public abstract override String ToString();

        public abstract override bool Equals(object other);

        public abstract override int GetHashCode();

        public abstract int Width { get; }

        public abstract Value this[int index] { get; }

        public abstract Value Pull3 { get; }

        public abstract Value Pull4 { get; }
        
        public abstract Value Not { get; }

        public abstract Value Resolve(Value other);

        public abstract Value And(Value other);

        public abstract Value Or(Value other);
    }
}

