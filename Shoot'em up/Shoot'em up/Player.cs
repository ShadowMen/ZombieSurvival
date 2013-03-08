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
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch _spriteBatch;
        KeyboardState _newKbState, _oldKbState;

        //Player
        Texture2D _PlayerTexture;
        Vector2 _vector;
        Rectangle _animation, _drawRect;
        Vector2 _move;

        //Weapon
        Texture2D _BulletTexture;

        float _health = 50, _maxhealth = 50, _frameTime, _scale = 1.2f;
        viewDirection _direction;

        Weapon _weapon;

        int _frame, _score = 0, _velocity = 140;

        #region Öffentliche Variabeln
        //Vector
        public Vector2 Vector
        {
            get { return _vector; }
            set { _vector = value; }
        }
        //Texturen
        public Texture2D BulletTextur
        {
            get { return _BulletTexture; }
        }

        public Texture2D PlayerTextur
        {
            get { return _PlayerTexture; }
        }
        //Waffe
        public Weapon weapon
        {
            get { return _weapon; }
        }

        public Rectangle HitBox
        {
            get { return _drawRect; }
        }

        public float Health
        {
            get { return _health; }
            set { _health = value; }
        }

        public float MaxHealth
        {
            get { return _maxhealth; }
            set { _maxhealth = value; }
        }

        public int Score
        {
            get { return _score; }
            set { _score = value; }
        }
        #endregion

        public Player(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;
            
            //Player
            _PlayerTexture = game.Content.Load<Texture2D>("Texturen\\Player\\Player");
            _BulletTexture = game.Content.Load<Texture2D>("Texturen\\Player\\Bullet");
            _vector = new Vector2(500, 500);

            //Weapon
            _weapon = new Weapon(game.Content.Load<SoundEffect>("Sounds\\Weapon\\Shoot"));
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if(Game1.gameState == GameState.Game)
            {
            _newKbState = Keyboard.GetState();
            float _elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Player Bewegung
            if (_newKbState.IsKeyDown(Keys.S)) _move.Y = _velocity * _elapsed;
            else if (_newKbState.IsKeyDown(Keys.W)) _move.Y = -_velocity * _elapsed;
            else _move.Y = 0;
            
            if (_newKbState.IsKeyDown(Keys.D)) _move.X = _velocity * _elapsed;
            else if (_newKbState.IsKeyDown(Keys.A)) _move.X = -_velocity * _elapsed;
            else _move.X = 0;

            _vector += _move;

            _drawRect = new Rectangle((int)_vector.X, (int)_vector.Y, (int)(32 * _scale), (int)(48 * _scale));

            //PlayerDirection
            float rotation = MathHelper.ToDegrees((float)Math.Atan2(Mouse.GetState().Y - _drawRect.Center.Y - Camera._position.Y, Mouse.GetState().X - _drawRect.Center.X - Camera._position.X));
            
            if (rotation > -135 && rotation <= -45) _direction = viewDirection.Up;
            else if (rotation > -45 && rotation <= 45) _direction = viewDirection.Right;
            else if (rotation > 45 && rotation <= 135) _direction = viewDirection.Down;
            else if ((rotation > 135||rotation <= 180) && (rotation <= -135 || rotation >= -180)) _direction = viewDirection.Left;

            //Player verhindern vom Feld zu gehen
            if (_vector.X < 0) _vector.X = 0;
            else if (_vector.X > 1000 - _drawRect.Width) _vector.X = 1000 - _drawRect.Width;
            if (_vector.Y < 0) _vector.Y = 0;
            else if (_vector.Y > 1000 - _drawRect.Height) _vector.Y = 1000 - _drawRect.Height;

            //player animieren
            if (_move != Vector2.Zero)
            {
                _frameTime -= _elapsed;
                if (_frameTime <= 0)
                {
                    _frameTime = 0.1f;
                    _frame++;
                }

                if (_frame > 3) _frame = 0;
            }
            else _frame = 0;

            _animation = new Rectangle(32 * _frame, 48 * (int)_direction, 32, 48);

            //Waffe Updaten
            _weapon.Update(gameTime, new Vector2(_drawRect.Center.X, _drawRect.Center.Y));

            _oldKbState = _newKbState;
        }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Game1.gameState != GameState.Menu)
            {
                _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.GetMatrix());

                //Bullet
                _weapon.Draw(_spriteBatch, _BulletTexture);

                //Player
                _spriteBatch.Draw(_PlayerTexture, _drawRect, _animation, Color.White);

                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public void Reset()
        {
            weapon.Bullets.Clear();
            _health = _maxhealth;
            _score = 0;
            _vector = new Vector2(500, 500);
        }
    }
}
