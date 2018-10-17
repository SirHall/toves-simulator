/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Sim.Inst;

namespace Toves.Layout.Comp
{
    public enum PokeEventType {
        PokeStart,
        PokeMove,
        PokeEnd,
        PokeCancel
    }

    public interface PokeEventArgs
    {
        PokeEventType Type { get; }

        int X { get; }

        int Y { get; }

        Action<IInstanceState> StateUpdate { get; set; }

        void Repaint();

        void Repropagate();

        void RejectPoke();

        void RequestView(object module, object sim);
    }
}

