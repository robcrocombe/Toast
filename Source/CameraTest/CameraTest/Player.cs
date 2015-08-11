using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace CameraTest
{
    class Player : Character
    {
        public Vector2 Motion;
        public int SPEED = 10;
        public int Lives = 3;

        public Player(ContentManager content)
        {
            Position = GameConfig.PlayerStartPos;
            Texture = content.Load<Texture2D>("Player");
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, 43, 61);
        }

        public void Update(Camera cam, bool speed)
        {
            SPEED = 10;
            if (speed)
                SPEED *= 2;
            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            if (pad.IsConnected)
            {
                if (pad.ThumbSticks.Left.X != 0 || pad.ThumbSticks.Left.Y != 0)
                {
                    Position.X += pad.ThumbSticks.Left.X * SPEED;
                    Position.Y -= pad.ThumbSticks.Left.Y * SPEED;
                    Position.X = MathHelper.Clamp(Position.X, -24000, 24000);
                    Position.Y = MathHelper.Clamp(Position.Y, -24000, 24000);
                    cam.Rotation = (float)Math.Atan2(pad.ThumbSticks.Left.X, pad.ThumbSticks.Left.Y);
                }
            }
            if (Motion != Vector2.Zero)
            {
                Motion.Y = Motion.Y - MathHelper.ToRadians(cam.Rotation);
            }

            Bounds.X = (int)Position.X;
            Bounds.Y = (int)Position.Y;
            Rotation = cam.Rotation;
        }

        public void Die()
        {
            Lives--;
            if (Lives <= 0)
            {
                Game1.GameState = GAMESTATE.GameOver;
                return;
            }
            else
            {
                Game1.GameState = GAMESTATE.Respawn;
                return;
            }
        }

    }
}