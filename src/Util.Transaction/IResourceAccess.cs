/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Util.Transaction {
    public interface IResourceAccess {
        Resource Resource { get; }

        bool CanWrite { get; }

        bool IsLive { get; }

        void CheckReadAccess();

        void CheckWriteAccess();

        void Downgrade();

        void Acquire();

        void Release();
    }
}