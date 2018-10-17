/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Layout.Comp
{
    public interface Pokeable
    {
        void ProcessPokeEvent(PokeEventArgs args);

        void PaintPokeProgress(IComponentPainter painter);
    }
}

