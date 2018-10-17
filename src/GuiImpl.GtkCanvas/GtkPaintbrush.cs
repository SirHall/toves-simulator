/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Cairo;
using Gtk;
using Toves.AbstractGui.Canvas;

namespace Toves.GuiImpl.GtkCanvas {
    public class GtkPaintbrush : IPaintbrush {
        private Widget parent;
        private Color color;

        public GtkPaintbrush(Widget parent, Context context) {
            this.parent = parent;
            this.Context = context;
            this.Color = 0x000000;
            this.FontFamily = "Arial";
            this.FontSize = 16;
            this.FontStyle = FontStyle.Normal;
        }

        internal Context Context { get; private set; }

        public int Color {
            get {
                Color c = color;
                int r = (int) (c.R * 255 + 0.49) & 0xFF;
                int g = (int) (c.G * 255 + 0.49) & 0xFF;
                int b = (int) (c.B * 255 + 0.49) & 0xFF;
                return (r << 16) | (g << 8) | b;
            }
            set {
                double r = ((value >> 16) & 0xFF) / 255.0;
                double g = ((value >> 8) & 0xFF) / 255.0;
                double b = (value & 0xFF) / 255.0;
                Color c = new Color(r, g, b);
                color = c;
                Context.Color = c;
            }
        }

        public int StrokeWidth {
            get {
                return (int) Context.LineWidth;
            }
            set {
                Context.LineWidth = value;
            }
        }
        
        public string FontFamily { get; set; }

        public FontStyle FontStyle { get; set; }

        public int FontSize { get; set; }

        public void TranslateCoordinates(int deltaX, int deltaY) {
            Context.Translate(deltaX, deltaY);
        }

        public void ScaleCoordinates(double scaleX, double scaleY) {
            Context.Scale(scaleX, scaleY);
        }

        public void StrokeCircle(int centerX, int centerY, int radius) {
            Context.Arc(centerX, centerY, radius, 0, 2 * Math.PI);
            Context.Stroke();
        }

        public void StrokeRectangle(int x0, int y0, int width, int height) {
            Context.Rectangle(x0, y0, width, height);
            Context.Stroke();
        }

        public void StrokePolygon(int[] xs, int[] ys) {
            Context.MoveTo(xs[0], ys[0]);
            for (int i = 1; i < xs.Length; i++) {
                Context.LineTo(xs[i], ys[i]);
            }
            Context.ClosePath();
            Context.Stroke();
        }

        public void StrokeArc(int centerX, int centerY, int radius,
                               int arcStart, int arcLength) {
            double pi = Math.PI;
            Context.Arc(centerX, centerY, radius, arcStart * pi / 180.0,
                   (arcStart + arcLength) * pi / 180.0);
            Context.Stroke();
        }

        public void StrokeLine(int x0, int y0, int x1, int y1)
        {
            Context.MoveTo(x0, y0);
            Context.LineTo(x1, y1);
            Context.Stroke();
        }

        public void StrokeLines(int[] xs, int[] ys)
        {
            Context.MoveTo(xs[0], ys[0]);
            for (int i = 1; i < xs.Length; i++) {
                Context.LineTo(xs[i], ys[i]);
            }
            Context.Stroke();
        }

        public void FillCircle(int centerX, int centerY, int radius) {
            Context.Arc(centerX, centerY, radius, 0, 2 * Math.PI);
            Context.Fill();
        }
        
        public void FillRectangle(int x0, int y0, int width, int height) {
            Context.Rectangle(x0, y0, width, height);
            Context.Fill();
        }

        public void FillPolygon(int[] xs, int[] ys) {
            Context.MoveTo(xs[0], ys[0]);
            for (int i = 1; i < xs.Length; i++) {
                Context.LineTo(xs[i], ys[i]);
            }
            Context.ClosePath();
            Context.Fill();
        }
        
