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
    public class Weapon
    {
        public List<Bullet> Bullets = new List<Bullet>();

        float _fireDelay;
        int _velocity = 1000;
        SoundEffect _Shoot;

        public Weapon(SoundEffect WeaponShoot)
        {
            _Shoot = WeaponShoot;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime, Vector2 playerCenter)
        {
            float _elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _fireDelay -= _elapsed;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && _fireDelay <= 0)
            {
                ShootWeapon(playerCenter);
                _fireDelay = 0.2f;
            }

            foreach (Bullet bullet in Bullets)
            {
                bullet.Update(gameTime, _velocity);
            }

            for (int i = 0; i < Bullets.Count; i++)
            {
                if (!Bullets[i].IsActive) Bullets.RemoveAt(i);
            }
        }

        public bool HasHitted(Rectangle objekt)
        {
            foreach (Bullet bullet in Bullets)
            {
                if (bullet.HitBox.Intersects(objekt) && bullet.IsActive)
                {
                    bullet.IsActive = false;
                    return true;
                }
            }
            return false;
        }

        private void ShootWeapon(Vector2 PlayerCenter)
        {
            Bullet newBullet = new Bullet();
            newBullet.Shoot(PlayerCenter, new Vector2(Mouse.GetState().X, Mouse.GetState().Y) - Camera._position);
            _Shoot.Play(0.5f, 1f, 0f);
            Bullets.Add(newBullet);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            foreach (Bullet bullet in Bullets)
            {
                if (bullet.IsActive) spriteBatch.Draw(texture, bullet.Vector, null, Color.White, bullet.Rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 1f);
            }

        }
    }
}
