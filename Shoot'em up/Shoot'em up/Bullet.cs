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
    public class Bullet
    {
        Vector2 _vector, _move, _target;

        float _lifeTime, _rotation;
        bool _isActive;

        //Öffentliche Variabeln
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        public Vector2 Vector
        {
            get { return _vector; }
        }

        public float Rotation
        {
            get { return _rotation; }
        }

        public Rectangle HitBox
        {
            get { return new Rectangle((int)_vector.X, (int)_vector.Y, 15, 5); }
        }

        public Bullet()
        {

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime, int velocity)
        {
            if(_isActive)
            {
                float _elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _vector += _move * velocity * _elapsed;
                _lifeTime -= 3 * _elapsed;
                if (_lifeTime <= 0) _isActive = false;
                if (_vector == _target) _isActive = false;
            }
        }

        public void Shoot(Vector2 startVector, Vector2 TargetVector)
        {
            _vector = startVector;
            _target = TargetVector;
            _move = TargetVector - _vector;
            _move.Normalize();
            _rotation = (float)Math.Atan2(TargetVector.Y - _vector.Y, TargetVector.X - _vector.X);
            _lifeTime = 2;
            _isActive = true;
        }
    }
}
