/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Sim.Inst {
    public class DummyInstanceState : IInstanceState {
        public DummyInstanceState(Instance instance) {
            this.Instance = instance;
        }

        public Instance Instance { get; private set; }

        public Value Get(int index) {
            int width = 1;
            int i = -1;
            foreach (Port p in Instance.Ports) {
                i++;
                if (i == index) {
                    width = p.Width;
                    break;
                }
            }
            return Value.Create(Value.X, width);
        }

        public Value GetDriven(int index) {
            return Get(index);
        }

        public void Set(int index, Value value, int delay) {
            throw new InvalidOperationException("cannot set value on dummy state");
        }
    }
}

