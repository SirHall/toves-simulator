/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Sim.Inst {
    public class ValueMultiBit : Value {
        private Value[] values;
        private Value pull3 = null;
        private Value pull4 = null;

        private ValueMultiBit() { }
        
        public ValueMultiBit(ValueKey key, Value[] values) {
            if (key == null) {
                throw new InvalidOperationException("need key");
            }
            this.values = values;
        }

        public override String ToString() {
            Value[] vs = values;
            String[] strs = new String[vs.Length];
            for (int i = 0; i < vs.Length; i++) {
                strs[i] = vs[i].ToString();
            }
            return string.Join("", strs);
        }
        
        public override bool Equals(object other) {
            ValueMultiBit o = other as ValueMultiBit;
            if (o == null) {
                return false;
            } else if (o == this) {
                return true;
            } else {
                Value[] v0s = this.values;
                Value[] v1s = o.values;
                if (v0s.Length != v1s.Length) {
                    return false;
                }
                for (int i = 0; i < v0s.Length; i++) {
                    if (v0s[i] != v1s[i]) {
                        return false;
                    }
                }
                return true;
            }
        }

        public override int GetHashCode() {
            int ret = 0;
            foreach (Value v in this.values) {
                ret = 31 * ret + v.GetHashCode();
            }
            return ret;
        }

        public override int Width {
            get { return values.Length; }
        } 
        
        public override Value this[int index] {
            get { return values[index]; }
        }
        
        public override Value Pull3 {
            get {
                Value[] vs = values;
                Value[] ps = vs;
                Value ret = pull3;
                if (ret == null) {
                    bool same = true;
                    ps = new Value[values.Length];
                    for (int i = 0; i < ps.Length; i++) {
                        Value v0 = vs[i];
                        Value v1 = v0.Pull4;
                        if (v0 != v1) {
                            same = false;
                        }
                        ps[i] = v1;
                    }
                    if (same) {
                        ps = vs;
                    }
                    ret = Value.Create(ps);
                    pull3 = ret;
                }
                return ret;
            }
        }

        public override Value Pull4 {
            get {
                Value[] vs = values;
                Value[] ps = vs;
                Value ret = pull4;
                if (ret == null) {
                    bool same = true;
                    ps = new Value[values.Length];
                    for (int i = 0; i < ps.Length; i++) {
                        Value v0 = vs[i];
                        Value v1 = v0.Pull4;
                        if (v0 != v1) {
                            same = false;
                        }
                        ps[i] = v1;
                    }
                    if (same) {
                        ps = vs;
                    }
                    ret = Value.Create(ps);
                    pull4 = ret;
                }
                return ret;
            }
        }
        
        public override Value Not {
            get {
                Value[] v0s = values;
                Value[] ret = new Value[v0s.Length];
                bool v0Same = true;
                for (int i = 0; i < ret.Length; i++) {
                    Value v0 = v0s[i];
                    Value vx = v0.Not;
                    ret[i] = vx;
                    if (vx != v0) {
                        v0Same = false;
                    }
                }
                return v0Same ? this : Value.Create(ret);
            }
        }

        public override Value Resolve(Value other) {
            Value[] v0s = values;
            WidthMismatchException.Check(v0s.Length, other.Width);
            Value[] ret = new Value[v0s.Length];
            bool v0Same = true;
            bool v1Same = true;
            for (int i = 0; i < ret.Length; i++) {
                Value v0 = v0s[i];
                Value v1 = other[i];
                Value vx = v0.Resolve(v1);
                ret[i] = vx;
                if (vx != v0) {
                    v0Same = false;
                }
                if (vx != v1) {
                    v1Same = false;
                }
            }
            return v0Same ? this : v1Same ? this : Value.Create(ret);
        }

        public override Value And(Value other) {
            Value[] v0s = values;
            WidthMismatchException.Check(v0s.Length, other.Width);
            Value[] ret = new Value[v0s.Length];
            bool v0Same = true;
            bool v1Same = true;
            for (int i = 0; i < ret.Length; i++) {
                Value v0 = v0s[i];
                Value v1 = other[i];
                Value vx = v0.And(v1);
                ret[i] = vx;
                if (vx != v0) {
                    v0Same = false;
                }
                if (vx != v1) {
                    v1Same = false;
                }
            }
            return v0Same ? this : v1Same ? this : Value.Create(ret);
        }
        
        public override Value Or(Value other) {
            Value[] v0s = values;
            WidthMismatchException.Check(v0s.Length, other.Width);
            Value[] ret = new Value[v0s.Length];
            bool v0Same = true;
            bool v1Same = true;
            for (int i = 0; i < ret.Length; i++) {
                Value v0 = v0s[i];
                Value v1 = other[i];
                Value vx = v0.Or(v1);
                ret[i] = vx;
                if (vx != v0) {
                    v0Same = false;
                }
                if (vx != v1) {
                    v1Same = false;
                }
            }
            return v0Same ? this : v1Same ? this : Value.Create(ret);
        }
    }
}
