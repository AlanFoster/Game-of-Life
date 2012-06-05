using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using EventArgs = System.EventArgs;

namespace GameOfLife.BoilerPlate.ScreenManager {
    public class CustomManager : Manager {
        private bool _playingWorld;
        public bool PlayingWorld {
            get { return _playingWorld; }
            set {
                if (_playingWorld && !value) {
                    RemoveWorldControls();
                }
                _playingWorld = value;
            }
        }
        private List<Component> WorldComponents;

        private Window _mainWindow;
        public Window MainWindow {
            get {
                return _mainWindow;
            }
            set {
                _mainWindow = value;
                _mainWindow.MouseDown += WindowMouseDown;
                _mainWindow.MouseUp += WindowMouseUp;
            }
        }

        private ScreenManager _screenManager;

        public CustomManager(Game game, ScreenManager screenManager)
            : base(game) {
            _screenManager = screenManager;
            WorldComponents = new List<Component>();
        }

        private static void WindowMouseDown(object sender, TomShane.Neoforce.Controls.EventArgs args) {
            GameMouse.GetInstance().LeftButton = ButtonState.Pressed;
        }

        private static void WindowMouseUp(object sender, TomShane.Neoforce.Controls.EventArgs args) {
            GameMouse.GetInstance().LeftButton = ButtonState.Released;
        }

        public override void SendToBack(Control control) {
            if (control != null && !control.StayOnTop) {
                ControlsList cs = (control.Parent == null) ? Controls as ControlsList : control.Parent.Controls as ControlsList;
                if (cs.Contains(control)) {
                    cs.Remove(control);
                    if (!control.StayOnBack) {
                        int pos = control != _mainWindow && _mainWindow != null ? 1 : 0;
                        for (int i = 1; i < cs.Count; i++) {
                            if (!cs[i].StayOnBack) {
                                break;
                            }
                            pos = i;
                        }
                        try {
                            cs.Insert(pos, control);
                        } catch (Exception) {
                            cs.Insert(0, control);
                        }

                    } else {
                        cs.Insert(0, control);
                    }
                }
            }
        }

        public override void Add(Component component) {
            if (PlayingWorld) {
                WorldComponents.Add(component);
            }
            base.Add(component);
        }

        public void RemoveWorldControls() {
            foreach (var control in WorldComponents.ToList()) {
                Remove(control);
            }
        }
    }
}
