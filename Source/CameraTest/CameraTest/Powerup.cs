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
    public enum POWERUP_TYPE
    {
        Speed,
        MoreBullets,
    }
    class Powerup : SpriteObject
    {
        public POWERUP_TYPE Type;
        public bool Active = false; //Has it been picked up yet?

        public int activeTime;
        public int activeCount = 0;

        public static List<Powerup> SpawnPowerups(ContentManager content)
        {
            List<Powerup> powerups = new List<Powerup>();
            for (int i = 0; i < 10; i++)
            {
                Powerup temp = new Powerup(new Vector2(Game1.getRandom(-24000, 24000), Game1.getRandom(-24000, 24000)), content);
                powerups.Add(temp);
            }
            return powerups;
        }

        public Powerup(Vector2 pos, ContentManager cont)
        {
            int type = Game1.getRandom(0, 2);
            if (type == 0)
            {
                Type = POWERUP_TYPE.Speed;
                activeTime = 5 * 60;
                Texture = cont.Load<Texture2D>("jamJar");
            }
            else if (type == 1)
            {
                Type = POWERUP_TYPE.MoreBullets;
                activeTime = 10 * 60;
                Texture = cont.Load<Texture2D>("butterTub");
            }
            Position = pos;
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }

        public bool Update() //If returns true, needs to be deleted
        {
            if (Active)
            {
                if (activeCount >= activeTime)
                {
                    return true;
                }
                else
                    activeCount += 1;
            }
            else
            {

            }
            return false;
        }

        public void UpdateCollision(Player player)
        {
            if (player.Bounds.Intersects(this.Bounds))
            {
                Active = true;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            if (!Active)
            {
                batch.Draw(Texture, Position, Color.White);
            }
        }
    }
}
