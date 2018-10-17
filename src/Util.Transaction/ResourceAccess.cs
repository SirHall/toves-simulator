/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;

namespace Toves.Util.Transaction {
    public abstract class ResourceAccess : IResourceAccess {
        private static int lastAccessId = -1;

        private int id;

        protected ResourceAccess(Resource resource, bool canWrite) {
            int nextId = lastAccessId + 1;
            lastAccessId = nextId;
            this.id = nextId;
            this.Resource = resource;
            this.IsLive = false;
            this.CanWrite = canWrite;
        }

        public override string ToString() {
            return string.Format("{0}{1}{2}", this.IsLive ? "lock" : "dead", this.CanWrite ? "X" : "S", this.id);
        }

        public Resource Resource { get; private set; }

        public bool CanWrite { get; private set; }

        public bool IsLive { get; private set; }

        public void CheckReadAccess() {
            if (!IsLive) {
                throw new InvalidOperationException("attempt to access outside transaction");
            }
        }

        public void CheckWriteAccess() {
            if (!IsLive || !CanWrite) {
                throw new InvalidOperationException("attempt to access outside transaction");
            }
        }

        public void Acquire() {
            Resource.ResourceHelper.Acquire(this);
            IsLive = true;
        }

        public void Downgrade() {
            if (CanWrite) {
                Resource.ResourceHelper.Downgrade(this, () => CanWrite = false);
            }
        }

        public void Release() {
            Resource.ResourceHelper.Release(this);
            IsLive = false;
        }
    }
}