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
    public class Particle
    {
        Vector2 _vector, _move;

        float _lifetime, _rotation;

        //Öffentliche Variabel
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

        public float Lifetime
        {
            get { return _lifetime; }
            set { _lifetime = value; }
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Particle()
        {
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            if (_lifetime > 0)
            {
                float _elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _lifetime -= 10 * _elapsed;
                _vector += (_move - _vector)* 20 *_elapsed;
            }
        }
    }
}
