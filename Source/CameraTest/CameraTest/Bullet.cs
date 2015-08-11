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
    //This is just a generic "Bullet" class
    //It can, of course, be used for anything
    //where something travels in a straight line
    //from a given position, to a given position
    class Bullet : SpriteObject
    {
        private const int HEIGHT = 16;
        private const int WIDTH = 16;
        private const int SPEED = 5;
        private const int MAX_LIFE = 15 * 100;

        private bool Flip;
        private float Magnitude;
        private Vector2 Motion;

        public static int LastBullet = 0;
        public static int ShootLimit = 200;

        private int Life = 0;

        //To limit the bullets
        //I should have imported my weapons class really
        //But meh. Too late.
        private static int bulletCount;
        private static int bulletsLimit = 30;

        public Vector2 getPos()
        {
            return Position;
        }
        public Rectangle getBounds()
        {
            return Bounds;
        }

        public Bullet(Vector2 start, Vector2 dest, bool flip, ContentManager cont, bool IsPlayer )
        {
            Position = start;
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, WIDTH, HEIGHT);
            Flip = flip;
            Motion = start - dest;
            Magnitude = (float)Math.Sqrt((Motion.X * Motion.X) + (Motion.Y * Motion.Y));
            if (IsPlayer)
                Texture = cont.Load<Texture2D>("playerBullet");
            else
                Texture = cont.Load<Texture2D>("enemyBullet");
            bulletCount++;
            Rotation = -(float)Math.Atan2((double)dest.X, (double)dest.Y);
        }

        public static Bullet LookForBullet(Camera cam, Player player, ContentManager Content, bool powerup, GameTime gameTime)
        {
            LastBullet += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (powerup)
            {
                if (LastBullet >= ShootLimit / 2)
                {
                    GamePadState pad = GamePad.GetState(PlayerIndex.One);
                    if (pad.ThumbSticks.Right.X != 0 || pad.ThumbSticks.Right.Y != 0)
                    {
                        Vector2 offset = new Vector2(21, 51);
                        Matrix m = Matrix.CreateRotationZ(cam.Rotation);
                        offset = Vector2.Transform(offset, m);

                        Vector2 Velocity = pad.ThumbSticks.Right;
                        Velocity.Y *= -1;
                        Velocity.Normalize();

                        LastBullet = 0;
                        GameConfig.StartVibrate();
                        SoundManager.PlayRandomShootingSound();
                        return new Bullet(player.Position + offset, player.Position + offset + Velocity, false, Content, true);
                    }
                }
            }
            else
            {
                if (LastBullet >= ShootLimit)
                {
                    GamePadState pad = GamePad.GetState(PlayerIndex.One);
                    if (pad.ThumbSticks.Right.X != 0 || pad.ThumbSticks.Right.Y != 0)
                    {
                        Vector2 offset = new Vector2(21, 51);
                        Matrix m = Matrix.CreateRotationZ(cam.Rotation);
                        offset = Vector2.Transform(offset, m);

                        Vector2 Velocity = pad.ThumbSticks.Right;
                        Velocity.Y *= -1;
                        Velocity.Normalize();

                        LastBullet = 0;
                        GameConfig.StartVibrate();
                        SoundManager.PlayRandomShootingSound();
                        return new Bullet(player.Position + offset, player.Position + offset + Velocity, false, Content, true);
                    }
                }
            }
            return null;
        }

        public bool UpdateBullets(GameTime gameTime)
        {
            Position.X -= (Motion.X / Magnitude) * 20;
            Position.Y -= (Motion.Y / Magnitude) * 20;
            Bounds.X = (int)Position.X;
            Bounds.Y = (int)Position.Y;
            Life += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (Life >= MAX_LIFE)
            {
                return true;
            }
            else
                return false;
        }

        //Bit obvious?
    }
}
