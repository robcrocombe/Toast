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

        public static void addKill()
        {
            numberOfKills++;
        }

        public static void addPlanetDestroyed()
        {
            numberOfPlanetsKilled++;
        }

        public static void gameEnded(GameTime gt)
        {
            string kills = "You killed " + numberOfKills + " enemies & destroyed " + numberOfPlanetsKilled + " enemy stronghold planets! Well done solider!";
            string timeTaken = "You took " + gt.ElapsedGameTime.Minutes + " minutes, " + gt.ElapsedGameTime.Seconds + " seconds and " + gt.ElapsedGameTime.Milliseconds + " milliseconds";
        }
    }
}
