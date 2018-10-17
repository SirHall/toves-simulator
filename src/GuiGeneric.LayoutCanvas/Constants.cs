/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Layout.Data;
using Toves.Sim.Inst;

namespace Toves.GuiGeneric.LayoutCanvas
{
    public class Constants
    {
        public static readonly int COLOR_GHOST = 0x888888;
        public static readonly int COLOR_DEAD = 0xCCCCCC;

        public static readonly int GRID_SIZE = 32;
        public static readonly int WIRE_WIDTH = 10;
        public static readonly int SOLDER_RADIUS = 10;

        public static readonly int WIRING_READY_RADIUS = 16;

        private Constants()
        {
        }
        
        public static int GetColorFor(Value value) {
            if (value.Width == 1) {
                Value val = value.Pull4;
                if (val == Value.One) {
                    return 0x00DD00;
                } else if (val == Value.Zero) {
                    return 0x008800;
                } else if (val == Value.X) {
                    return 0xBB0000;
                } else {
                    return 0x000066;
                }
            } else {
                for (int i = 0; i < value.Width; i++) {
                    if (value[i] == Value.X) {
                        return 0x770000;
                    }
                }
                return 0x000000;
            }
        }

        public static Location SnapToGrid(Location inLoc) {
            return new Location((inLoc.X + 16) & ~0x1F, (inLoc.Y + 16) & ~0x1F);
        }
    }
}

