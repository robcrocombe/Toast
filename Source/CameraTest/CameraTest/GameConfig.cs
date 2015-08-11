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
using ParticleProject;

namespace CameraTest
{
    public static class GameConfig
    {
        public static Vector2 EarthPos;
        public static Vector2 EggPos;
        public static Vector2 BaconPos;
        public static Vector2 PorridgePos;
        //public static Vector2 EarthPos;
        //public static Texture2D EarthTexture;
        //public static Rectangle earthBound;
        //public static int EarthHealth = 100;

        //public static void KillEgg()
        //{

        //}

        //public static void KillBacon()
        //{

        //}

        //public static Vector2 EggPos;
        //public static Texture2D EggTexture;
        //public static Rectangle eggBound;
        //public static int EggHealth = 100;

        //public static Vector2 BaconPos;
        //public static Texture2D BaconTexture;
        //public static Texture2D BaconRingTexture;
        //public static Rectangle baconBound;
        //public static int BaconHealth = 100;

        //public static Vector2 PorridgePos;
        //public static Texture2D PorridgeTexture;

        public static int VibrationCount;
        public static int VibrationLimit = 5;

        public static Vector2 PlayerStartPos;

        public static ParticleEngine EnemyExplosion;
        public static Texture2D ParticleTexture;
        public static int ExplosionCount = 0;

        public static bool SpeedPowerup, BulletPowerup;
        public static int SpeedCount, BulletCount;
        public static int SpeedLimit = 1000 * 10;
        public static int BulletLimit = 1000 * 20;

        //public static void SetupMap(int mapSize, ContentManager content)
        //{ //Makes the positions of the planets
        //    EarthTexture = content.Load<Texture2D>("Planet");
        //    EggTexture = content.Load<Texture2D>("eggPlanet");
        //    BaconTexture = content.Load<Texture2D>("PlanetBacon");
        //    BaconRingTexture = content.Load<Texture2D>("PlanetBaconRings");

        //    bool Intersects = true;
        //    while (Intersects)
        //    {
        //        EarthPos = GetPosition(mapSize);
        //        EggPos = GetPosition(mapSize);
        //        Rectangle earthRect = new Rectangle((int)EarthPos.X, (int)EarthPos.Y, 3000, 3000);
        //        Rectangle eggRect = new Rectangle((int)EggPos.X, (int)EggPos.Y, 3000, 3000);
        //        if (earthRect.Intersects(eggRect))
        //        {
        //            Intersects = true;
        //        }
        //        else
        //        {
        //            Intersects = false;
        //        }
        //    }
        //    Intersects = true;
        //    while (Intersects)
        //    {
        //        BaconPos = GetPosition(mapSize);
        //        Rectangle baconRect = new Rectangle((int)BaconPos.X, (int)BaconPos.Y, 3000, 3000);
        //        Rectangle earthBound = new Rectangle((int)EarthPos.X, (int)EarthPos.Y, 3000, 3000);
        //        Rectangle eggBound = new Rectangle((int)EggPos.X, (int)EggPos.Y, 3000, 3000);
        //        if (baconRect.Intersects(earthBound))
        //        {
        //            Intersects = true;
        //        }
        //        else if (baconRect.Intersects(eggBound))
        //        {
        //            Intersects = true;
        //        }
        //        else
        //        {
        //            Intersects = false;
        //        }
        //    }
        //}

        //private static Vector2 GetPosition(int mapSize)
        //{
        //    return new Vector2(Game1.getRandom((mapSize * -1), mapSize), Game1.getRandom((mapSize * -1), mapSize));
        //}

        public static void StartVibrate()
        {
            GamePad.SetVibration(PlayerIndex.One, 1f, 1f);
            VibrationCount = 1;
        }

        public static void UpdateVibrate()
        {
            if (VibrationCount > 0)
            {
                VibrationCount++;
                if (VibrationCount >= VibrationLimit)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                    VibrationCount = 0;
                }
            }
        }

        public static void UpdatePowerup(GameTime gameTime)
        {
            if (SpeedPowerup == true)
            {
                SpeedCount += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (SpeedCount >= SpeedLimit)
                {
                    SpeedPowerup = false;
                    SpeedCount = 0;
                }
            }
            if (BulletPowerup == true)
            {
                BulletCount += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (BulletCount >= BulletLimit)
                {
                    BulletPowerup = false;
                    BulletCount = 0;
                }
            }
        }

        public static void UpdateExplosion(GameTime gameTime)
        {
            if (ExplosionCount > 0)
            {
                ExplosionCount += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (ExplosionCount >= 100)
                {
                    ExplosionCount = 0;
                    EnemyExplosion.Location.X = -1;
                }
            }
            if (EnemyExplosion.Location.X != -1)
            {
                EnemyExplosion.Update(20);
            }
            else
            {
                EnemyExplosion.Update(0);
            }
        }

    }
}
