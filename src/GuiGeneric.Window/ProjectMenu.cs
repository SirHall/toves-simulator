/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.AbstractGui.Menu;
using Toves.Layout.Model;
using Toves.Proj.Model;
using Toves.Proj.Module;
using Toves.Util.Transaction;

namespace Toves.GuiGeneric.Window {
    public class ProjectMenu : GenericMenu {
        private WindowModel window;

        public ProjectMenu(WindowModel window) : base("Project") {
            this.window = window;

            AddItem(new GenericMenuItem("Add Module...", AddModule));
            AddItem(new GenericMenuItem("Rename Module...", RenameModule));
            AddItem(new GenericMenuItem("Remove Module", RemoveModule));
        }

        public void AddModule(IGenericMenuCallback callback) {
            String name = callback.RequestString("Add Module", "Name for new module:");
            if (name != null) {
                bool success;
                Transaction xn = new Transaction();
                IProjectAccess proj = xn.RequestWriteAccess(window.Project);
                using (xn.Start()) {
                    ProjectModule cur = proj.GetModule(name);
                    if (cur == null) {
                        success = true;
                        proj.AddModule(name, new LayoutModel());
                    } else {
                        success = false;
                    }
                }
                if (!success) {
                    callback.Notify("Cannot Add", "Cannot add second module of same name.");
                }
            }
        }
        
        public void RenameModule(IGenericMenuCallback callback) {
            ProjectModule toRename = window.CurrentModule;
            String name = callback.RequestString("Rename Module", "New name for module:");
            if (name != null) {
                name = name.Trim();
                if (name == "") {
                    callback.Notify("Cannot Rename", "Module cannot have an empty name.");
                } else {
                    bool success;
                    Transaction xn = new Transaction();
                    IProjectAccess proj = xn.RequestWriteAccess(window.Project);
                    using (xn.Start()) {
                        ProjectModule cur = proj.GetModule(name);
                        if (cur == null) {
                            success = true;
                            proj.SetModuleName(toRename, name);
                        } else {
                            success = false;
                        }
                    }
                    if (!success) {
                        callback.Notify("Cannot Rename", "Cannot add second module of same name.");
                    }
                }
            }
        }

        public void RemoveModule(IGenericMenuCallback callback) {
            ProjectModule toRemove = window.CurrentModule;
            Transaction xn = new Transaction();
            IProjectAccess proj = xn.RequestWriteAccess(window.Project);
            int failureReason = 0;
            using (xn.Start()) {
                IEnumerator<ProjectModule> mods = proj.GetModules().GetEnumerator();
                int size;
                if (!mods.MoveNext()) {
                    size = 0;
                } else {
                    if (!mods.MoveNext()) {
                        size = 1;
                    } else {
                        size = 2;
                    }
                }
                if (size == 1) {
                    failureReason = 1;
                } else {
                    bool success = proj.RemoveModule(toRemove);
                    if (!success) {
                        failureReason = 2;
                    }
                }
            }
            if (failureReason == 1) {
                callback.Notify("Cannot Remove", "Cannot remove only module in project.");
            } else if (failureReason == 2) {
                callback.Notify("Cannot Remove", "Module not found within project.");
            }
        }
    }
}

