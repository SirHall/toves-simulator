/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Diagnostics;
using Toves.Layout.Comp;

namespace Toves.Proj.Module {
    public abstract class ModuleInstance : ComponentInstance {
        public const int Untouched = 0;
        public const int FirstDepress = 1;
        public const int Touched = 2;
        public const int SecondDepress = 3;

        private int pokeState = Untouched;
        private Stopwatch pokeStopwatch = new Stopwatch();

        public ModuleInstance(ModuleComponent source) : base(source) {
            pokeStopwatch.Stop();
        }

        public abstract object SimulationState { get; }

        public int PokeState {
            get {
                int result = pokeState;
                if (result == Untouched) {
                    return result;
                } else if (pokeStopwatch.ElapsedMilliseconds < 1000) {
                    return result;
                } else {
                    pokeStopwatch.Stop();
                    pokeState = Untouched;
                    return Untouched;
                }
            }
            set {
                pokeState = value;
                if (value != Untouched) {
                    pokeStopwatch.Restart();
                }
            }
        }
    }
}