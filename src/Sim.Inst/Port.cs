/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Sim.Model;

namespace Toves.Sim.Inst {
    public class Port : Node {
        private Instance instance;
        private PortArgs args;
        private Value drivenValue = Value.U;

        public Port(Instance instance, PortArgs args) {
            this.instance = instance;
            this.args = args;
        }

        public Instance Instance { get { return instance; } }

        public PortType Type { get { return args.Type; } }

        public int Width { get { return args.Width; } }

        public PortArgs PortArgs { get { return args; } }
        
        public bool IsOutput {
            get { return (Type & PortType.Output) == PortType.Output; }
        }
        
        public bool IsInput {
            get { return (Type & PortType.Input) == PortType.Input; }
        }

        public Value GetDrivenValue(SimulationModel.Key key) {
            if (key == null) {
                throw new InvalidOperationException("need key");
            }
            return this.drivenValue;
        }

        public void SetDrivenValue(SimulationModel.Key key, Value value) {
            if (key == null) {
                throw new InvalidOperationException("need key");
            }
            this.drivenValue = value;
        }
    }
}

