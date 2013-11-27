using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Shoot_em_up
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class crosshair : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch _spriteBatch;
        Texture2D _texture;
        public static MouseState _mState;

        public crosshair(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            _spriteBatch = spriteBatch;
            _texture = game.Content.Load<Texture2D>("Texturen\\Crosshair");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (Game1.gameState == GameState.Game)
            {
                _mState = Mouse.GetState();
                if (_mState.X < 0) Mouse.SetPosition(0, _mState.Y);
                else if (_mState.X > Game.GraphicsDevice.Viewport.Width) Mouse.SetPosition(800, _mState.Y);
                if (_mState.Y < 0) Mouse.SetPosition(_mState.X, 0);
                else if (_mState.Y > Game.GraphicsDevice.Viewport.Height) Mouse.SetPosition(_mState.X, Game.GraphicsDevice.Viewport.Height);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Game1.gameState != GameState.Menu)
            {
                DrawOrder = 10;
                _spriteBatch.Begin();
                _spriteBatch.Draw(_texture, new Vector2(_mState.X - _texture.Width / 2, _mState.Y - _texture.Height / 2), Color.White);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
