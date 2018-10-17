/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Util.Transaction {
    public enum ResourceHookType {
        AfterRead,
        AfterWrite,
        AfterDowngrade
    }

    public interface Resource {
        ResourceHelper ResourceHelper { get; }

        void Hook(ResourceHookType hookType, IResourceAccess access);
    }

    public interface Resource<T> : Resource where T : IResourceAccess {
        T CreateAccess(bool canWrite);
    }
}