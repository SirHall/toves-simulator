/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Gtk;
using Toves.AbstractGui.Menu;
using Toves.GuiGeneric.Window;

namespace Toves.GuiImpl.GtkMain {
    public static class GtkMenu {
        private class Callback : IGenericMenuCallback {
            private Window parent;
            private GenericMenuItem menuItem;

            internal Callback(Window parent, GenericMenuItem menuItem) {
                this.parent = parent;
                this.menuItem = menuItem;
            }

            public void Handler(object sender, EventArgs args) {
                menuItem.Execute(this);
            }

            public void Notify(string title, string message) {
                Dialog dlog = new Dialog(title, parent, DialogFlags.Modal,
                                         "OK", ResponseType.Ok);
                dlog.VBox.Add(new Label(message));
                dlog.ShowAll();
                dlog.Run();
                dlog.Destroy();
            }

            public string RequestString(string title, string prompt) {
                Dialog dlog = new Dialog(title, parent, DialogFlags.Modal,
                                         "OK", ResponseType.Ok,
                                         "Cancel", ResponseType.Cancel);
                Label label = new Label(prompt);
                label.SetAlignment(0.0f, 0.5f);
                Entry input = new Entry();
                input.Activated += (obj, args) => {
                    dlog.Respond(ResponseType.Ok);
                };
                dlog.VBox.Add(label);
                dlog.VBox.Add(input);
                dlog.ShowAll();
                ResponseType result = (ResponseType) dlog.Run();
                string inputValue = input.Text;
                dlog.Destroy();
                if (result == ResponseType.Ok) {
                    return inputValue;
                } else {
                    return null;
                }
            }

        }

        public static MenuBar Create(Window window, WindowModel model) {
            MenuBar result = new MenuBar();
            foreach (GenericMenu menuModel in model.Menus) {
                MenuItem menuContainer = new MenuItem(menuModel.Title);
                menuContainer.Submenu = BuildMenu(window, menuModel);
                result.Append(menuContainer);
            }
            return result;
        }

        private static Menu BuildMenu(Window window, GenericMenu menuModel) {
            Menu result = new Menu();
            foreach (GenericMenuElement elt in menuModel.Elements) {
                if (elt is GenericMenu) {
                    MenuItem item = new MenuItem(elt.Title);
                    item.Submenu = BuildMenu(window, elt as GenericMenu);
                    result.Append(item);
                } else if (elt is GenericMenuItem) {
                    MenuItem item = new MenuItem(elt.Title);
                    item.Activated += new Callback(window, elt as GenericMenuItem).Handler;
                    result.Append(item);
                } else {
                    Console.Error.WriteLine("unexpected menu element {0}", elt.GetType().Name);
                }
            }
            return result;
        }
    }
}

