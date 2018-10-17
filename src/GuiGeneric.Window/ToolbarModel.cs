/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Layout.Comp;
using Toves.GuiGeneric.LayoutCanvas;
using Toves.Components.Gates;
using Toves.Components.Io;

namespace Toves.GuiGeneric.Window {
    internal struct ToolbarTool {
        internal String imageName;
        internal Component toolMaster;

        internal ToolbarTool(String imageName, Component toolMaster) {
            this.imageName = imageName;
            this.toolMaster = toolMaster;
        }
    }

    public class ToolbarModel {
        private WindowModel window;

        private ToolbarTool[] tools = new ToolbarTool[] {
            new ToolbarTool("gate-and.gif", new AndGate()),
            new ToolbarTool("gate-or.gif", new OrGate()),
            new ToolbarTool("gate-not.gif", new NotGate()),
            new ToolbarTool("io-toggle.gif", new ToggleSwitch()),
            new ToolbarTool("io-led.gif", new Led()),
        };
        private int selected = -1;

        public ToolbarModel(WindowModel window) {
            this.window = window;
        }

        public event EventHandler<ToolbarChangedArgs> ToolbarChangedEvent;

        public int Count { get { return tools.Length; } }

        public int Selected {
            get { return selected; }
            set {
                if (selected != value) {
                    int old = selected;
                    selected = value;

                    if (value >= 0 && value < tools.Length) {
                        window.BeginAdd(tools[value].toolMaster, () => {
                            this.Selected = -1;
                        });
                    } else {
                        window.StopAdd(tools[old].toolMaster);
                    }

                    OnToolboxChangeEvent(new ToolbarChangedArgs(old, value));
                }
            }
        }

        public string GetImageName(int i) {
            return tools[i].imageName;
        }

        protected virtual void OnToolboxChangeEvent(ToolbarChangedArgs args) {
            EventHandler<ToolbarChangedArgs> handler = ToolbarChangedEvent;
            if (handler != null) {
                handler(this, args);
            }
        }
    }
}

