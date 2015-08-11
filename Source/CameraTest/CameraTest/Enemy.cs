using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CameraTest
{
    public enum EnemyType
    {
        Baconite,
        EggMan,
        Cerealian,
        Porrigian
    }

    public enum EnemyMode
    {
        defender,
        random
    }

    /// <summary>
    /// This amazing method spawns all of the enemies required by the game. 
    /// </summary>
    class EnemySpawner
    {
        public static void InitialSpawn(ref List<Enemy> enemyList)
        {
            // The baconites have an initial spawn at their base of 10 bacon men
            for (int i = 0; i < 45; i++)
            {
                int randomIntX = Game1.getRandom(-4000, 4000);
                int randomIntY = Game1.getRandom(-4000, 4000);
                enemyList.Add(new Enemy(new Vector2(GameConfig.BaconPos.X + randomIntX, GameConfig.BaconPos.Y + randomIntY), EnemyType.Baconite, enemyList, EnemyMode.defender));
            }

            // The Egg people have an initial spawn at their base of 25 egg people
            for (int i = 0; i < 25; i++)
            {
                //Random random = new Random();
                int randomIntX = Game1.getRandom(-2500, 2500);
                int randomIntY = Game1.getRandom(-2500, 2500);
                enemyList.Add(new Enemy(new Vector2(GameConfig.EggPos.X + randomIntX, GameConfig.EggPos.Y + randomIntY), EnemyType.EggMan, enemyList, EnemyMode.defender));
            }

            // The porrigians have an initial spawn at their base of 50 
            //for (int i = 0; i < 15; i++)
            //{
            //    Random random = new Random();
            //    int randomIntX = random.Next(-2500, 2500);
            //    int randomIntY = random.Next(-2500, 2500);
            //    enemyList.Add(new Enemy(new Vector2(GameConfig.PorridgePos.X + randomIntX, GameConfig.PorridgePos.Y + randomIntY), EnemyType.Porrigian, enemyList));
            //}

            // The cerealians have an intial spawn at their base of 30
            //for (int i = 0; i < 15; i++)
            //{
            //    Random random = new Random();
            //    int randomIntX = random.Next(-2500, 2500);
            //    int randomIntY = random.Next(-2500, 2500);
            //    enemy.Add(new Enemy(new Vector2(GameConfig.PorridgePos.X + randomIntX, GameConfig.PorridgePos.Y + randomIntY), EnemyType.EggMan, enemy));
            //}
        }

        public static int numberOfEnemiesToSpawn = 15;
        public static int loopsSinceLastRandomSpawn;
        /// <summary>
        /// spawns enemies in space in random positions outside of the enemy race regions every minute
        /// </summary>
        public static void RandomSpawn(ref List<Enemy> enemyList)
        {
            if (loopsSinceLastRandomSpawn % 3600 == 0)
            {
                //a minute has elapsed since we last spawned some random hombres
                //MORE HOMBRES
                Random randomizer = new Random();
                
                //EnemyType randomEnemyType = (EnemyType)randomizer.Next(0, 4);
                EnemyType randomEnemyType = EnemyType.EggMan;
                for (int i = 0; i < numberOfEnemiesToSpawn; i++)
                {
                    int randomX;
                    int randomY;
                    do
                    {
                        randomX = randomizer.Next(-24000, 24000);
                        randomY = randomizer.Next(-24000, 24000);
                    }
                    while (checkNotApplicableSpawn(randomX, randomY, randomEnemyType));
                    Vector2 spawnPoint = new Vector2(randomX, randomY);
                    enemyList.Add(new Enemy(spawnPoint, randomEnemyType, enemyList, EnemyMode.random));
                }
                numberOfEnemiesToSpawn = numberOfEnemiesToSpawn + 10;
            }
            loopsSinceLastRandomSpawn++;
        }

        private static bool checkNotApplicableSpawn(int X, int Y, EnemyType enemyType)
        {
            int spriteWidth = 0;
            int spriteHeight = 0;
            switch (enemyType)
            {
                case EnemyType.Baconite:
                    //Sprite
                    spriteWidth = 100;
                    spriteHeight = 100;
                    break;

                case EnemyType.EggMan:
                    //Sprite Size
                    spriteWidth = 40;
                    spriteHeight = 64;
                    break;

                case EnemyType.Cerealian:
                    //Sprite Size
                    spriteWidth = 64;
                    spriteHeight = 78;
                    break;

                case EnemyType.Porrigian:
                    //Sprite Size
                    spriteWidth = 100;
                    spriteHeight = 100;
                    break;
            }
            Rectangle temporaryBounds = new Rectangle(X, Y, spriteWidth, spriteHeight);
            Rectangle earth = new Rectangle((int)GameConfig.EarthPos.X - 2500, (int)GameConfig.EarthPos.Y - 2500, 5000, 5000);
            Rectangle porrige = new Rectangle((int)GameConfig.PorridgePos.X - 2500, (int)GameConfig.PorridgePos.Y - 2500, 5000, 5000);
            //Rectangle toast = new Rectangle((int)GameConfig.toas.X - 2500, (int)GameConfig.EarthPos.Y - 2500, 5000, 5000);
            Rectangle bacon = new Rectangle((int)GameConfig.BaconPos.X - 2500, (int)GameConfig.BaconPos.Y - 2500, 5000, 5000);
            Rectangle egg = new Rectangle((int)GameConfig.EggPos.X - 2500, (int)GameConfig.EggPos.Y - 2500, 5000, 5000);
            //Rectangle cereal = new Rectangle((int)GameConfig.cr.X - 2500, (int)GameConfig.EarthPos.Y - 2500, 5000, 5000);

            if(temporaryBounds.Intersects(earth) || temporaryBounds.Intersects(porrige) || temporaryBounds.Intersects(bacon) || temporaryBounds.Intersects(egg))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    enum DefenderEnemyState
    {
        patrolling,
        killing
    }

    class Enemy : Character
    {
        // Memebers of the individual enemy
        private EnemyMode mode;
        private EnemyType type;
        private int speedInPixels;
        private int weaponRange;
        private float accuracy; // Between 0 for always missed and 1 for always hits
        private List<Bullet> enemyFiredBulletList = new List<Bullet>();
        private DefenderEnemyState defenderEnemyState;

        //Sprites for each enemy type
        private static Texture2D baconiteTexture;
        private static Texture2D eggManTexture;
        private static Texture2D cerealianTexture;
        private static Texture2D porrigianTexture;

        private Vector2 positionToMoveToOnPatrol = Vector2.Zero;
        private Vector2 directionToMoveTowardsPatrol = Vector2.Zero;

        // Load ALL The sprites!
        public static void LoadContent(ContentManager content)
        {
            eggManTexture = content.Load<Texture2D>("eggEnemy");
            baconiteTexture = content.Load<Texture2D>("baconEnemy");
            //cerealianTexture = content.Load<Texture2D>("Assets/Enemies/Cerealian");
            //porrigianTexture = content.Load<Texture2D>("Assets/Enemies/Porrigian");
        }

        int fireRateInGameLoops = 0;
        int ID = 0;
        public Enemy(Vector2 spawnPosition, EnemyType enemyType, List<Enemy> enemyList, EnemyMode emode)
        {
            // Set initial position and the race of the enemy, passed as params
            Position = spawnPosition;
            type = enemyType;
            mode = emode;

            int spriteWidth = 40;
            int spriteHeight = 64;
            ID = enemyList.Count() - 1;

            //Set properties depending on race of enemy
            switch (enemyType)
            {
                case EnemyType.Baconite:
                    //Race characteristics
                    speedInPixels = 7;
                    weaponRange = 300;
                    accuracy = 0.7f;
                    fireRateInGameLoops = 40;

                    //Sprite
                    //spriteWidth = 100;
                    //spriteHeight = 100;
                    Texture = baconiteTexture;
                    break;

                case EnemyType.EggMan:
                    //Race characteristics
                    speedInPixels = 5;
                    weaponRange = 250;
                    accuracy = 0.8f;
                    fireRateInGameLoops = 30;

                    //Sprite Size
                    spriteWidth = 40;
                    spriteHeight = 64;
                    Texture = eggManTexture;
                    break;

                case EnemyType.Cerealian:
                    //Race characteristics                    
                    speedInPixels = 7;
                    weaponRange = 600;
                    accuracy = 0.6f;
                    fireRateInGameLoops = 60;

                    //Sprite Size
                    spriteWidth = 100;
                    spriteHeight = 100;
                    Texture = cerealianTexture;
                    break;

                case EnemyType.Porrigian:
                    //Race characteristics
                    speedInPixels = 4;
                    weaponRange = 700;
                    accuracy = 0.45f;
                    fireRateInGameLoops = 60;

                    //Sprite Size
                    spriteWidth = 100;
                    spriteHeight = 100;
                    Texture = porrigianTexture;
                    break;
            }

            Bounds = new Rectangle((int)Position.X, (int)Position.Y, spriteWidth, spriteHeight);
        }
        int gameLoopsSinceLastFire = 60;
        float angletoPointRightWay = 0;

        public void Update(Vector2 playerPosition, ContentManager content, List<Enemy> enemyList, GameTime gameTime, Player player)
        {
            Vector2 directionToMoveTowardPlayer = new Vector2();
            Vector2 directionToMoveTowardPlanet = new Vector2();

            if (mode == EnemyMode.random)
            {
                directionToMoveTowardPlayer = (playerPosition - Position);
                directionToMoveTowardPlayer.Normalize();
                Position += directionToMoveTowardPlayer * speedInPixels;
                gameLoopsSinceLastFire++;
            }
            else
            {
                if( mode == EnemyMode.defender)
                {
                    if (defenderEnemyState == DefenderEnemyState.killing)
                    {
                        Vector2 planetPosition = new Vector2();
                        switch (type)
                        {
                            case EnemyType.Baconite:
                                planetPosition = GameConfig.BaconPos;
                                break;

                            case EnemyType.EggMan:
                                planetPosition = GameConfig.EggPos;
                                break;

                            //case EnemyType.Cerealian:
                            //    planetPosition = GameConfig;
                            //    break;

                            case EnemyType.Porrigian:
                                planetPosition = GameConfig.PorridgePos;
                                break;
                        }
                        Rectangle areaToDefend = new Rectangle((int)planetPosition.X - 2500, (int)planetPosition.Y - 2500, 5000, 5000);
                        if (Bounds.Intersects(areaToDefend))
                        {
                            //We're in the defend zone
                            directionToMoveTowardPlayer = (playerPosition - Position);
                            directionToMoveTowardPlayer.Normalize();
                            Position += directionToMoveTowardPlayer * speedInPixels;
                            gameLoopsSinceLastFire++;
                        }
                        else
                        {
                            directionToMoveTowardPlanet = (planetPosition - Position);
                            directionToMoveTowardPlanet.Normalize();
                            Position += directionToMoveTowardPlanet * (speedInPixels * 4);
                            gameLoopsSinceLastFire++;
                        }
                    }

                    if(defenderEnemyState == DefenderEnemyState.patrolling)
                    {
                        Vector2 planetPosition = new Vector2();
                        switch (type)
                        {
                            case EnemyType.Baconite:
                                planetPosition = GameConfig.BaconPos;
                                break;

                            case EnemyType.EggMan:
                                planetPosition = GameConfig.EggPos;
                                break;

                            //case EnemyType.Cerealian:
                            //    planetPosition = GameConfig;
                            //    break;

                            case EnemyType.Porrigian:
                                planetPosition = GameConfig.PorridgePos;
                                break;
                        }

                        if (positionToMoveToOnPatrol == Vector2.Zero || Position == positionToMoveToOnPatrol)
                        {
                            positionToMoveToOnPatrol = new Vector2(planetPosition.X + Game1.getRandom(-100, 100), planetPosition.Y +  Game1.getRandom(-1000, 1000));
                            directionToMoveTowardsPatrol = (positionToMoveToOnPatrol - Position);
                            directionToMoveTowardsPatrol.Normalize();
                            Position += directionToMoveTowardsPatrol * (speedInPixels);

                            angletoPointRightWay = VectorToAngle(directionToMoveTowardsPatrol);
                            Rotation = angletoPointRightWay;
                        }
                        else
                        {
                            Position += directionToMoveTowardsPatrol * (speedInPixels);
                            angletoPointRightWay = VectorToAngle(directionToMoveTowardsPatrol);
                            Rotation = angletoPointRightWay;
                        }

                        if ((playerPosition.X - Position.X) < weaponRange && (playerPosition.Y - Position.Y) < weaponRange)
                        {
                            defenderEnemyState = DefenderEnemyState.killing;
                        }
                    }
                }
            }

            if ((playerPosition.X - Position.X) < weaponRange && (playerPosition.Y - Position.Y) < weaponRange)
            {
                //Within Range,
                //FIRE!
                if (gameLoopsSinceLastFire >= fireRateInGameLoops)
                {
                    enemyFiredBulletList.Add(new Bullet(Position, playerPosition, false, content, false));
                    gameLoopsSinceLastFire = 0;
                }
            }
            for (int i = 0; i < enemyFiredBulletList.Count; i++)
            {
                if (enemyFiredBulletList[i].UpdateBullets(gameTime))
                {
                    enemyFiredBulletList.RemoveAt(i);
                    break;
                }
            }
            Bounds.X = (int)Position.X;
            Bounds.Y = (int)Position.Y;

            foreach (Enemy e in enemyList)
            {
                Vector2 directionTowardsOtherEnemy = (e.Position - Position);
                directionTowardsOtherEnemy.Normalize();

                if (e.ID != ID && Bounds.Intersects(e.Bounds))
                {
                    Position -= directionTowardsOtherEnemy * (speedInPixels);
                }
            }
            angletoPointRightWay = VectorToAngle(directionToMoveTowardPlayer);
            Rotation = angletoPointRightWay;
            gameLoopsSinceLastFire++;
            PlayerCollision(player);
        }

        private void PlayerCollision(Player player)
        {
            for (int i = 0; i < enemyFiredBulletList.Count; i++)
            {
                Rectangle playerBound = new Rectangle((int)player.Position.X - 100, (int)player.Position.Y - 100, 200, 200);
                //Rectangle enemyBound2 = new Rectangle((int)enemyList[i].Position.X + 100, (int)enemyList[i].Position.Y + 100, 200, 200);
                Rectangle bulletBound = new Rectangle((int)enemyFiredBulletList[i].Position.X - 100, (int)enemyFiredBulletList[i].Position.Y - 100, 50, 50);
                    //Rectangle bulletBound2 = new Rectangle((int)bulletList[j].Position.X + 100, (int)bulletList[j].Position.Y + 100, 50, 50);
                    if (playerBound.Intersects(bulletBound))
                    {
                        if (enemyFiredBulletList[i].IsColliding(player))
                        {
                            enemyFiredBulletList.RemoveAt(i);
                            player.Health -= 34;
                            if (player.Health <= 0)
                            {
                                GameConfig.StartVibrate();
                                player.Die();
                            }
                            return;
                        }
                    }
                }
            }
            
        

        public void Draw(SpriteBatch batch, Camera cam)
        {
            //batch.Draw(Texture, Bounds, null, Color.White, cam.Rotation, Vector2.Zero, SpriteEffects.None, 0f);
            batch.Draw(Texture, Bounds, null, Color.White, angletoPointRightWay, Vector2.Zero, SpriteEffects.None, 0);
            for (int i = 0; i < enemyFiredBulletList.Count; i++)
            {
                enemyFiredBulletList[i].Draw(batch);
            }
        }

        float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, -vector.Y);
        }
    }
}
