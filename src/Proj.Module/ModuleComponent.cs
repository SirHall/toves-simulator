/* Copyright (c) 2013, Carl Burch.  License information is in the GtkMain.cs
 * source file and at www.toves.org/. */
using System;
using System.Collections.Generic;
using Toves.Layout.Comp;
using Toves.Layout.Data;
using Toves.Sim.Inst;
using Toves.Util.Transaction;

namespace Toves.Proj.Module {
    public class ModuleComponent : Component, Pokeable {
        private Func<String> nameGetter;
        private ProjectModule module;

        public ModuleComponent(ProjectModule module, Func<String> nameGetter) {
            this.nameGetter = nameGetter;
            this.module = module;
            this.Connections = module.Presentation.Connections;
            module.AddComponent(this);
        }

        public override string Name {
            get {
                return nameGetter();
            }
        }

        public ProjectModule Module {
            get { return module; }
        }

        public override Bounds OffsetBounds {
            get { return module.Presentation.OffsetBounds; }
        }

        public override bool ShouldSnap {
            get { return true; }
        }

        public override bool Contains(int offsetX, int offsetY) {
            return module.Presentation.Contains(offsetX, offsetY);
        }

        public override void Paint(IComponentPainter painter) {
            module.Presentation.Paint(painter);

            ModuleInstance instance = painter.Instance as ModuleInstance;
            int glassColor = 0;
            switch (instance.PokeState) {
            case ModuleInstance.FirstDepress:
                glassColor = 0xFF9999;
                break;
            case ModuleInstance.Touched:
                glassColor = 0;
                break;
            case ModuleInstance.SecondDepress:
                glassColor = 0xA00000;
                break;
            default:
                glassColor = 0xBBBBBB;
                break;
            }
            Bounds bds = this.OffsetBounds;
            int glassX = bds.X + bds.Width / 2 - 4;
            int glassY = bds.Y + bds.Height / 2 - 4;
            painter.Color = glassColor;
            painter.StrokeWidth = 4;
            painter.StrokeCircle(glassX, glassY, 14);
            painter.StrokeLine(glassX + 10, glassY + 10, glassX + 18, glassY + 18);
        }

        public override Instance CreateInstance() {
            return module.Implementation.CreateInstance(this);
        }

        public override void Propagate(IInstanceState state) {
            module.Implementation.Propagate(state);
        }
        
        public void ProcessPokeEvent(PokeEventArgs args) {
            switch (args.Type) {
            case PokeEventType.PokeStart:
                args.StateUpdate = (IInstanceState state) => {
                    ModuleInstance instance = state.Instance as ModuleInstance;
                    int oldState = instance.PokeState;
                    int newState = oldState;
                    if (IsInGlass(args)) {
                        if (oldState == ModuleInstance.Untouched) {
                            newState = ModuleInstance.FirstDepress;
                        } else if (oldState == ModuleInstance.Touched) {
                            newState = ModuleInstance.SecondDepress;
                        }
                    } else {
                        newState = ModuleInstance.Untouched;
                        args.RejectPoke();
                    }
                    if (newState != oldState) {
                        instance.PokeState = newState;
                        args.Repaint();
                    }
                };
                break;
            case PokeEventType.PokeEnd:
                args.StateUpdate = (IInstanceState state) => {
                    ModuleInstance instance = state.Instance as ModuleInstance;
                    int oldState = instance.PokeState;
                    int newState = oldState;
                    if (IsInGlass(args)) {
                        if (oldState == ModuleInstance.FirstDepress) {
                            newState = ModuleInstance.Touched;
                        } else if (oldState == ModuleInstance.SecondDepress) {
                            newState = ModuleInstance.Untouched;
                            args.RequestView(module, instance.SimulationState);
                        }
                    } else {
                        if (oldState == ModuleInstance.FirstDepress) {
                            newState = ModuleInstance.Untouched;
                        } else if (oldState == ModuleInstance.SecondDepress) {
                            newState = ModuleInstance.Touched;
                        }
                    }
                    if (newState != oldState) {
                        instance.PokeState = newState;
                        args.Repaint();
                    }
                };
                break;
            case PokeEventType.PokeCancel:
                args.StateUpdate = (IInstanceState state) => {
                    ModuleInstance instance = state.Instance as ModuleInstance;
                    int oldState = instance.PokeState;
                    int newState = oldState;
                    if (oldState == ModuleInstance.FirstDepress) {
                        newState = ModuleInstance.Untouched;
                    } else if (oldState == ModuleInstance.SecondDepress) {
                        newState = ModuleInstance.Touched;
                    }
                    if (newState != oldState) {
                        instance.PokeState = newState;
                        args.Repaint();
                    }
                };
                break;
            }
        }

        public void PaintPokeProgress(IComponentPainter painter) {
        }

        private bool IsInGlass(PokeEventArgs args) {
            Bounds bds = this.OffsetBounds;
            int dx = args.X - (bds.X + bds.Width / 2 - 4);
            int dy = args.Y - (bds.Y + bds.Height / 2 - 4);
            return dx * dx + dy * dy < 16 * 16;
        }

        public override Component Clone() {
            ModuleComponent result = base.Clone() as ModuleComponent;
            result.module.AddComponent(result);
            return result;
        }
    }
}

