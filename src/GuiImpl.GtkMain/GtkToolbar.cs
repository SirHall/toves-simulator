/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Gtk;
using Toves.GuiGeneric.Window;

namespace Toves.GuiImpl.GtkMain {
    internal class ToolbarItem : ToggleToolButton {
        internal ToolbarItem(GtkToolbar toolbar, ToolbarModel model, int i) {
            this.IconWidget = GtkMain.GetImage(model.GetImageName(i));
            this.Index = i;
            this.Toggled += (sender, e) => {
                if (!toolbar.Updating) {
                    model.Selected = this.Active ? i : -1;
                }
            };
        }

        internal int Index { get; private set; }
    }

    public class GtkToolbar : Toolbar {
        private ToolbarItem[] buttons;

        public GtkToolbar(Window parent, ToolbarModel model) {
            this.Updating = false;
            this.ToolbarStyle = ToolbarStyle.Icons;
            this.Tooltips = false;

            buttons = new ToolbarItem[model.Count];
            for (int i = 0; i < buttons.Length; i++) {
                buttons[i] = new ToolbarItem(this, model, i);
                this.Insert(buttons[i], -1);
            }

            ToolItem space = new ToolItem();
            space.Expand = true;
            ToolButton info = new ToolButton(Stock.Info);
            info.Clicked += (sender, args) => {
                GtkAbout.ShowAbout(parent);
            };
            this.Insert(space, -1);
            this.Insert(info, -1);

            model.ToolbarChangedEvent += (sender, e) => SetSelected(e.Value);
        }

        internal bool Updating { get; set; }

        private void SetSelected(int value) {
            this.Updating = true;
            try {
                foreach (ToolbarItem item in buttons) {
                    bool act = value == item.Index;
                    if (item.Active != act) {
                        item.Active = act;
                    }
                }
            } finally {
                this.Updating = false;
            }
        }
    }
}

