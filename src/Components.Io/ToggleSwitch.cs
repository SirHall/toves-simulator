/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Sim.Inst;

namespace Toves.Components.Io {
    public class ToggleSwitch : ComponentSharedData, Pokeable {
        private class SwitchInstance : ComponentInstance {
            internal Value currentValue = Value.Zero;
            internal bool pendingPress = false;

            internal SwitchInstance(Component comp) : base(comp) { }
        }

        public ToggleSwitch() {
            ShareOffsetBounds(new Bounds(-64, -32, 64, 64));
            this.Connections = new ConnectionPoint[] { ConnectionPoint.newOutput(0, 0) };
        }

        public override string Name { get { return "Toggle Switch"; } }

        public override bool Contains(int offsetX, int offsetY) {
            return OffsetBounds.Contains(offsetX, offsetY, 5);
        }

        public void ProcessPokeEvent(PokeEventArgs args) {
            switch (args.Type) {
            case PokeEventType.PokeStart:
                args.StateUpdate = PokeUpdate(args, true, false);
                break;
            case PokeEventType.PokeEnd:
                args.StateUpdate = PokeUpdate(args, false, true);
                break;
            case PokeEventType.PokeCancel:
                args.StateUpdate = PokeUpdate(args, false, false);
                break;
            }
        }

        private Action<IInstanceState> PokeUpdate(PokeEventArgs args, bool pending, bool flip) {
            return (IInstanceState state) => {
                SwitchInstance myState = state.Instance as SwitchInstance;
                bool repaint = false;
                if (flip) {
                    myState.currentValue = myState.currentValue.Not;
                    myState.pendingPress = pending;
                    args.Repropagate();
                    repaint = true;
                }
                if (myState.pendingPress != pending) {
                    myState.pendingPress = pending;
                    repaint = true;
                }
                if (repaint) {
                    args.Repaint();
                }
            };
        }

        public void PaintPokeProgress(IComponentPainter painter) {
        }

        public override Instance CreateInstance() {
            return new SwitchInstance(this);
        }
            
        public override void Propagate(IInstanceState state) {
            SwitchInstance myState = state.Instance as SwitchInstance;
            Value curVal = myState == null ? Value.X : myState.currentValue;
            state.Set(0, curVal, 1);
        }
        
        public override void Paint(IComponentPainter painter) {
            SwitchInstance myState = painter.Instance as SwitchInstance;
            bool pending = myState.pendingPress;
            Value value = myState.currentValue;
            int valColor = painter.GetColorFor(value);
            int vr = (valColor >> 16) & 0xFF;
            int vg = (valColor >> 8) & 0xFF;
            int vb = valColor & 0xFF;
            int darken = ((vr >> 1) << 16) | ((vg >> 1) << 8) | (vb >> 1);
            int contrast = vr + vg + vb > 128 * 3 ? 0x000000 : 0xFFFFFF;

            painter.FontSize = 40;
            painter.FontStyle = Toves.AbstractGui.Canvas.FontStyle.Bold;
            if (pending) {
                painter.StrokeWidth = 10;
                painter.Color = valColor;
                painter.FillRectangle(-52, -20, 52, 52);
                painter.Color = 0;
                painter.StrokeRectangle(-52, -20, 52, 52);
                painter.Color = contrast;
                painter.DrawText(-26, 0, value.ToString(), TextAlign.Center | TextAlign.VCenter);
            } else {
                int[] bgXs = new int[] { -64, -52, 0, 0, -12 };
                int[] bgYs = new int[] { 20, 32, 32, -20, -32 };
                painter.StrokeWidth = 10;
                painter.Color = darken;
                painter.FillPolygon(bgXs, bgYs);
                painter.Color = valColor;
                painter.FillRectangle(-64, -32, 52, 52);
                painter.Color = 0;
                painter.StrokeRectangle(-64, -32, 52, 52);
                painter.StrokeLines(bgXs, bgYs);
                painter.StrokeLine(-12, 20, 0, 32);
                painter.Color = contrast;
                painter.DrawText(-38, -12, value.ToString(), TextAlign.Center | TextAlign.VCenter);
            }
            painter.PaintPorts();
        }
    }
}