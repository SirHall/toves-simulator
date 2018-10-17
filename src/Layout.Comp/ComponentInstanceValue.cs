/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Sim.Inst;

namespace Toves.Layout.Comp
{
    public class ComponentInstanceValue<T> : ComponentInstance
    {
        public ComponentInstanceValue(Component component, T dflt) : base(component)
        {
            this.Value = dflt;
        }

        public T Value;
    }
}

