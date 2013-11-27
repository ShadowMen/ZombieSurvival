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
    public static class Camera
    {
        public static Vector2 _position;

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public static void Update(GameTime gameTime, Vector2 playerVector, Viewport view)
        {
            if (Game1.gameState == GameState.Game)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                //Kamera verfolgt den Spieler
                if (_position.X > (-playerVector.X + view.Width / 2) + (view.Width / 5)) _position.X -= 140 * elapsed;
                else if (_position.X < (-playerVector.X + view.Width / 2) - (view.Width / 5)) _position.X += 140 * elapsed;

                if (_position.Y > (-playerVector.Y + view.Height / 2) + (view.Height / 5)) _position.Y -= 140 * elapsed;
                else if (_position.Y < (-playerVector.Y + view.Height / 2) - (view.Height / 5)) _position.Y += 140 * elapsed;
            }
        }

        public static Matrix GetMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(_position, 0));
        }
    }
}
