/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.AbstractGui.Canvas {
    public enum FontStyle {
        Normal = 0,
        Bold = 1,
        Italic = 2,
        BoldItalic = 3
    }

    [Flags]
    public enum TextAlign {
        BaselineLeft = 0,
        Right = 1,
        Center = 2,
        Top = 8,
        Bottom = 16,
        VCenter = 24,
    }

    public interface IPaintbrush : IDisposable {
        int Color { get; set; }

        int StrokeWidth { get; set; }
        
        string FontFamily { get; set; }

        FontStyle FontStyle { get; set; }

        int FontSize { get; set; }

        IPaintbrush Create();

        void TranslateCoordinates(int deltaX, int deltaY);

        void ScaleCoordinates(double scaleX, double scaleY);

        void StrokeCircle(int cx, int cy, int radius);

        void StrokeRectangle(int x0, int y0, int width, int height);

        void StrokePolygon(int[] xs, int[] ys);

        void StrokeArc(int cx, int cy, int radius, int arcStart, int arcLength);

        void StrokeLine(int x0, int y0, int x1, int y1);

        void StrokeLines(int[] xs, int[] ys);

        void FillCircle(int cx, int cy, int radius);

        void FillRectangle(int x0, int y0, int width, int height);

        void FillPolygon(int[] xs, int[] ys);

        void DrawText(int x, int y, String text, TextAlign align);
    }
}