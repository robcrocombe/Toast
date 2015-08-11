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

namespace CameraTest
{
    /// <summary>
    /// THIS IS A VERY VERY SHIT DEMO
    /// BUT SHOWS OFF A WORKING CAMERA
    /// THE CAMERA IS KEPT CENTRAL ON THE SPRITE
    /// WE CAN DEFINE ANY OBJECTS TO POSITIVE OR NEGATIVE X AND Y
    /// TO, BASICALLY THE MAXIMUM INTEGER..
    /// PERFORMANCE TESTING WILL BE INTERESTING IF WE DON'T LIMIT IT..
    /// </summary>
    /// 
    public enum GAMESTATE
    {
        MainMenu,
        MainGame,
        GameOver,
        PauseMenu,
        HighScoreMenu,
        Respawn,
        Intro,
    }
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Essentials
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        static Random rand = new Random();
        public static GAMESTATE GameState = GAMESTATE.MainMenu; //WILL CHANGE TO MAIN MENU WHEN WE HAVE A MENU

        //Objects
        Player player;
        Camera cam = new Camera();
        ParticleProject.ParticleEngine engine;

        List<Stars> starList = new List<Stars>();
        List<Bullet> bulletList = new List<Bullet>();
        List<Enemy> enemyList = new List<Enemy>();
        List<Planet> planetList = new List<Planet>();

        List<Powerup> powerupList = new List<Powerup>();
        List<Powerup> powerupsAvailable = new List<Powerup>();

        //Random vars
        float Zoom = 1.0f;
        Vector2 prevPos = Vector2.Zero;

        Texture2D particleTex;

        public List<Stars> getStars(ContentManager content)
        {
            List<Stars> starList = new List<Stars>();
            for (int i = 0; i < 30000; i++)
            {
                Stars temp = new Stars();
                temp.Texture = Content.Load<Texture2D>("Star");
                temp.Position.X = Game1.getRandom(-24000, 24000);
                temp.Position.Y = Game1.getRandom(-24000, 24000);
                temp.Bound = new Rectangle((int)temp.Position.X, (int)temp.Position.Y, 10, 10);
                starList.Add(temp);
            }
            return starList;
        }

        public static int getRandom(int min, int max)
        {
            return rand.Next(min, max);
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            NewGame();
            base.Initialize();
        }

        private void NewGame()
        {
            starList = new List<Stars>();
            bulletList = new List<Bullet>();
            enemyList = new List<Enemy>();
            planetList = new List<Planet>();
            HighscoreManager.Reset();
            powerupList = new List<Powerup>();
            powerupsAvailable = new List<Powerup>();
            starList = getStars(Content);
            particleTex = Content.Load<Texture2D>("particle");
            MakeMap();
            GameConfig.PlayerStartPos = GameConfig.EarthPos - new Vector2(15, 15);
            GameConfig.ParticleTexture = Content.Load<Texture2D>("particle");
            GameConfig.EnemyExplosion = new ParticleProject.ParticleEngine(GameConfig.ParticleTexture, Vector2.Zero, 0);
            GameConfig.EnemyExplosion.Location.X = -1;
            MakePowerups();
            player = new Player(Content);
            player.Lives = 3;
            Enemy.LoadContent(Content);
            EnemySpawner.InitialSpawn(ref enemyList);
            Powerup.SpawnPowerups(Content);
        }

        private void MakeMap()
        {
            Planet earth = new Planet();
            earth.Texture = Content.Load<Texture2D>("Planet");
            earth.Position = new Vector2(getRandom(-22000, 22000), getRandom(-22000, 22000));
            earth.Bounds = new Rectangle((int)earth.Position.X, (int)earth.Position.Y, earth.Texture.Width, earth.Texture.Height);
            planetList.Add(earth);
            Planet egg = new Planet();
            egg.Texture = Content.Load<Texture2D>("eggPlanet");
            bool Intersects = true;
            while (Intersects)
            {
                egg.Position = new Vector2(getRandom(-22000, 22000), getRandom(-22000, 22000));
                egg.Bounds = new Rectangle((int)egg.Position.X, (int)egg.Position.Y, egg.Texture.Width, egg.Texture.Height);
                if (!egg.Bounds.Intersects(earth.Bounds))
                    Intersects = false;
            }
            planetList.Add(egg);
            Planet bacon = new Planet();
            bacon.Texture = Content.Load<Texture2D>("PlanetBacon");
            Intersects = true;
            while (Intersects)
            {
                bacon.Position = new Vector2(getRandom(-22000, 22000), getRandom(-22000, 22000));
                bacon.Bounds = new Rectangle((int)bacon.Position.X, (int)bacon.Position.Y, bacon.Texture.Width, bacon.Texture.Height);
                if (!bacon.Bounds.Intersects(earth.Bounds))
                {
                    if (!bacon.Bounds.Intersects(egg.Bounds))
                    {
                        Intersects = false;
                    }
                }
            }
            planetList.Add(bacon);
            GameConfig.EarthPos = earth.Position;
            GameConfig.EggPos = egg.Position;
            GameConfig.BaconPos = bacon.Position;
        }

