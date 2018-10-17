/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Diagnostics;
using System.Threading;
using Toves.Sim.Model;
using Toves.Util.Transaction;

namespace Toves.GuiGeneric.LayoutCanvas {
    internal class RepaintThread {
        private static readonly int MillisecondsBetweenRepaints = 40;

        private LayoutCanvasModel canvas;
        private SimulationModel simModel = null;
        private Thread thread = null;
        private bool stopRequested = false;
        private bool repaintRequested = false;
        private Stopwatch stopwatch = new Stopwatch();
        private object monitor = new object();

        public RepaintThread(LayoutCanvasModel canvas) {
            this.canvas = canvas;
        }

        public void Start() {
            if (thread == null) {
                thread = new Thread(new ThreadStart(this.Run));
                thread.Start();
            }
        }

        public void SetSimulationModel(SimulationModel value) {
            lock (monitor) {
                SimulationModel oldValue = simModel;
                if (value != oldValue) {
                    simModel = value;
                    if (oldValue != null) {
                        oldValue.SimulationModelModifiedEvent -= SimulationUpdated;
                    }
                    if (value != null) {
                        value.SimulationModelModifiedEvent += SimulationUpdated;
                    }
                }
            }
        }

        private void SimulationUpdated(object sender, SimulationModelModifiedArgs args) {
            RequestRepaint();
        }

        public void RequestRepaint() {
            lock (monitor) {
                if (!repaintRequested) {
                    repaintRequested = true;
                    Monitor.PulseAll(monitor);
                }
            }
        }
        
        public void RequestStop() {
            lock (monitor) {
                stopRequested = true;
                Monitor.PulseAll(monitor);
            }
        }

        private void Run() {
            lock (monitor) {
                stopwatch.Restart();
            }
            while (true) {
                lock (monitor) {
                    while (true) {
                        if (stopRequested) {
                            break;
                        }
                        if (repaintRequested) {
                            long toWait = MillisecondsBetweenRepaints - stopwatch.ElapsedMilliseconds;
                            if (toWait > 0) {
                                Monitor.Wait(monitor, (int) toWait);
                            } else {
                                break;
                            }
                        } else {
                            Monitor.Wait(monitor);
                        }
                    }
                }
                if (stopRequested) {
                    break;
                }
                canvas.RepaintCanvas();
            }
        }

        public void NotifyRepainted() {
            lock (monitor) {
                stopwatch.Restart();
            }
        }
    }
}

