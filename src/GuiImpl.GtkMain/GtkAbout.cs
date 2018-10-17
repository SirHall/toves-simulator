/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Gtk;

namespace Toves.GuiImpl.GtkMain {
    public class GtkAbout : Dialog {
        public static void ShowAbout(Window parent) {
            GtkAbout dlog = new GtkAbout(parent);
            dlog.Modal = true;
            dlog.AddButton("Close", ResponseType.Close);
            dlog.Run();
            dlog.Destroy();
        }

        private GtkAbout(Window parent) : base("About Toves", parent, 0) {
            VBox vbox = new VBox();
            AddLabel(vbox, string.Format("Toves {0}", GtkMain.Version));
            AddLabel(vbox, string.Format("(c) {0}, Carl Burch (<tt>toves@toves.org</tt>)", GtkMain.CopyrightYear));
            AddLabel(vbox, "Released under GPLv3");
            AddLabel(vbox, "Information at <i>http://www.toves.org/</i>");
            vbox.PackEnd(new Label(""), true, true, 0);

            HBox hbox = new HBox();
            Image img = GtkMain.GetImage("tove.png");
            hbox.BorderWidth = 10;
            hbox.Spacing = 10;
            hbox.Add(img);
            hbox.Add(vbox);

            this.VBox.Add(hbox);
            this.ShowAll();
        }

        private void AddLabel(VBox vbox, string labelString) {
            Label label = new Label();
            label.Markup = labelString;
            label.SetAlignment(0, 0);
            vbox.PackStart(label, false, true, 3);
        }
    }
}