        public void DrawText(int x, int y, String text, TextAlign align) {
            Pango.FontDescription fd = Pango.FontDescription.FromString(FontFamily);
            fd.Size = Pango.Units.FromPixels(FontSize);
            if ((FontStyle & FontStyle.Bold) != 0) {
                fd.Weight = Pango.Weight.Bold;
            }
            if ((FontStyle & FontStyle.Italic) != 0) {
                fd.Style = Pango.Style.Italic;
            }

            Pango.Font font = parent.PangoContext.LoadFont(fd);
            Pango.Language lang = Pango.Language.FromString("en");
            Pango.FontMetrics metrics = font.GetMetrics(lang);
            int estWidth = text.Length * metrics.ApproximateCharWidth * 3 / 2;

            Pango.Context pango = parent.PangoContext;
            Pango.Layout layout = new Pango.Layout(pango) { Width = estWidth,
                Wrap = Pango.WrapMode.Word, Alignment = Pango.Alignment.Left,
                FontDescription = fd };
            int color = this.Color;
            ushort cRed = (ushort) ((color >> 16) & 0xFF);
            ushort cGreen = (ushort) ((color >> 8) & 0xFF);
            ushort cBlue = (ushort) (color & 0xFF);
            layout.Attributes = new Pango.AttrList();
            layout.Attributes.Insert(new Pango.AttrForeground(cRed, cGreen, cBlue));
            layout.SetText(text);

            double x0;
            double y0;
            double scale = 1.0 / Pango.Scale.PangoScale;
            Pango.Rectangle dummy;
            Pango.Rectangle extents;
            layout.GetExtents(out dummy, out extents);
            switch ((TextAlign) ((int) align & 7)) {
            case TextAlign.Right:
                x0 = x - scale * extents.Width;
                break;
            case TextAlign.Center:
                x0 = x - 0.5 * scale * extents.Width;
                break;
            default:
                x0 = x;
                break;
            }
            switch ((TextAlign) ((int) align & ~7)) {
            case TextAlign.Top:
                y0 = y;
                break;
            case TextAlign.Bottom:
                y0 = y - scale * (metrics.Ascent + metrics.Descent);
                break;
            case TextAlign.VCenter:
                y0 = y - 0.5 * scale * metrics.Ascent;
                break;
            default:
                y0 = y - scale * metrics.Ascent;
                break;
            }

            Matrix srcM = Context.Matrix;
            Pango.Matrix dstM;
            dstM.X0 = 0.0;
            dstM.Xx = srcM.Xx;
            dstM.Xy = srcM.Xy;
            dstM.Y0 = 0.0;
            dstM.Yx = srcM.Yx;
            dstM.Yy = srcM.Yy;
            parent.PangoContext.Matrix = dstM;
            srcM.TransformPoint(ref x0, ref y0);
            parent.GdkWindow.DrawLayout(parent.Style.TextGC(StateType.Normal),
                                        (int) (0.5 + x0), (int) (0.5 + y0), layout);
            dstM.Xx = 1.0;
            dstM.Yy = 1.0;
            parent.PangoContext.Matrix = dstM;
        }

        public IPaintbrush Create() {
            return new GtkSubPaintbrush(this);
        }

        public void Dispose() {
            throw new InvalidOperationException("Can't dispose GtkPaintbrush");
        }
    }

    internal class GtkSubPaintbrush : PaintbrushAdapter {
        private int savedColor;
        private string savedFontFamily;
        private int savedFontSize;
        private FontStyle savedFontStyle;

        internal GtkSubPaintbrush(GtkPaintbrush master) : base(master) {
            savedColor = master.Color;
            savedFontFamily = master.FontFamily;
            savedFontSize = master.FontSize;
            savedFontStyle = master.FontStyle;
            master.Context.Save();
        }

        public override IPaintbrush Create() {
            return new GtkSubPaintbrush(this.BaseBrush as GtkPaintbrush);
        }

        public override void Dispose() {
            GtkPaintbrush master = this.BaseBrush as GtkPaintbrush;
            master.Context.Restore();
            master.Color = savedColor;
            master.FontFamily = savedFontFamily;
            master.FontSize = savedFontSize;
            master.FontStyle = savedFontStyle;
        }
    }
}