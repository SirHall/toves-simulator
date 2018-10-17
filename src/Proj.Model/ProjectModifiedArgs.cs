/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using Toves.Proj.Module;

namespace Toves.Proj.Model {
    public class ProjectModifiedArgs : EventArgs {
        public enum ChangeTypes {
            ModuleAdded,
            ModuleRemoved,
            ModuleRenamed
        }

        public ProjectModifiedArgs(IProjectAccess access, ChangeTypes changeType, ProjectModule changedModule) {
            this.ProjectAccess = access;
            this.ChangeType = changeType;
            this.ChangedModule = changedModule;
        }

        public IProjectAccess ProjectAccess { get; private set; }

        public ChangeTypes ChangeType { get; private set; }

        public ProjectModule ChangedModule { get; private set; }
    }
}