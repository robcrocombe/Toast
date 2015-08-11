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
    public class Stars
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Rectangle Bound;


        public void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture, Bound, Color.White);
        }


    }
}
