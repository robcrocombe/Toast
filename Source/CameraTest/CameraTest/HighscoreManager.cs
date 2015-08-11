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
    class HighscoreManager
    {
        private static int numberOfKills = 0;
        private static int numberOfPlanetsKilled = 0;

        private static int score = 0;

        //public static void addKill()
        //{
        //    numberOfKills++;
        //}

        //public static void addPlanetDestroyed()
        //{
        //    numberOfPlanetsKilled++;
        //}

        public static void Reset()
        {
            score = 0;
        }

        public static void AddScore(int Score)
        {
            score += Score;
        }

        //public static void gameEnded(GameTime gt, ContentManager Content, SpriteBatch sb)
        //{
        //    string kills = "You killed " + numberOfKills + " enemies & destroyed " + numberOfPlanetsKilled + " enemy stronghold planets! Well done solider!";
        //    string timeTaken = "You took " + gt.ElapsedGameTime.Minutes + " minutes, " + gt.ElapsedGameTime.Seconds + " seconds and " + gt.ElapsedGameTime.Milliseconds + " milliseconds";
        //    //SpriteFont font = Content.Load<SpriteFont>("SpriteFont1");
        //    //sb.DrawString(font, kills, new Vector2(350, 0), Color.Red);
        //}

        public static void DrawEnd(SpriteBatch batch, SpriteFont font)
        {
            batch.DrawString(font, score.ToString(), new Vector2(600, 400), Color.White);
        }

        public static void Draw(SpriteBatch batch, SpriteFont font)
        {
            string Score = "Score: " + score.ToString();
            Vector2 size = font.MeasureString(Score);
            Vector2 place = new Vector2(400, 0);
            place.X += size.X;
            batch.DrawString(font, Score, place, Color.White);
        }
    }
}
