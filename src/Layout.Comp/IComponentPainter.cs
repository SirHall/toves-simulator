/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

using Toves.AbstractGui.Canvas;
using Toves.Layout.Data;
using Toves.Sim.Inst;

namespace Toves.Layout.Comp {
    public class TextAlign  {
        public static readonly Toves.AbstractGui.Canvas.TextAlign BaselineLeft
            = Toves.AbstractGui.Canvas.TextAlign.BaselineLeft;
        public static readonly Toves.AbstractGui.Canvas.TextAlign Right
            = Toves.AbstractGui.Canvas.TextAlign.Right;
        public static readonly Toves.AbstractGui.Canvas.TextAlign Center
            = Toves.AbstractGui.Canvas.TextAlign.Center;
        public static readonly Toves.AbstractGui.Canvas.TextAlign Top
            = Toves.AbstractGui.Canvas.TextAlign.Top;
        public static readonly Toves.AbstractGui.Canvas.TextAlign Bottom
            = Toves.AbstractGui.Canvas.TextAlign.Bottom;
        public static readonly Toves.AbstractGui.Canvas.TextAlign VCenter
            = Toves.AbstractGui.Canvas.TextAlign.VCenter;
    }

    public interface IComponentPainter : IPaintbrush {
        ComponentInstance Instance { get; }

        Value GetPortValue(int index);

        int GetColorFor(Value value);

        void SetColorFor(Value value);

        void PaintPorts();
    }
}