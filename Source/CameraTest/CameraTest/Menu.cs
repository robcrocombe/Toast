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
    class Menu
    {
        //private static SpriteObject background;
        //private static List<Stars> stars;
        //private static SpriteObject logo;
        //private static SpriteObject tileBackground;

        //private static SpriteObject playIcon;
        //private static SpriteObject highscoreIcon;
        //private static SpriteObject achievementIcon;
        //private static SpriteObject settingIcon;

        //public static void Initialize(GraphicsDevice graphics)
        //{
        //    background = new SpriteObject(0,0,2000,2000);
        //    logo = new SpriteObject(graphics.Viewport.Width/2 - 150 , 300, 300, 150);
        //}

        //public static void LoadContent(ContentManager Content)
        //{
        //    getStars(Content);
        //    background.Texture = Content.Load<Texture2D>("Menu/Background");
        //    logo.Texture = Content.Load<Texture2D>("Menu/Logo");
        //    highscoreIcon.Texture = Content.Load<Texture2D>("Menu/HighscoreIcon");
        //    achievementIcon.Texture = Content.Load<Texture2D>("Menu/AchievementIcon");
        //    settingIcon.Texture = Content.Load<Texture2D>("Menu/SettingIcon");
        //}

        //public static void Draw(SpriteBatch spriteBatch)
        //{
        //    foreach (Stars s in stars)
        //    {
        //        s.Draw(spriteBatch);
        //    }
        //}

        //public static List<Stars> getStars(ContentManager Content)
        //{
        //    List<Stars> starList = new List<Stars>();
        //    for (int i = 0; i < 30000; i++)
        //    {
        //        Stars temp = new Stars();
        //        temp.Texture = Content.Load<Texture2D>("Star");
        //        temp.Position.X = Game1.getRandom(-24000, 24000);
        //        temp.Position.Y = Game1.getRandom(-24000, 24000);
        //        temp.Bound = new Rectangle((int)temp.Position.X, (int)temp.Position.Y, 10, 10);
        //        starList.Add(temp);
        //    }
        //    return starList;
        //}
    }
}
