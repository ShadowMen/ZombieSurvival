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
    public enum MenuState
    {
        Main,
        Option,
        Pause,
        Closed
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Menu : DrawableGameComponent
    {
        SpriteBatch _spriteBatch;
        SpriteFont _font;
        Texture2D _backgroundTexture, _pauseTexture;
        Viewport _view;

        KeyboardState _old, _new;
        MenuState _state;
        List<string> _items = new List<string>();

        int selectedItem = 0, _scaleGrow = 1;
        float _scale = 1;

        #region öffentliche Variabeln
        public MenuState menuState
        {
            get { return _state; }
            set { _state = value; }
        }

        public List<string> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public int SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value; }
        }
        #endregion

        public Menu(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;
            _font = game.Content.Load<SpriteFont>("Font");
            _backgroundTexture = game.Content.Load<Texture2D>("Texturen\\Menu\\MainBg");
            _pauseTexture = game.Content.Load<Texture2D>("Texturen\\Menu\\PauseBg");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime, Viewport view)
        {
            _view = view;
            _new = Keyboard.GetState();
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _scale += _scaleGrow * elapsed;
            if (_scale >= 2)
            {
                _scale = 2;
                _scaleGrow *= -1;
            }
            else if(_scale <= 1)
            {
                _scale = 1;
                _scaleGrow *= -1;
            }

            if (IsKeyPressed(Keys.Up)) selectedItem--;
            else if (IsKeyPressed(Keys.Down)) selectedItem++;

            if (selectedItem > _items.Count - 1) selectedItem = 0;
            else if(selectedItem < 0) selectedItem = _items.Count - 1;

            _old = _new;
        }

        //Items wechseln
        public void ChangeItems(string[] items)
        {
            _items.Clear();
            for (int i = 0; i < items.Length; i++)
            {
                _items.Add(items[i]);
            }
        }

        //Ist Taste Gedrückt
        private bool IsKeyPressed(Keys key)
        {
            return _new.IsKeyDown(key) && _old.IsKeyUp(key);
        }

        //Draw Methode
        public override void Draw(GameTime gameTime)
        {
            if (_state != MenuState.Closed)
            {
                DrawOrder = 9999;
                _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                //Hintergrund
                if (_state == MenuState.Pause) _spriteBatch.Draw(_pauseTexture, new Rectangle(0, 0, _view.Width, _view.Height), Color.White);
                else _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, _view.Width, _view.Height), Color.White);
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i] == null) continue;
                    Vector2 stringVector = _font.MeasureString(_items[i]);

                    if (i == selectedItem) _spriteBatch.DrawString(_font, _items[i], new Vector2(_view.Width / 2, (_view.Height / 3 + stringVector.Y / 2) + 30 * i), Color.Red, 0, new Vector2(stringVector.X / 2, stringVector.Y / 2), _scale, SpriteEffects.None, 1f);
                    else _spriteBatch.DrawString(_font, _items[i], new Vector2(_view.Width / 2 - stringVector.X / 2, _view.Height / 3 + 30 * i), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);

                }
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
