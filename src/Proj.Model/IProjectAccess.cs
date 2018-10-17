/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Util.Transaction;
using Toves.Proj.Module;

namespace Toves.Proj.Model {
    public interface IProjectAccess : IResourceAccess {
        IEnumerable<ProjectModule> GetModules();
        ProjectModule GetModule(String name);
        string GetModuleName(ProjectModule module);

        ProjectModule AddModule(String name, IImplementation implementation);
        bool RemoveModule(ProjectModule module);
        void SetModuleName(ProjectModule module, String value);
    }
}

