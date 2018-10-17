/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Toves.Util.Collections;

namespace Toves.Util.Transaction {
    public class ResourceHelper {
        private static readonly bool Debug = false;
        private static int lastIdAllocated = -1;

        private int id;
        private Dictionary<Thread, List<ResourceAccess>> allocated = new Dictionary<Thread, List<ResourceAccess>>();
        private Queue<Thread> requests = new Queue<Thread>();
        private bool exclusiveLock = false;
        private List<object> acquireLocks = new List<object>();
        private object dataLock = new object();

        public ResourceHelper() {
            int nextId = lastIdAllocated + 1;
            lastIdAllocated = nextId;
            id = nextId;
        }

        public event EventHandler OnModify;

        public void AddAcquireLock(object toAdd) {
            lock (dataLock) {
                acquireLocks.Add(toAdd);
            }
        }

        public void RemoveAcquireLock(object toRemove) {
            lock (dataLock) {
                acquireLocks.Remove(toRemove);
            }
        }

        public override string ToString() {
            return string.Format("resource{0}", id);
        }

        private bool HasWrite(Thread cur) {
            if (allocated.ContainsKey(cur)) {
                foreach (ResourceAccess access in allocated[cur]) {
                    if (access.CanWrite) {
                        return true;
                    }
                }
                return false;
            } else {
                return false;
            }
        }

        private bool CanAcquire(Thread cur, ResourceAccess access) {
            if (access.CanWrite) {
                return allocated.Count == (allocated.ContainsKey(cur) ? 1 : 0);
            } else {
                return !exclusiveLock;
            }
        }

        public void Acquire(ResourceAccess access) {
            List<object> monitors = null;
            try {
                monitors = AcquireAll();
                Thread cur = Thread.CurrentThread;
                bool alreadyHave = access.CanWrite ? HasWrite(cur) : allocated.ContainsKey(cur);
                if (alreadyHave) {
                    // thread already has access - we don't need to wait on ourselves
                } else if (requests.Count == 0 && CanAcquire(cur, access)) {
                    // request can be satisfied immediately
                } else { // we must wait for request to be satisfied
                    if (Debug) {
                        Console.WriteLine("{0}: {1} waiting [{2}/{3}]", this, access, exclusiveLock,
                                          ",".JoinObjectStrings(allocated.Values.SelectMany(x => x)));
                    }
                    requests.Enqueue(cur);
                    while (true) {
                        Monitor.Wait(dataLock);

                        if (requests.Peek() == cur && CanAcquire(cur, access)) {
                            requests.Dequeue();
                            break;
                        }
                    }
                    if (!access.CanWrite) {
                        // maybe next request behind me can also awake
                        Monitor.PulseAll(dataLock);
                    }
                }
                List<ResourceAccess> curAccess;
                if (allocated.ContainsKey(cur)) {
                    curAccess = allocated[cur];
                } else {
                    curAccess = new List<ResourceAccess>();
                    allocated[cur] = curAccess;
                }
                curAccess.Add(access);
                if (access.CanWrite) {
                    exclusiveLock = true;
                }
                if (Debug) {
                    Console.WriteLine("{0}: {1} acquired", this, access);
                }
            } finally {
                foreach (object monitor in monitors) {
                    Monitor.Exit(monitor);
                }
            }
        }

        private List<object> AcquireAll() {
            Monitor.Enter(dataLock);
            List<object> allLocks = new List<object>(acquireLocks);
            bool tryAgain = true;
            int tries = 0;
            while (tryAgain && tries < 100) {
                tries++;
                tryAgain = false;
                for (int i = 0; i < allLocks.Count; i++) {
                    bool entered = Monitor.TryEnter(allLocks[i]);
                    if (!entered) {
                        tryAgain = true;
                        for (int j = 0; j < i; j++) {
                            Monitor.Exit(allLocks[j]);
                        }
                        Thread.Sleep(10);
                    }
                }
            }
            if (tries >= 100) {
                Console.Error.WriteLine("Deadlock detected - going ahead at our own peril");
            }
            allLocks.Add(dataLock);
            return allLocks;
        }

        public void Downgrade(ResourceAccess access, Action toDowngrade) {
            if (access.CanWrite) {
                lock (dataLock) {
                    access.Resource.Hook(ResourceHookType.AfterWrite, access);
                    toDowngrade();
                    if (!HasWrite(Thread.CurrentThread)) {
                        exclusiveLock = false;
                        Monitor.PulseAll(dataLock);
                    }
                    access.Resource.Hook(ResourceHookType.AfterDowngrade, access);
                    EventHandler handler = OnModify;
                    if (handler != null) {
                        handler(this, null);
                    }
                }
            }
        }

        public void Release(ResourceAccess access) {
            lock (dataLock) {
                Thread cur = Thread.CurrentThread;
                List<ResourceAccess> curAccess = allocated[cur];
                if (!curAccess.Contains(access)) {
                    throw new InvalidOperationException("Access to resource is not active");
                }
                curAccess.Remove(access);
                if (curAccess.Count == 0) {
                    allocated.Remove(cur);
                }
                if (access.CanWrite) {
                    EventHandler handler = OnModify;
                    if (handler != null) {
                        handler(this, null);
                    }
                }
                if (access.CanWrite && !HasWrite(cur)) {
                    exclusiveLock = false;
                    Monitor.PulseAll(dataLock); // if head is read request, it can go
                } else if (allocated.Count == 0) {
                    Monitor.PulseAll(dataLock); // whatever is at head can go
                }
                if (Debug) {
                    Console.WriteLine("{0}: {1} released [{2}/{3}/{4}]", this, access, exclusiveLock,
                                      ",".JoinObjectStrings(curAccess),
                                      ",".JoinObjectStrings(allocated.Values.SelectMany(x => x)));
                }
            }
        }
    }
}