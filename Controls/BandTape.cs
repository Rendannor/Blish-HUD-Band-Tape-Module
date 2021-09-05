using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bh.Racing.Controls
{
    public class BandTape : Control
    {

        #region Load Static

        private static readonly Texture2D _bandTapeTexture;

        static BandTape()
        {
            _bandTapeTexture = Module.ModuleInstance.ContentsManager.GetTexture("roller-beetle-tape.png");
        }

        #endregion


        public System.Drawing.Point position = new System.Drawing.Point(-500,-500);
        public float transparence = 1.0f;
        public float scale = 0.5f;

        public BandTape()
        {
            this.Size = new Point((int)(224 * scale), (int)(56 * scale));
      
            UpdateLocation(null, null);
            Graphics.SpriteScreen.Resized += UpdateLocation;
        }

        public void UpdateLocation(object sender, EventArgs e)
        {
            this.Location = new Point(this.position.X - this.Size.X / 2, this.position.Y - this.Size.Y / 2);
        }

        protected override CaptureType CapturesInput() => CaptureType.ForceNone;

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
        {
            spriteBatch.DrawOnCtrl(this,
                                   _bandTapeTexture,
                                   _size.InBounds(bounds),
                                   null,
                                   Color.White * transparence,
                                   0f,
                                   Vector2.Zero);
        }

    }
}
