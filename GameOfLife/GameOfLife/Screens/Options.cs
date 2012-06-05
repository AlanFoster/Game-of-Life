using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.ScreenManager;
using GameOfLife.Data;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.Screens
{
    class Options : GameLayer
    {
        public static float Volume = 0.5f;
        public static float EffectsVolume = 0.5f;
        private DisplayMode FullScreenResolution;
        private Panel resolutionPanel;
        private List<RadioButton> ScreenResolutions; 

        public Options(string layerName) : base(layerName)
        {
        }

        public override void Initialize()
        {   
            base.Initialize();
            ScreenResolutions = new List<RadioButton>();
            var top = 50;
            var exit = new Button(ControlManager.Manager)
                           {Text = "Back to previous screen", Left = 50, Top = top, Width = 200, Height = 50};
            exit.Init();
            ControlManager.Add(exit);
            exit.Click += (sender, args) => ScreenManager.SwapScreens(this, Constants.ScreenNames.MainMenu);

            var panel = new Panel(ControlManager.Manager) {Width = 856, Height = 467, Left = 300, Top = 50 };
            panel.Init();
            ControlManager.Add(panel);

            var resLabel = new Label(ControlManager.Manager) {Text = "Screen Resolution:", Top = top, Left = 20, Width = 250};
            resLabel.Init();
            panel.Add(resLabel);

            top += 50;

            resolutionPanel = new Panel(ControlManager.Manager)
                                      {Color = Color.Gray, Width = 300, Height = 150, Top = 70, Left = 50, Parent = panel, AutoScroll = true};
            resolutionPanel.Init();
            var resTop = 10;
           
            DisplayMode current = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.Where(i => i.Format == SurfaceFormat.Color && i.Width >= 1224))
            {
                if (mode.Width > current.Width)
                    FullScreenResolution = mode;

                var option = new RadioButton(ControlManager.Manager) { Text = String.Format("{0}x{1}", mode.Width, mode.Height), Width = 200, Left = 50, Top = resTop, Parent = resolutionPanel};
                option.Checked = mode.Width == GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width && mode.Height == GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                int x = mode.Width;
                int y = mode.Height;
                option.Click += (sender, args) => ApplyResolution(x, y);
                resTop += 30;
                option.Init();
                current = mode;
                ScreenResolutions.Add(option);
            }

            top += resolutionPanel.Height;

            var fullScreenModeLabel = new Label(ControlManager.Manager) { Text = "Full Screen Mode:", Top = top, Left = resLabel.Left, Parent = panel, Width = 200 };
            fullScreenModeLabel.Init();

            top += fullScreenModeLabel.Height;

            var fullScreenPanel = new Panel(ControlManager.Manager) { Color = Color.Gray, Width = resolutionPanel.Width, Height = 50, Top = top, Left = resolutionPanel.Left, Parent = panel };
            fullScreenPanel.Init();

            top += fullScreenModeLabel.Height;

            String OnOff = Application.Graphics.IsFullScreen ? "On" : "Off";
            var fullScreenIndicator = new RadioButton(ControlManager.Manager) { Top = top, Left = 100, Width = 200, Parent = panel, Text = "Full Screen: " + OnOff, Checked = Application.Graphics.IsFullScreen };
            fullScreenIndicator.Click += (sender, args) => FullScreenMode(fullScreenIndicator);
            fullScreenIndicator.Init();

            top += fullScreenPanel.Height;

            var backgroundSoundLabel = new Label(ControlManager.Manager) { Text = "Background Volume:", Top = top, Left = 20, Width = 250 };
            backgroundSoundLabel.Init();
            panel.Add(backgroundSoundLabel);
            top += backgroundSoundLabel.Height;

            var backgroundVolumePercentage = new Label(ControlManager.Manager) { Text = "50%", Top = top, Left = 480, Width = 250 };
            backgroundVolumePercentage.Init();
            panel.Add(backgroundVolumePercentage);

            var backgroundVolume = new TrackBar(ControlManager.Manager) {Width = 400, Top = top, Left = 50};
            backgroundVolume.Init();
            panel.Add(backgroundVolume);
            backgroundVolume.Value = 50;
            backgroundVolume.ValueChanged += (sender, args) => ChangeBackgroundVolume(backgroundVolumePercentage, backgroundVolume.Value);
            top += 20;

            var soundEffectsLabel = new Label(ControlManager.Manager) { Text = "Effects Volume:", Top = top, Left = 20, Width = 250 };
            soundEffectsLabel.Init();
            panel.Add(soundEffectsLabel);
            top += 20;

            var effectsVolumePercentage = new Label(ControlManager.Manager) { Text = "50%", Top = top, Left = 480, Width = 250 };
            backgroundVolumePercentage.Init();
            panel.Add(effectsVolumePercentage);

            var effectsVolume = new TrackBar(ControlManager.Manager) {Width = 400, Top = top, Left = 50 };
            effectsVolume.Init();
            panel.Add(effectsVolume);
            effectsVolume.Value = 50;
            effectsVolume.ValueChanged += (sender, args) => ChangeEffectsVolume(effectsVolumePercentage, effectsVolume.Value); 
        }
        private void FullScreenMode(RadioButton fullScreenIndicator)
        {
            Application.Graphics.PreferredBackBufferWidth = 
                    Application.Graphics.IsFullScreen ? 1225 : GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Application.Graphics.PreferredBackBufferHeight = 
                Application.Graphics.IsFullScreen ? 550 : GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            Application.Graphics.ToggleFullScreen();

            Application.Graphics.ApplyChanges();
            
            RecalculatePosition();
            
            resolutionPanel.Enabled = Application.Graphics.IsFullScreen;

            foreach (var screenResolution in ScreenResolutions.Where(i => i.Text == FullScreenResolution.Width + "x" +
                                           FullScreenResolution.Height && Application.Graphics.IsFullScreen).ToArray())
                screenResolution.Checked = true;

            fullScreenIndicator.Checked = Application.Graphics.IsFullScreen;
            fullScreenIndicator.Text = Application.Graphics.IsFullScreen ? "Full Screen: On" : "Full Screen: Off";
        }

        private void ChangeBackgroundVolume(Label label, int level)
        {
            label.Text = level + "%";
            Volume = level / (float)100;
        }

        private void ChangeEffectsVolume(Label label, int level)
        {
            label.Text = level + "%";
            EffectsVolume = level / (float)100;
        }
        private void ApplyResolution(int x, int y)
        {   
            if (Application.Graphics.PreferredBackBufferWidth == x && Application.Graphics.PreferredBackBufferHeight == y)
                return;

            Application.Graphics.PreferredBackBufferWidth = x;
            Application.Graphics.PreferredBackBufferHeight = y;
            Application.Graphics.ApplyChanges();
            ScreenManager.RecalculatePosition();
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {

        }
    }
}
