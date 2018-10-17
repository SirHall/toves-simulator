/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Gtk;
using Toves.GuiGeneric.Window;

namespace Toves.GuiImpl.GtkMain {
    public class GtkToolbox : TreeView {
        private ToolboxModel source;

        public GtkToolbox(ToolboxModel source) {
            this.source = source;
            CellRendererText nameRenderer = new CellRendererText();
            TreeViewColumn nameColumn = new TreeViewColumn();
            nameColumn.Title = "Component";
            nameColumn.PackStart(nameRenderer, true);
            nameColumn.SetCellDataFunc(nameRenderer, GetCellData);
            this.AppendColumn(nameColumn);

            this.Model = BuildStore(source);
            this.HeadersVisible = false;
            this.Selection.Mode = SelectionMode.Single;
            this.Selection.Changed += HandleSelect;
            this.RowActivated += HandleActivation;
            source.ToolboxChangedEvent += UpdateStore;

            this.ExpandAll();
        }

        private TreeStore BuildStore(ToolboxModel source) {
            TreeStore store = new TreeStore(typeof(ToolboxItem));
            foreach (ToolboxDrawer drawer in source.Drawers) {
                TreeIter drawerIter = store.AppendValues(drawer);
                foreach (ToolboxItem item in drawer.GetContents()) {
                    store.AppendValues(drawerIter, item);
                }
            }
            return store;
        }
        
        private void UpdateStore(object sourceObj, ToolboxChangedArgs args) {
            ToolboxChangedArgs.ChangeTypes changeType = args.ChangeType;
            ToolboxItem changedItem = args.ChangedItem;
            TreeStore store = this.Model as TreeStore;
            if (changeType == ToolboxChangedArgs.ChangeTypes.ItemUnselected) {
                foreach (TreePath path in this.Selection.GetSelectedRows()) {
                    TreeIter iter;
                    if (store.GetIter(out iter, path)) {
                        ToolboxItem val = GetItem(iter);
                        if (val == changedItem) {
                            this.Selection.UnselectIter(iter);
                        }
                    }
                }
            } else if (changeType == ToolboxChangedArgs.ChangeTypes.ItemAdded) {
                ToolboxDrawer containingDrawer = null;
                ToolboxItem itemBefore = null;
                foreach (ToolboxDrawer drawer in source.Drawers) {
                    ToolboxItem prev = null;
                    foreach (ToolboxItem item in drawer.GetContents()) {
                        if (item == changedItem) {
                            containingDrawer = drawer;
                            itemBefore = prev;
                        }
                        prev = item;
                    }
                }
                store.Foreach((model, path, iter) => {
                    if (itemBefore == null) {
                        if (GetItem(iter) == containingDrawer) {
                            TreeIter node = store.PrependNode(iter);
                            store.SetValue(node, 0, changedItem);
                            return true;
                        }
                    } else {
                        if (GetItem(iter) == itemBefore) {
                            TreeIter parent;
                            store.IterParent(out parent, iter);
                            TreeIter node = store.InsertNodeAfter(parent, iter);
                            store.SetValue(node, 0, changedItem);
                            return true;
                        }
                    }
                    return false;
                });
            } else {
                store.Foreach((model, path, iter) => {
                    if (GetItem(iter) == changedItem) {
                        if (changeType == ToolboxChangedArgs.ChangeTypes.ItemRemoved) {
                            store.Remove(ref iter);
                        } else if (changeType == ToolboxChangedArgs.ChangeTypes.ItemRenamed) {
                            store.EmitRowChanged(path, iter);
                        }
                        return true;
                    }
                    return false;
                });
            }
        }

        private ToolboxItem GetItem(TreeIter iter) {
            TreeStore store = this.Model as TreeStore;
            return store == null ? null : store.GetValue(iter, 0) as ToolboxItem;
        }

        private void GetCellData(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter) {
            CellRendererText cellText = cell as CellRendererText;
            ToolboxItem val = GetItem(iter);
            if (val != null) {
                cellText.Text = val.Name;
            }
        }

        private void HandleSelect(object sender, EventArgs args) {
            TreeStore store = this.Model as TreeStore;
            if (store != null) {
                foreach (TreePath path in this.Selection.GetSelectedRows()) {
                    TreeIter iter;
                    if (store.GetIter(out iter, path)) {
                        ToolboxItem val = GetItem(iter);
                        if (val != null) {
                            source.SelectItem(val);
                        }
                    }
                }
            }
        }
        
        private void HandleActivation(object sender, RowActivatedArgs args) {
            TreeStore store = this.Model as TreeStore;
            if (store != null) {
                TreeIter iter;
                if (store.GetIter(out iter, args.Path)) {
                    ToolboxItem val = GetItem(iter);
                    if (val != null) {
                        source.ActivateItem(val);
                    }
                }
            }
        }
    }
}