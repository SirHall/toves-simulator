/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.AbstractGui.Canvas {
    public abstract class PaintbrushAdapter : IPaintbrush {
        public PaintbrushAdapter(IPaintbrush baseBrush) {
            this.BaseBrush = baseBrush;
        }

        public IPaintbrush BaseBrush { get; set; }

        public int Color {
            get { return BaseBrush.Color; }
            set { BaseBrush.Color = value; }
        }

        public int StrokeWidth {
            get { return BaseBrush.StrokeWidth; }
            set { BaseBrush.StrokeWidth = value; }
        }
                
        public string FontFamily {
            get { return BaseBrush.FontFamily; }
            set { BaseBrush.FontFamily = value; }
        }

        public FontStyle FontStyle {
            get { return BaseBrush.FontStyle; }
            set { BaseBrush.FontStyle = value; }
        }

        public int FontSize {
            get { return BaseBrush.FontSize; }
            set { BaseBrush.FontSize = value; }
        }


        public void TranslateCoordinates(int deltaX, int deltaY) {
            BaseBrush.TranslateCoordinates(deltaX, deltaY);
        }

        public void ScaleCoordinates(double scaleX, double scaleY) {
            BaseBrush.ScaleCoordinates(scaleX, scaleY);
        }

        public void StrokeCircle(int cx, int cy, int radius) {
            BaseBrush.StrokeCircle(cx, cy, radius);
        }

        public void StrokeRectangle(int x0, int y0, int width, int height) {
            BaseBrush.StrokeRectangle(x0, y0, width, height);
        }

        public void StrokePolygon(int[] xs, int[] ys) {
            BaseBrush.StrokePolygon(xs, ys);
        }

        public void StrokeArc(int cx, int cy, int radius, int arcStart, int arcLength) {
            BaseBrush.StrokeArc(cx, cy, radius, arcStart, arcLength);
        }

        public void StrokeLine(int x0, int y0, int x1, int y1) {
            BaseBrush.StrokeLine(x0, y0, x1, y1);
        }

        public void StrokeLines(int[] xs, int[] ys) {
            BaseBrush.StrokeLines(xs, ys);
        }

        public void FillCircle(int cx, int cy, int radius) {
            BaseBrush.FillCircle(cx, cy, radius);
        }

        public void FillRectangle(int x0, int y0, int width, int height) {
            BaseBrush.FillRectangle(x0, y0, width, height);
        }

        public void FillPolygon(int[] xs, int[] ys) {
            BaseBrush.FillPolygon(xs, ys);
        }
        
        public void DrawText(int x, int y, String text, TextAlign align) {
            BaseBrush.DrawText(x, y, text, align);
        }

        public abstract IPaintbrush Create();

        public virtual void Dispose() {
            BaseBrush.Dispose();
        }
    }
}