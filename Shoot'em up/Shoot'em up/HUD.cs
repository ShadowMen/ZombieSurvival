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
    public class HUD : DrawableGameComponent
    {
        SpriteFont _font;
        SpriteBatch _spriteBatch;
        Viewport _view;

        int _totalFrames = 0, _fps = 0;
        float _elapsed = 0.0f;

        float _health, _maxHealth;
        int _score;

        public HUD(Game game, SpriteBatch spriteBatch, SpriteFont font) : base(game)
        {
            _font = font;
            _spriteBatch = spriteBatch;
        }

        public void Update(GameTime gameTime, Viewport view, float Health, float MaxHealth, int score)
        {
            _view = view;
            _health = Health;
            _maxHealth = MaxHealth;
            _score = score;
        }

        public override void Draw(GameTime gameTime)
        {
            DrawOrder = 1000;

            if (Game1.gameState != GameState.Menu)
            {
                _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.GetMatrix());

                //HP
                string HealthText = string.Format("Health: {0}%", ((_health / _maxHealth) * 100).ToString("0.00"));
                Vector2 healthVector = _font.MeasureString(HealthText);
                _spriteBatch.DrawString(_font, HealthText, new Vector2(-Camera._position.X + _view.Width - healthVector.X, -Camera._position.Y + _view.Height - healthVector.Y), Color.Black);

                //Score
                string ScoreText = string.Format("Score: {0}", _score);
                Vector2 scoreVector = _font.MeasureString(ScoreText);
                _spriteBatch.DrawString(_font, ScoreText, new Vector2(-Camera._position.X, -Camera._position.Y + _view.Height - scoreVector.Y), Color.Black);

                //FPS
                _totalFrames++;
                _elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_elapsed >= 1000)
                {
                    _fps = _totalFrames;
                    _totalFrames = 0;
                    _elapsed = 0;
                }
                _spriteBatch.DrawString(_font, string.Format("FPS: {0}", _fps), -Camera._position, Color.Black);

                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
