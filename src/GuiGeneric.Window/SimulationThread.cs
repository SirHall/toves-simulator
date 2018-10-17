/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Threading;
using Toves.Sim.Model;
using Toves.Util.Transaction;

namespace Toves.GuiGeneric.Window {
    public class SimulationThread {
        private SimulationModel simModel;
        private Thread thread = null;
        private bool stopRequested = false;
        private object monitor = new object();

        public SimulationThread(SimulationModel simModel) {
            this.simModel = simModel;
        }

        public void Start() {
            if (thread == null) {
                thread = new Thread(new ThreadStart(this.Run));
                thread.Start();
            }
        }

        private void Run() {
            while (true) {
                Transaction xn = new Transaction();
                ISimulationAccess testSim = xn.RequestReadAccess(simModel);
                using (xn.Start()) {
                    while (!testSim.IsStepPending() && !stopRequested) {
                        xn.WaitForModify(testSim, monitor, () => stopRequested);
                    }
                }
                if (stopRequested) {
                    break;
                }
                xn = new Transaction();
                ISimulationAccess stepSim = xn.RequestWriteAccess(simModel);
                using (xn.Start()) {
                    stepSim.StepSimulation();
                }
            }
        }
        
        public void RequestStop() {
            lock (monitor) {
                stopRequested = true;
                Monitor.PulseAll(monitor);
            }
        }
    }
}

