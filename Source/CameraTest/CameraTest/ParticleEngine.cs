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

namespace ParticleProject
{
    public class Particle
    {
        public const float GRAVITY = 0f;
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Velocity;
        public float Angle;
        public float AngularVelocity;
        public Color col;
        public float Size;
        public int Life;

        public Particle(Texture2D tex, Vector2 pos, Vector2 vel,
            float angle, float angularV, Color color, float size,
                int life)
        {
            this.Texture = tex;
            this.Position = pos;
            this.Velocity = vel;
            this.Angle = angle;
            this.AngularVelocity = angularV;
            this.col = color;
            this.Size = size;
            this.Life = life;
        }

        public void Update()
        {
            this.Life--;
            this.Position += Velocity;
            this.Position.Y += GRAVITY / (Life/2);
        }

        public void Draw(SpriteBatch batch, float Rotation)
        {
            Rectangle source = new Rectangle((int)Position.X, (int)Position.Y, 4, 4 );
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            batch.Draw(Texture, source, null, col, Rotation, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }

    public class ParticleEngine
    {
        private Random rand;
        public Vector2 Location;
        private List<Particle> particleList;
        private Texture2D texture;
        private List<Color> colorList = new List<Color>();
        int EngineLife;
        int EngineCount = 0;

        public ParticleEngine(Texture2D tex, Vector2 pos, int life)
        {
            colorList.Add(Color.Yellow);
            colorList.Add(Color.Orange);
            colorList.Add(Color.Red);
            colorList.Add(Color.Gray);
            Location = pos;
            this.texture = tex;
            this.particleList = new List<Particle>();
            rand = new Random();
            if (life != 0)
            {
                EngineLife = life;
                EngineCount += 1;
            }
        }
   

        //Some cool stuff

        //GOOD FOR EXPLOSIONS
        //Vector2 vel = new Vector2(
        //1.5f + rand.Next(-3, 3),
        //1.5f + rand.Next(-3, 3));

        //SAME, BUT EXPLOSION ONLY UPWARDS
//        Vector2 vel = new Vector2(
//        1.5f + rand.Next(-3, 3),
//        1.5f + rand.Next(-3, 0));

        private Particle GetNewParticle()
        {
            Vector2 pos = Location;
            Vector2 vel = new Vector2(
            1.5f + rand.Next(-3, 3),
            1.5f + rand.Next(-3, 3));
            float angle = 10;
            float angularVel = 0.1f * 2000;
            Color color = colorList[rand.Next(colorList.Count)];
            float size = (float)rand.NextDouble();
            int life = 10 + rand.Next(20);

            return new Particle(texture, pos, vel, angle, angularVel, color, size, life);
        }

        public bool Update(int Total)
        {
            if (Location.X != -1)
            {
                int total = Total;
                for (int i = 0; i < total; i++)
                    particleList.Add(GetNewParticle());
            }
            for (int p = 0; p < particleList.Count; p++)
            {
                particleList[p].Update();
                if (particleList[p].Life <= 0)
                {
                    particleList.RemoveAt(p);
                    p--;
                }
            }
            if (EngineCount != 0)
            {
                EngineCount += 1;
                if (EngineCount >= EngineLife)
                {
                    EngineCount = 0;
                    return true;
                }
            }
            return false;
        }

        public void Draw(SpriteBatch batch, float Rotation)
        {
            for (int i = 0; i < particleList.Count; i++)
                particleList[i].Draw(batch, Rotation);
        }
    
    }
}