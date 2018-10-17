/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using System.Threading;

namespace Toves.Util.Transaction {
    public class Transaction : IDisposable {
        private List<IResourceAccess> resources = new List<IResourceAccess>();
        private bool started = false;

        public Transaction() {
        }

        public T RequestReadAccess<T>(Resource<T> resource) where T : IResourceAccess {
            return RequestAccess(resource, false);
        }

        public T RequestWriteAccess<T>(Resource<T> resource) where T : IResourceAccess {
            return RequestAccess(resource, true);
        }

        private T RequestAccess<T>(Resource<T> resource, bool canWrite) where T : IResourceAccess {
            if (started) {
                throw new InvalidOperationException("Cannot request access after transaction begun");
            }
            T access = resource.CreateAccess(canWrite);
            resources.Add(access);
            return access;
        }

        public Transaction Start() {
            foreach (IResourceAccess access in resources) {
                access.Acquire();
            }
            started = true;
            return this;
        }

        public void Dispose() {
            if (started) {
                started = false;
                foreach (IResourceAccess access in resources) {
                    if (access.CanWrite) {
                        access.Resource.Hook(ResourceHookType.AfterWrite, access);
                    }
                }
                foreach (IResourceAccess access in resources) {
                    if (access.CanWrite) {
                        access.Downgrade();
                    }
                }
                foreach (IResourceAccess access in resources) {
                    if (access.CanWrite) {
                        access.Resource.Hook(ResourceHookType.AfterDowngrade, access);
                    } else {
                        access.Resource.Hook(ResourceHookType.AfterRead, access);
                    }
                }
                foreach (IResourceAccess access in resources) {
                    access.Release();
                }
            }
        }

        public void WaitForModify(IResourceAccess access, object stopMonitor, Func<bool> stopTest) {
            bool modified = false;
            EventHandler pulse = (obj, args) => {
                lock (stopMonitor) {
                    modified = true;
                    Monitor.PulseAll(stopMonitor);
                }
            };
            access.Resource.ResourceHelper.AddAcquireLock(stopMonitor);
            access.Resource.ResourceHelper.OnModify += pulse;
            lock (stopMonitor) {
                while (!stopTest() && !modified) {
                    Dispose();
                    Monitor.Wait(stopMonitor);
                }
            }
            access.Resource.ResourceHelper.RemoveAcquireLock(stopMonitor);
            access.Resource.ResourceHelper.OnModify -= pulse;
            Start();
        }
    }
}