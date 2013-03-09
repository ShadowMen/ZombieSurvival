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
    public enum viewDirection
    {
        Down = 0,
        Left = 1,
        Right = 2,
        Up = 3
    }

    public enum GameState
    {
        Game,
        Menu,
        Paused
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D boden;

        Player player;
        HUD playerHUD;
        Menu menu;

        List<Enemy> Zombies = new List<Enemy>();
        Blood blood;
        crosshair Crosshair;

        KeyboardState newKbState, oldKbState;

        public static GameState gameState = new GameState();

        float elapsedTime = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Window Size
            Window.Title = "Zombie Survival";
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;

            //Tickrate
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;

            //Multi Sampling
            graphics.PreferMultiSampling = false;

            //Fullscreen
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Boden
            boden = Content.Load<Texture2D>("Texturen\\Ground\\Ground");

            //PlayerHUD
            playerHUD = new HUD(this, spriteBatch, Content.Load<SpriteFont>("Font"));
            Components.Add(playerHUD);

            //Menu
            menu = new Menu(this, spriteBatch);
            Components.Add(menu);
       
            //Blut
            blood = new Blood(this, spriteBatch);
            Components.Add(blood);

            //player hinzufügen
            player = new Player(this, spriteBatch);
            Components.Add(player);

            //Crosshair hinzufügen
            Crosshair = new crosshair(this, spriteBatch);
            Components.Add(Crosshair);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            newKbState = Keyboard.GetState();

            //Objekte Aktualisieren
            playerHUD.Update(gameTime, GraphicsDevice.Viewport, player.Health, player.MaxHealth, player.Score);
            Camera.Update(gameTime, player.Vector, GraphicsDevice.Viewport);
            for (int i = 0; i < Zombies.Count; i++) Zombies[i].Update(gameTime, player.Vector);

            //Menu Items
            if (menu.menuState == MenuState.Main)
            {
                string[] items = { "Start", "Optionen", "Beenden" };
                menu.ChangeItems(items);
                gameState = GameState.Menu;
            }
            else if (menu.menuState == MenuState.Option)
            {
                string[] items = { "Zurueck" };
                menu.ChangeItems(items);
                gameState = GameState.Menu;
            }
            else if (menu.menuState == MenuState.Pause)
            {
                string[] items = { "Fortsetzen", "Beenden" };
                menu.ChangeItems(items);
                gameState = GameState.Paused;
            }
            else
            {
                //Ist Fenster nicht mehr aktiv, dann pausier das Spiel
                if (!IsActive) menu.menuState = MenuState.Pause;
                else gameState = GameState.Game;
            }

            //Menu Update
            menu.Update(gameTime, GraphicsDevice.Viewport);
            if (IsKeyPressed(Keys.Enter) && menu.menuState != MenuState.Closed)
            {
                switch (menu.menuState)
                {
                    case MenuState.Main:
                        if (menu.SelectedItem == 0)
                        {
                            ResetGame();
                            menu.menuState = MenuState.Closed;
                        }
                        else if (menu.SelectedItem == 1) menu.menuState = MenuState.Option;
                        else if (menu.SelectedItem == 2) this.Exit();
                        break;
                    case MenuState.Option:
                        if (menu.SelectedItem == 0) menu.menuState = MenuState.Main;
                        break;
                    case MenuState.Pause:
                        if (menu.SelectedItem == 0) menu.menuState = MenuState.Closed;
                        else if (menu.SelectedItem == 1) menu.menuState = MenuState.Main;
                        break;
                }
                menu.SelectedItem = 0;
            }

            //Durch drücken der Escape Taste ins Pause Menu
            if (IsKeyPressed(Keys.Escape) && gameState == GameState.Game) menu.menuState = MenuState.Pause;

            if (gameState == GameState.Game)
            {
                elapsedTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                //Zombie Spawnen
                if (elapsedTime <= 0)
                {
                    SpawnEnemy();
                    elapsedTime += 1600;
                }

                UpdateZombieList();

                //Zombie mit Zombies vergleichen
                for (int i = 0; i < Zombies.Count; i++)
                {
                    for (int a = 0; a < Zombies.Count; a++)
                    {
                        if (a == i) continue;

                        //gegenseitig abprallen
                        if (Zombies[i].HitBox.Intersects(Zombies[a].HitBox))
                        {
                            Vector2 Distace = new Vector2(Zombies[i].HitBox.Center.X, Zombies[i].HitBox.Center.Y) - new Vector2(Zombies[a].HitBox.Center.X, Zombies[a].HitBox.Center.Y);
                            Distace.Normalize();
                            Zombies[i].Vector += Distace;
                        }

                        //Zombie vor oder hinter Zombie zeichnen
                        if (Zombies[i].Vector.Y < Zombies[a].Vector.Y) Zombies[i].DrawOrder = Zombies[a].DrawOrder + 1;
                        else if (Zombies[i].Vector.Y > Zombies[a].Vector.Y) Zombies[i].DrawOrder = Zombies[a].DrawOrder - 1;
                    }
                }
                
                foreach (Enemy zombie in Zombies)
                {
                    //Überprüfung ob Zombie vor oder hinter dem Spieler ist
                    if (player.Vector.Y + 32 < zombie.Vector.Y + 40) zombie.DrawOrder = player.DrawOrder + 1;
                    else zombie.DrawOrder = player.DrawOrder - 1;

                    //Zombie auf Schaden überprüfen
                    if (player.weapon.HasHitted(zombie.HitBox))
                    {
                        zombie.GetHitted(4.1f);
                        if (zombie.Health <= 0) player.Score += 20;
                        blood.CreateBlood(new Vector2(zombie.HitBox.Center.X, zombie.HitBox.Center.Y));
                    }

                    //Spieler auf Schaden überprüfen
                    if (zombie.HitBox.Intersects(player.HitBox))
                    {
                        player.Health -= 2.3f;

                        //Ist Spieler tot gehe ins Hauptmenu
                        if (player.Health <= 0) menu.menuState = MenuState.Main;

                        //Spieler vom Zombie wegschlagen
                        Vector2 Distance = new Vector2(player.HitBox.Center.X, player.HitBox.Center.Y) - new Vector2(zombie.HitBox.Center.X, zombie.HitBox.Center.Y);
                        player.Vector += Distance;
                    }
                }

            }

            oldKbState = newKbState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (gameState != GameState.Menu)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.GetMatrix());
                //Boden
                spriteBatch.Draw(boden, new Rectangle(0, 0, 1000, 1000), Color.White);

                spriteBatch.End();
            }
                

            base.Draw(gameTime);
        }

        private void SpawnEnemy()
        {
            //neuen Zombie erstellen
            Random rnd = new Random();
            Enemy zombie = new Enemy(this, spriteBatch);
            zombie.MaxHealth = 20;
            zombie.Health = zombie.MaxHealth;
            zombie.Vector = new Vector2(rnd.Next(100, 900), rnd.Next(100, 900));

            Components.Add(zombie);
            //Zombie zur Liste hinzufügen
            Zombies.Add(zombie);
        }

        private void UpdateZombieList()
        {
            for (int i = 0; i < Zombies.Count; i++)
            {
                if (Zombies[i].Health <= 0)
                {
                    Components.Remove(Zombies[i]);
                    Zombies.RemoveAt(i);
                }
            }
        }
        
        private bool IsKeyPressed(Keys key)
        {
            return newKbState.IsKeyDown(key) && oldKbState.IsKeyUp(key);
        }

        private void ResetGame()
        {
            while(Zombies.Count > 0)
            {
                Components.Remove(Zombies[0]);
                Zombies.RemoveAt(0);
            }
            blood.Clear();
            player.Reset();
            Camera._position = -player.Vector/2;
        }
    }
}