        private void MakePowerups()
        {
            for (int i = 0; i < 25; i++)
            {
                Powerup p = new Powerup(new Vector2(getRandom(-22000, 22000), getRandom(-22000, 22000)), Content);
                powerupList.Add(p);
            }
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            engine = new ParticleProject.ParticleEngine(particleTex, new Vector2(20, 20), 0);
            // TODO: use this.Content to load your game content here

            SoundManager.LoadContent(Content);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void UpdateMainGame(GameTime gameTime)
        {
            UpdatePlayer(gameTime);
            UpdateEnemy(gameTime);
            UpdateBullet(gameTime);
            UpdateZoom();
            UpdatePowerup();
            CheckCollisions();
            GameConfig.UpdateVibrate();
            SoundManager.PlaySong(GameSongMode.gameLoop);
            GameConfig.UpdateExplosion(gameTime);
        }

        private void UpdateRespawn(GameTime gameTime)
        {
            GameConfig.UpdateVibrate();
            GetRestart();
        }

        private void GetRestart()
        {
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            if (pad.IsButtonDown(Buttons.Start))
            {
                player.Position = GameConfig.PlayerStartPos;
                player.Update(cam, false);
                GameState = GAMESTATE.MainGame;
            }
        }

        private void UpdateMenu()
        {
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            if (pad.Buttons.Start == ButtonState.Pressed)
            {
                GameState = GAMESTATE.MainGame;
                NewGame();
            }
        }

        private void UpdateGameOver()
        {
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            if (pad.IsButtonDown(Buttons.A))
                GameState = GAMESTATE.MainMenu;
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            switch (GameState)
            {
                case GAMESTATE.MainMenu:
                    {
                        UpdateMenu();
                        break;
                    }
                case GAMESTATE.MainGame:
                    {
                        UpdateMainGame(gameTime);
                        break;
                    }
                case GAMESTATE.PauseMenu:
                    {
                        break;
                    }
                case GAMESTATE.HighScoreMenu:
                    {
                        break;
                    }
                case GAMESTATE.GameOver:
                    {
                        UpdateGameOver();
                        break;
                    }
                case GAMESTATE.Respawn:
                    {
                        UpdateRespawn(gameTime);
                        break;
                    }
                case GAMESTATE.Intro:
                    {
                        break;
                    }
            }
            EnemySpawner.RandomSpawn(ref enemyList);
            base.Update(gameTime);
        }

        private void CheckCollisions()
        {
            BulletEnemyCollision();
            BulletPlanetCollision();
            PlanetPlayerCollision();
        }
        private void BulletEnemyCollision()
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                Rectangle enemyBound1 = new Rectangle((int)enemyList[i].Position.X - 100, (int)enemyList[i].Position.Y - 100, 200, 200);
                //Rectangle enemyBound2 = new Rectangle((int)enemyList[i].Position.X + 100, (int)enemyList[i].Position.Y + 100, 200, 200);
                for (int j = 0; j < bulletList.Count; j++)
                {
                    Rectangle bulletBound1 = new Rectangle((int)bulletList[j].Position.X - 100, (int)bulletList[j].Position.Y - 100, 50, 50);
                    //Rectangle bulletBound2 = new Rectangle((int)bulletList[j].Position.X + 100, (int)bulletList[j].Position.Y + 100, 50, 50);
                    if (enemyBound1.Intersects(bulletBound1))
                    {
                        if (enemyList[i].IsColliding(bulletList[j]))
                        {
                            GameConfig.EnemyExplosion.Location = enemyList[i].Position;
                            GameConfig.ExplosionCount = 1;
                            enemyList.RemoveAt(i);
                            bulletList.RemoveAt(j);
                            HighscoreManager.AddScore(10);
                            return;
                        }
                    }
                }
            }
        }
        private void BulletPlanetCollision()
        {
            if (bulletList.Count > 0)
            {
                for (int i = 0; i < planetList.Count; i++)
                {
                    for (int j = 0; j < bulletList.Count; j++)
                    {
                        if (bulletList[j].Bounds.Intersects(planetList[i].Bounds))
                        {
                            bulletList.RemoveAt(j);
                            planetList[i].Health -= 1;
                            if (planetList[i].Health <= 0)
                            {
                                planetList[i].KillPlanet();
                                HighscoreManager.AddScore(100);
                                break;
                            }
                        }
                    }
                }
            }
        }
        private void PlanetPlayerCollision()
        {
            for (int i = 0; i < planetList.Count; i++)
            {
                Rectangle bigBox = new Rectangle((int)player.Position.X - 50, (int)player.Position.Y - 50, 100, 100);
                if (bigBox.Intersects(planetList[i].Bounds))
                {
                    if (player.IsColliding(planetList[i]))
                    {
                        player.Die();
                        break;
                    }
                }
            }
        }
        private void CheckPowerups()
        {
            //GameConfig.BulletPowerup = false;
            //GameConfig.SpeedPowerup = false;
            //for (int i = 0; i < powerupList.Count; i++)
            //{
            //    if (powerupList[i].Type == POWERUP_TYPE.Speed && powerupList[i].Active)
            //        GameConfig.SpeedPowerup = true;
            //    else if (powerupList[i].Type == POWERUP_TYPE.MoreBullets && powerupList[i].Active)
            //        GameConfig.BulletPowerup = true;
            //}
        }
        private void UpdatePlayer(GameTime gameTime)
        {
            Vector2 offset = new Vector2(21, 51);
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            if (pad.IsButtonDown(Buttons.LeftStick) && GameConfig.SpeedPowerup == false)
            {
                for (int i = 0; i < powerupsAvailable.Count; i++)
                {
                    if (powerupsAvailable[i].Type == POWERUP_TYPE.Speed)
                    {
                        GameConfig.SpeedPowerup = true;
                        powerupsAvailable.RemoveAt(i);
                        break;
                    }
                }
            }
            else if (pad.IsButtonDown(Buttons.RightStick) && GameConfig.BulletPowerup == false)
            {
                for (int i = 0; i < powerupsAvailable.Count; i++)
                {
                    if (powerupsAvailable[i].Type == POWERUP_TYPE.MoreBullets)
                    {
                        GameConfig.BulletPowerup = true;
                        powerupsAvailable.RemoveAt(i);
                        break;
                    }
                }
            }
            GameConfig.UpdatePowerup(gameTime);
            player.Update(cam, GameConfig.SpeedPowerup);

            Matrix m = Matrix.CreateRotationZ(cam.Rotation);
            offset = Vector2.Transform(offset, m);
            Vector2 engineLocation = player.Position + offset;
            if (engineLocation != prevPos)
            {
                engine.Location = engineLocation;
            }
            else
            {
                engine.Location.X = -1;
            }
            engine.Update(player.SPEED);
            prevPos = engineLocation;
        }
        private void UpdateBullet(GameTime gameTime)
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (bulletList[i].UpdateBullets(gameTime))
                {
                    bulletList.RemoveAt(i);
                    break;
                }
            }
            Bullet temp = Bullet.LookForBullet(cam, player, Content, GameConfig.BulletPowerup, gameTime);
            if (temp != null)
                bulletList.Add(temp);
        }
        private void UpdateZoom() //Debug
        {
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            if (pad.Triggers.Left != 0)
                Zoom = MathHelper.Clamp(Zoom - 0.01f, 0.01f, 1f);
            else if (pad.Triggers.Right != 0)
                Zoom = MathHelper.Clamp(Zoom + 0.01f, 0.01f, 1f);
            cam.Zoom = this.Zoom;
        }
        private void UpdatePowerup()
        {
            for (int i = 0; i < powerupList.Count; i++)
            {
                Rectangle bigBox = new Rectangle((int)player.Position.X - 50, (int)player.Position.Y - 50, 100, 100);
                if (bigBox.Intersects(powerupList[i].Bounds))
                {
                    if (player.IsColliding(powerupList[i]))
                    {
                        powerupsAvailable.Add(powerupList[i]);
                        powerupList.RemoveAt(i);
                        break;
                    }
                }
            }
            //for (int i = 0; i < powerupList.Count; i++)
            //{
            //    Rectangle bigBox = new Rectangle((int)powerupList[i].Position.X, (int)powerupList[i].Position.Y,
            //        100, 100);
            //    if (powerupList[i].Update())
            //    {
                    
            //        break;
            //    }
            //    powerupList[i].UpdateCollision(player);
            //}
        }
        private void UpdateEnemy(GameTime gameTime)
        {
            EnemySpawner.RandomSpawn(ref enemyList);
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].Update(player.Position, Content, enemyList, gameTime, player);
            }
            //if (enemyEngine != null)
            //    enemyEngineAlive = enemyEngine.Update(20);
        }

        private void DrawMain()
        {
            GraphicsDevice.Clear(new Color(1, 0, 14));

            spriteBatch.Begin(SpriteSortMode.Deferred,
                            BlendState.AlphaBlend,
                            null,
                            null,
                            null,
                            null,
                            cam.getTransformation(GraphicsDevice));
            cam.Position = player.Position;
            for (int i = 0; i < starList.Count; i++)
                starList[i].Draw(spriteBatch);
            for (int i = 0; i < planetList.Count; i++)
                planetList[i].Draw(spriteBatch);

            for (int i = 0; i < bulletList.Count; i++)
                bulletList[i].Draw(spriteBatch, bulletList[i].Rotation);

            for (int i = 0; i < powerupList.Count; i++)
                powerupList[i].Draw(spriteBatch);

            GameConfig.EnemyExplosion.Draw(spriteBatch, 0);

            engine.Draw(spriteBatch, cam.Rotation);
            for (int i = 0; i < enemyList.Count; i++)
                enemyList[i].Draw(spriteBatch, cam);
            player.Draw(spriteBatch, cam.Rotation);
            

            //if (enemyEngine != null)
            //    if (enemyEngineAlive)
            //        enemyEngine.Draw(spriteBatch, 0);


            //SpriteFont font = Content.Load<SpriteFont>("SpriteFont1");
            //spriteBatch.DrawString(font, player.Position.X.ToString() + "," + player.Position.Y.ToString(), cam.Position, Color.White);
            //spriteBatch.DrawString(font, Zoom.ToString(), cam.Position - new Vector2(400, 240), Color.White);

            spriteBatch.End();

            spriteBatch.Begin();
            for (int i = 0; i < powerupsAvailable.Count; i++)
            {
                spriteBatch.Draw(powerupsAvailable[i].Texture, new Vector2(i * 32, 0), Color.White);
            }
            SpriteFont font = Content.Load<SpriteFont>("SpriteFont1");
            HighscoreManager.Draw(spriteBatch, font);
            spriteBatch.End();
        }

        private void DrawRespawn()
        {
            GraphicsDevice.Clear(new Color(1, 0, 14));

            spriteBatch.Begin();
            Texture2D tex = Content.Load<Texture2D>("deathText");
            Rectangle bound = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            spriteBatch.Draw(tex, bound, Color.White);
            spriteBatch.End();
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (GameState)
            {
                case GAMESTATE.MainGame:
                    {
                        DrawMain();
                        break;
                    }
                case GAMESTATE.Respawn:
                    {
                        DrawRespawn();
                        break;
                    }
                case GAMESTATE.GameOver:
                    {
                        GraphicsDevice.Clear(Color.Black);
                      
                        SpriteFont font = Content.Load<SpriteFont>("SpriteFont2");
                        spriteBatch.Begin();
                        spriteBatch.Draw(Content.Load<Texture2D>("gameOver"), new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                        HighscoreManager.DrawEnd(spriteBatch, font);
                        spriteBatch.End();
                        break;
                    }
                case GAMESTATE.MainMenu:
                    {
                        spriteBatch.Begin();
                        spriteBatch.Draw(Content.Load<Texture2D>("StartScreen"), new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                        spriteBatch.End();
                        break;
                    }
                case GAMESTATE.Intro:
                    {
                        break;
                    }
            }

            base.Draw(gameTime);
        }
    }
}
