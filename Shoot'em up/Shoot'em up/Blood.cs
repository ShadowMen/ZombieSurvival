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
    public class Blood : Microsoft.Xna.Framework.DrawableGameComponent
    {
        List<Particle> _Particles = new List<Particle>();
        SpriteBatch _spriteBatch;
        Texture2D _texture;

        public Blood(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;
            _texture = game.Content.Load<Texture2D>("Texturen\\Blood\\Blood");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (Game1.gameState == GameState.Game)
            {
                foreach (Particle particle in _Particles)
                {
                    particle.Update(gameTime);
                }

                for (int i = 0; i < _Particles.Count; i++)
                {
                    if (_Particles[i].Lifetime <= 0) _Particles.RemoveAt(i);
                }
            }

            base.Update(gameTime);
        }

        public void CreateBlood(Vector2 StartVector)
        {
            Random rnd = new Random();
            for (int i = 0; i < 4; i++)
            {
                Particle newParticle = new Particle();
                newParticle.Vector = StartVector;
                newParticle.MoveVector = StartVector + new Vector2(rnd.Next(-30, 30), rnd.Next(-30, 30));
                newParticle.Rotation = (float)rnd.Next(0, 360);
                newParticle.Lifetime = rnd.Next(30, 60);

                _Particles.Add(newParticle);
            }
        }

        public void Clear()
        {
            _Particles.Clear();
        }

        public override void Draw(GameTime gameTime)
        {
            if (Game1.gameState != GameState.Menu)
            {
                DrawOrder = -10;
                foreach (Particle particle in _Particles)
                {
                    _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.GetMatrix());
                    _spriteBatch.Draw(_texture, particle.Vector, null, Color.White, particle.Rotation, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    _spriteBatch.End();
                }
            }

            base.Draw(gameTime);
        }
    }
}
