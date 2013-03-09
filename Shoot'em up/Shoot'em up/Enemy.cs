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
    public class Enemy : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch _spriteBatch;

        //Texturen
        Texture2D _Enemytexture, _healthTexture;

        //Soundeffekte
        SoundEffectInstance _GetHitted;
        SoundEffect _Dying;

        //Vectoren und Rechtecke
        Vector2 _vector, _move;
        Rectangle _animation, _healthBox, _healthSource, _drawRect;

        //Sonstiges
        float _health, _maxhealth, _frameTime, _moveTime = 0, _scale = 1.2f;
        viewDirection _direction;

        int _frame, _velocity = 100;
        bool _isPlayerNear;

#region öffentliche Variabeln
        public Vector2 Vector
        {
            get { return _vector; }
            set { _vector = value; }
        }

        public Vector2 MoveVector
        {
            get { return _move; }
            set { _move = value; }
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

        public Rectangle HitBox
        {
            get { return _drawRect; }
        }

        public bool FollowPlayer
        {
            get { return _isPlayerNear; }
        }
#endregion

        

        public Enemy(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;
            _Enemytexture = game.Content.Load<Texture2D>("Texturen\\Zombie\\Zombie");
            _healthTexture = game.Content.Load<Texture2D>("Texturen\\Zombie\\HealthBar");
            _GetHitted = game.Content.Load<SoundEffect>("Sounds\\Zombie\\Hitted").CreateInstance();
            _Dying = game.Content.Load<SoundEffect>("Sounds\\Zombie\\Dieing");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime, Vector2 PlayerVector)
        {
            //Sound Pausieren
            if (Game1.gameState == GameState.Paused && _GetHitted.State == SoundState.Playing) _GetHitted.Pause();
            if (Game1.gameState == GameState.Game && _GetHitted.State == SoundState.Paused) _GetHitted.Resume();

            if (Game1.gameState == GameState.Game)
            {
                Random rnd = new Random(gameTime.TotalGameTime.Milliseconds);
                float _elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _moveTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                //DrawRect updaten
                _drawRect = new Rectangle((int)_vector.X, (int)_vector.Y, (int)(_animation.Width * _scale), (int)(_animation.Height * _scale));

                //Zombie bewegen
                if ((_isPlayerNear = IsPlayerNear(PlayerVector)))
                {
                    Vector2 distanz = PlayerVector - _vector;
                    distanz.Normalize();

                    _move = distanz * _velocity;
                }
                else if (_moveTime <= 0)
                {
                    _direction = RandomDirection();
                    if (_direction == viewDirection.Down) _move = new Vector2(0, _velocity / 2);
                    else if (_direction == viewDirection.Left) _move = new Vector2(_velocity / 2, 0);
                    else if (_direction == viewDirection.Right) _move = new Vector2(-_velocity / 2, 0);
                    else if (_direction == viewDirection.Up) _move = new Vector2(0, -_velocity / 2);
                    _moveTime = 4000;
                }
                _vector += _move * _elapsed;

                //Zombie direction
                if (_move.X < 0 && (_move.Y < 0.5f && _move.Y > -0.5f)) _direction = viewDirection.Left;
                else if (_move.X > 0 && (_move.Y < 0.5f && _move.Y > -0.5f)) _direction = viewDirection.Right;
                else if (_move.Y > 0 && (_move.X < 0.5f && _move.X > -0.5f)) _direction = viewDirection.Down;
                else if (_move.Y < 0 && (_move.X < 0.5f && _move.X > -0.5f)) _direction = viewDirection.Up;

                //Zombie verhindern, dass er vom Feld geht
                if (_vector.X < 0)
                {
                    _vector.X = 0;
                    _move *= -1;
                }
                else if (_vector.X > 1000 - _drawRect.Width)
                {
                    _vector.X = 1000 - _drawRect.Width;
                    _move *= -1;
                }
                if (_vector.Y < 0)
                {
                    _vector.Y = 0;
                    _move *= -1;
                }
                else if (_vector.Y > 1000 - _drawRect.Height)
                {
                    _vector.Y = 1000 - _drawRect.Height;
                    _move *= -1;
                }

                //Zombie animieren
                if (_move != Vector2.Zero)
                {
                    _frameTime -= _elapsed;
                    if (_frameTime <= 0)
                    {
                        _frame++;
                        _frameTime = 0.2f;
                    }
                    if (_frame > 3) _frame = 0;
                }
                _animation = new Rectangle((_Enemytexture.Width / 4) * _frame, (_Enemytexture.Height / 4) * (int)_direction, _Enemytexture.Width / 4, _Enemytexture.Height / 4);

                //HealthBar aktualisieren
                _healthBox = new Rectangle((int)_vector.X, (int)_vector.Y - 10, (int)((_health / _maxhealth) * _drawRect.Width), _healthTexture.Height / 2);
                _healthSource = new Rectangle(0, _healthTexture.Height / 2, (int)((_health / _maxhealth) * _drawRect.Width), _healthTexture.Height / 2);
            }

            base.Update(gameTime);
        }

        private viewDirection RandomDirection()
        {
            Random rnd = new Random();
            int rDirection = rnd.Next(1, 3);
            if (rDirection == 1) return viewDirection.Down;
            else if (rDirection == 2) return viewDirection.Left;
            else if (rDirection == 3) return viewDirection.Right;
            else return viewDirection.Up;
        }

        public override void Draw(GameTime gameTime)
        {
            if (Game1.gameState != GameState.Menu)
            {
                _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.GetMatrix());

                _spriteBatch.Draw(_Enemytexture, _drawRect, _animation, Color.White);
                if (_health < _maxhealth)
                {
                    _spriteBatch.Draw(_healthTexture, new Rectangle(_healthBox.X, _healthBox.Y, _drawRect.Width, _healthTexture.Height / 2), new Rectangle(0, 0, _healthTexture.Width, _healthTexture.Height / 2), Color.BlueViolet);
                    _spriteBatch.Draw(_healthTexture, _healthBox, _healthSource, Color.White);
                }
                
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private bool IsPlayerNear(Vector2 player)
        {
            return Vector2.Distance(_vector, player) < 200;
        }

        public void GetHitted(float Strenght)
        {
            _health -= Strenght;

            //Play Sound
            if (_GetHitted.State != SoundState.Playing && _health > 0) _GetHitted.Play();
            if (_health <= 0)
            {
                _GetHitted.Stop();
                _Dying.Play();
            }
        }
    }
}
