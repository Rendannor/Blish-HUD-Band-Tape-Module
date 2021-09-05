using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Bh.Racing.Controls;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Controls.Intern;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;

namespace Bh.Racing
{

    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private List<Control> _moduleControls;
        private CornerIcon _bandTapeIcon;
        private ContextMenuStrip _bandTapeIconMenu;
        private Panel _mainPanel;
        private Panel _placeTapePanel;

        internal static Module ModuleInstance;

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        private SettingEntry<float> _settingTransparency;
        private SettingEntry<float> _settingSize;

        private BandTape _bandTape;

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { ModuleInstance = this; }

        protected override void DefineSettings(SettingCollection settings)
        {
            _settingTransparency = settings.DefineSetting("TransparencyCursor", 1.0f, "Transparency", "Transparency of the band tape");
            _settingSize = settings.DefineSetting("SizeCursor", 1.0f, "Size", "Size of the band tape");
        }

        protected override void Initialize()
        {
            _moduleControls = new List<Control>();
            _bandTapeIconMenu = new ContextMenuStrip();
            _bandTapeIcon = new CornerIcon
            {
                IconName = "Band Tape",
                Icon = ContentsManager.GetTexture("band-tape-icon.png"),
                Priority = "Band Tape".GetHashCode()
            };
            _bandTapeIcon.Click += delegate { _bandTapeIconMenu.Show(_bandTapeIcon); };
            var setBandTapePositionItem = new ContextMenuStripItem
            {
                Text = "Set Position",
                Parent = _bandTapeIconMenu
            };
            setBandTapePositionItem.LeftMouseButtonPressed += delegate
            {
                if (GameService.GameIntegration.IsInGame && _placeTapePanel == null) _placeTapePanel = BuildPlaceTapePanel();
                GameService.Input.Mouse.LeftMouseButtonPressed += setTapePosition;
            };
            _moduleControls.Add(setBandTapePositionItem);
            var removeBandTapeItem = new ContextMenuStripItem
            {
                Text = "Remove the band tape",
                Parent = _bandTapeIconMenu
            };
            removeBandTapeItem.LeftMouseButtonPressed += delegate
            {
                _bandTape.position = new System.Drawing.Point(-500, -500);
                _bandTape.UpdateLocation(null, null);
            };
            _moduleControls.Add(removeBandTapeItem);

        }

        private Panel BuildPlaceTapePanel()
        {
            _mainPanel = new Panel
            {
                Parent = GameService.Graphics.SpriteScreen,
                Size = new Point(750, 100),
                Location = new Point(GameService.Graphics.SpriteScreen.Width / 2 - 350,
                    GameService.Graphics.SpriteScreen.Height / 2 - 50),
                ShowBorder = true,
                ShowTint = true,
                Title = "Band Tape",
                Opacity = 0.0f
            };
            var placeTapeLabel = new Label
            {
                Parent = _mainPanel,
                Size = _mainPanel.ContentRegion.Size,
                Location = new Point(0, 20),
                Text =
                    "Left click at the position where you want to put the band tape.",
                Font = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size24,
                    ContentService.FontStyle.Regular),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                StrokeText = true,
                ShowShadow = true
            };
            GameService.Animation.Tweener.Tween(_mainPanel, new { Opacity = 1.0f }, 0.35f);
            return _mainPanel;
        }
        private void setTapePosition(object sender2, MouseEventArgs e)
        {
            var pos = GraphicsService.Graphics.SpriteScreen.RelativeMousePosition;
            _bandTape.position = new System.Drawing.Point(pos.X, pos.Y);
            _bandTape.UpdateLocation(null, null);
            GameService.Input.Mouse.LeftMouseButtonPressed -= setTapePosition;
            GameService.Animation.Tweener.Tween(_mainPanel, new { Opacity = 0.0f }, 0.2f).OnComplete(() =>
            {
                _mainPanel.Dispose();
                _placeTapePanel = null;
            });
        }

        protected override void OnModuleLoaded(EventArgs e)
        {
            _bandTape = new BandTape()
            {
                Parent = GameService.Graphics.SpriteScreen,
            };
            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        protected override void Update(GameTime gameTime)
        {
            _bandTape.transparence = _settingTransparency.Value / 100;
            _bandTape.scale = _settingSize.Value / 100;
            _bandTape.Size = new Point((int)(224 * _bandTape.scale), (int)(56 * _bandTape.scale)); 
        }

        /// <inheritdoc />
        protected override async void Unload()
        {
            // Unload
            await GameService.Gw2WebApi.AnonymousConnection.Connection.CacheMethod.ClearAsync();

            // All static members must be manually unset
            ModuleInstance = null;
        }

    }

}
