/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Sim.Inst {
    public class ValueOneBit : Value {
        public static Value _U = new ValueOneBit("U", 3, 5);
        public static Value _X = new ValueOneBit("X", 3, 4);
        public static Value _Zero = new ValueOneBit("0", 2, 2);
        public static Value _One = new ValueOneBit("1", 1, 2);
        public static Value _Z = new ValueOneBit("Z", 0, 0);
        public static Value _W = new ValueOneBit("W", 3, 1);
        public static Value _L = new ValueOneBit("L", 2, 1);
        public static Value _H = new ValueOneBit("H", 1, 1);
        public static Value _DontCare = new ValueOneBit("-", 7, 3);

        private readonly String name;
        private readonly int value;
        private readonly int strength;

        private ValueOneBit() { }

        private ValueOneBit(String name, int value, int strength) {
            this.name = name;
            this.value = value;
            this.strength = strength;
        }
        
        public override String ToString() {
            return name;
        }

        public override bool Equals(object other) {
            return this == other;
        }

        public override int GetHashCode() {
            return (value << 4) | strength;
        }

        public override int Width { get { return 1; } }
        
        public override Value this[int index] {
            get { return this; }
        }
        
        public override Value Pull3 {
            get {
                switch (this.value) {
                case 1:
                    return _One;
                case 2:
                    return _Zero;
                default:
                    return _X;
                }
            }
        }
        
        public override Value Pull4 {
            get {
                switch (this.value) {
                case 0:
                    return _Z;
                case 1:
                    return _One;
                case 2:
                    return _Zero;
                default:
                    return _X;
                }
            }
        }
        
        public override Value Not {
            get {
                Value a = this.Pull3;
                if (a == _Zero) {
                    return _One;
                } else if (a == _One) {
                    return _Zero;
                } else {
                    return _X;
                }
            }
        }

        public override Value Resolve(Value other){
            /*    | U X 0 1 Z W L H - 
             *  --+------------------
             *  U | U U U U U U U U U  s5
             *  X | U X X X X X X X X  s4
             *  0 | U X 0 X 0 0 0 0 X  s2
             *  1 | U X X 1 1 1 1 1 X  s2
             *  Z | U X 0 1 Z W L H X  s0
             *  W | U X 0 1 W W W W X  s1
             *  L | U X 0 1 L W L W X  s1
             *  H | U X 0 1 H W W H X  s1
             *  - | U X X X X X X X X  s3
             */
            WidthMismatchException.Check(1, other.Width);
            ValueOneBit o = other as ValueOneBit;
            if (this.strength > o.strength) {
                return this == _DontCare ? _X : this;
            } else if (o.strength > this.strength) {
                return o == _DontCare ? _X : other;
            } else if (this == other) {
                return this;
            } else if (this.strength == 1) {
                return _W;
            } else {
                return _X;
            }
        }

        public override Value And(Value other) {
            WidthMismatchException.Check(1, other.Width);
            Value a = this.Pull3;
            Value b = other.Pull3 as ValueOneBit;
            if (a == _Zero || b == _Zero) {
                return _Zero;
            } else if (a == _X || b == _X) {
                return _X;
            } else {
                return _One;
            }
        }
        
        public override Value Or(Value other) {
            WidthMismatchException.Check(1, other.Width);
            Value a = this.Pull3;
            Value b = other.Pull3 as ValueOneBit;
            if (a == _One || b == _One) {
                return _One;
            } else if (a == _X || b == _X) {
                return _X;
            } else {
                return _Zero;
            }
        }
    }
}

