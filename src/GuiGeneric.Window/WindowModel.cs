/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.GuiGeneric.LayoutCanvas;
using Toves.AbstractGui.Menu;
using Toves.Layout.Comp;
using Toves.Layout.Model;
using Toves.Proj.Model;
using Toves.Proj.Module;
using Toves.Sim.Model;
using Toves.Util.Transaction;

namespace Toves.GuiGeneric.Window {
    public class WindowModel : IDisposable, CanvasCallback {
        private SimulationThread simThread;
        private Dictionary<ProjectModule, LayoutSimulation> moduleSimulations
            = new Dictionary<ProjectModule, LayoutSimulation>();
        private List<GenericMenu> menus = new List<GenericMenu>();

        public WindowModel() {
            this.Project = new Project();
            Transaction xn = new Transaction();
            IProjectAccess proj = xn.RequestWriteAccess(this.Project);
            ProjectModule currentModule;
            using (xn.Start()) {
                currentModule = proj.AddModule("main", new LayoutModel());
            }

            this.LayoutCanvas = new LayoutCanvasModel(this);
            this.ToolboxModel = new ToolboxModel(this);
            this.ToolbarModel = new ToolbarModel(this);
            this.simThread = null;

            menus.Add(new ProjectMenu(this));
            SetView(currentModule);
        }

        public Project Project { get; private set; }

        public ProjectModule CurrentModule { get; private set; }

        public IEnumerable<GenericMenu> Menus { get { return menus; } }

        public LayoutCanvasModel LayoutCanvas { get; private set; }

        public Component CanvasAdding {
            get {
                GestureAdd add = LayoutCanvas.Gesture as GestureAdd;
                return add == null ? null : add.Master;
            }
        }

        public ToolboxModel ToolboxModel { get; private set; }

        public ToolbarModel ToolbarModel { get; private set; }

        public void Dispose() {
            simThread.RequestStop();
        }

        public void SetView(ProjectModule module) {
            SetView(module, null);
        }

        public void SetView(ProjectModule module, LayoutSimulation sim) {
            if (module.Implementation is LayoutModel) {
                LayoutModel layoutModel = (LayoutModel) module.Implementation;
                LayoutSimulation layoutSim;
                if (sim != null) {
                    layoutSim = sim;
                    moduleSimulations[module] = sim;
                } else if (!moduleSimulations.TryGetValue(module, out layoutSim)) {
                    SimulationModel simModel = new SimulationModel();
                    layoutSim = new LayoutSimulation(simModel, layoutModel);
                    moduleSimulations[module] = layoutSim;
                }
                SimulationThread newThread = new SimulationThread(layoutSim.SimulationModel);
                newThread.Start();
                SimulationThread oldThread = simThread;
                simThread = newThread;
                if (oldThread != null) {
                    oldThread.RequestStop();
                }
                this.CurrentModule = module;
                ToolboxModel.UpdateCurrent(module);
                LayoutCanvas.SetView(layoutModel, layoutSim);
            } else {
                throw new InvalidOperationException("cannot view this module type");
            }
        }

        public void BeginAdd(Component comp, Action onDone) {
            if (comp == null) {
                LayoutCanvas.Gesture = null;
            } else {
                GestureAdd g = new GestureAdd(LayoutCanvas, comp);
                if (onDone != null) {
                    g.GestureCompleteEvent += (sender, e) => {
                        if (sender == g) {
                            onDone();
                        }
                    };
                }
                LayoutCanvas.Gesture = g;
            }
        }

        public void StopAdd(Component comp) {
            GestureAdd cur = LayoutCanvas.Gesture as GestureAdd;
            if (cur != null && cur.Master == comp) {
                LayoutCanvas.Gesture = null;
            }
        }
    }
}

