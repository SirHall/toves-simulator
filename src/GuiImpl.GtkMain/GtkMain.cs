/*
 * Copyright (c) 2013, Carl Burch.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * Carl Burch can be contacted at cburch@cburch.com or by post at
 * Hendrix College, 1600 Washington Ave, Conway AR 72032, USA.
 */
using System;
using System.Reflection;
using Gtk;

using Toves.GuiGeneric.LayoutCanvas;
using Toves.GuiGeneric.Window;
using Toves.GuiImpl.GtkCanvas;

namespace Toves.GuiImpl.GtkMain {
    public class GtkMain : Gtk.Window {
        public static string Version = "0.0.2";
        public static string CopyrightYear = "2013";

        public static void Main(string[] args) {
            Application.Init();
            GtkMain win = new GtkMain();
            win.Show();
            Application.Run();
        }

        public static Image GetImage(string name) {
            Assembly assem = Assembly.GetAssembly(typeof(WindowModel));
            string rsrc = string.Format("{0}.images.{1}", assem.GetName().Name,
                                        name);
            return new Image(assem, rsrc);
        }

        private WindowModel windowModel = new WindowModel();
        private GtkCanvas.GtkCanvas canvas;

        public GtkMain () : base("Toves") {
            MenuBar menubar = GtkMenu.Create(this, windowModel);
            GtkToolbar toolbar = new GtkToolbar(this, windowModel.ToolbarModel);
            GtkToolbox toolbox = new GtkToolbox(windowModel.ToolboxModel);
            canvas = new GtkCanvas.GtkCanvas();
            canvas.CanvasModel = windowModel.LayoutCanvas;

            HPaned hbox = new HPaned();
            hbox.Add1(toolbox);
            hbox.Add2(canvas);
            
            VBox vbox = new VBox(false, 0);
            vbox.PackStart(menubar, false, false, 0);
            vbox.PackStart(toolbar, false, false, 0);
            vbox.PackEnd(hbox, true, true, 0);

            this.Add(vbox);
            this.SetDefaultSize(980, 600);
            this.ShowAll();
            canvas.GrabFocus();
        }

        protected override bool OnDeleteEvent(Gdk.Event evnt) {
            windowModel.Dispose();
            Application.Quit();
            return true;
        }
    }
}
