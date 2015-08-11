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
    public class SpriteObject
    {
        /// <summary>
        /// The main sprite object used in both the menu and the game. Contains methods for detecting tapping,
        /// per pixel intersects and other events as well as ways of setting up and drawing sprites
        /// </summary>

        //Speed Control
        //temporary values, obv
        private const float X_SPEED = 5f;
        private const float Y_SPEED = 5f;

        //Texture and Colour data used for per pixel collision detection
        private Texture2D texture;
        public Texture2D Texture
        {
            get
            {
                //Give texture
                return texture;
            }
            set
            {
                //Set Texture
                texture = value;
                ColourData = new Color[value.Width * value.Height];
                Texture.GetData(ColourData);
                //#region DepreciatedCode
                ////Create multidimensional colour array used for rotated per pixel collision detection (SEE: HasCollidedWith() method) 

                ////First create a 1D array using the built in Texture method
                //Color[] oneDimensionalColourData = new Color[texture.Width * texture.Height]; 
                //texture.GetData(oneDimensionalColourData); 

                ////Now build the 2D array 
                //ColourData = new Color[texture.Width, texture.Height];
                //for (int x = 0; x < texture.Width; x++)
                //{
                //    for (int y = 0; y < texture.Height; y++)
                //    {
                //        ColourData[x, y] = oneDimensionalColourData[x + y * texture.Width];
                //    }
                //}
                //#endregion
            }
        }

        private Color[] ColourData;

        //Colour used in rotated per pixel
        //private Color[,] ColourData;

        //Float used to show how rotated the sprite is
        public float Rotation;

        //Matrix data used for per pixel collisions on rotated sprites
        private Matrix transformationMatrix
        {
            get
            {
                Matrix matrix = Matrix.CreateRotationZ(Rotation) *
                                Matrix.CreateTranslation(Position.X, Position.Y, 0) *
                                Matrix.Identity;
                return matrix;
            }
            set
            {
                //No set method, read only
            }
        }

        //Accurate positioning with Vector 2
        public Vector2 Position;

        //Rectagle used for drawing
        public int Width;
        public int Height;
        //public Rectangle Bounds
        //{
        //    get
        //    {
        //        //Rectangle dynamically produced from the Vector2 for position and the Width and Height
        //        return new Rectangle((int)Math.Round(Position.X, 0), (int)Math.Round(Position.Y, 0), Width, Height);
        //    }
        //    set
        //    {
        //        this.Bounds = value;
        //    }
        //}

        public Rectangle Bounds;

        //Constructor
        public SpriteObject(float NewX, float NewY, int NewWidth, int NewHeight)
        {
            //Set position
            Position.X = NewX;
            Position.Y = NewY;

            //Set width and height
            Width = NewWidth;
            Height = NewHeight;
        }

        public SpriteObject()
        {

        }

        //Method to Check if the Sprite is being Tapped

        //Integer used to see how long it was since the last press of this sprite. Stops too many presses happening in one go
        //public static int TapCountdown;
        //public bool IsTapped(TouchCollection TC)
        //{
        //    if (TC.Count == 0)
        //    {
        //        //If there are no fingers on the screen the button cannot be being touched, return false
        //        return false;
        //    }
        //    else
        //    {
        //        //There are fingers on the screen, check if any are touching the button
        //        Rectangle fingerPosition;
        //        for (int i = 0; i < TC.Count; i++)
        //        {
        //            fingerPosition = new Rectangle((int)TC[i].Position.X, (int)TC[i].Position.Y, 1, 1);

        //            if (fingerPosition.Intersects(this.Bounds) && TapCountdown == 0)
        //            {
        //                TapCountdown = 10;
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //}

        public bool IsColliding(SpriteObject collidable)
        {
            bool retval = false;

                if (IntersectPixels(this.transformationMatrix, this.Texture.Width, this.Texture.Height, this.ColourData, 
                    collidable.transformationMatrix, collidable.Texture.Width, collidable.Texture.Height, collidable.ColourData))
                {
                    retval = true;
                }
            return retval;
        }

        public static bool IntersectPixels(Matrix transformA, int widthA, int heightA, Color[] dataA, Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }


        //public bool HasCollidedWith(SpriteObject ObjectToCheck)
        //{
        //    #region depreciatedCode
        //    // Get with and height of this texture
        //    int thisTextureWidth = ColourData.GetLength(0);
        //    int thisTextureHeight = ColourData.GetLength(1);

        //    //Invert both matrix
        //    Matrix matrix1to2 = this.transformationMatrix * Matrix.Invert(ObjectToCheck.transformationMatrix);

        //    // Get with and height of the other texture
        //    int otherTextureWidth = ObjectToCheck.ColourData.GetLength(0);
        //    int otherTextureHeight = ObjectToCheck.ColourData.GetLength(1);

        //    // Loop through each pixel of this texture
        //    for (int width = 0; width < thisTextureWidth; width++)
        //    {
        //        for (int height = 0; height < thisTextureHeight; height++)
        //        {
        //            //The texture we're working on
        //            Vector2 thisTexturePixel = new Vector2(width, height);

        //            //This is texture in the other object that is in the same position as this texture
        //            Vector2 otherTexturePixel = Vector2.Transform(thisTexturePixel, matrix1to2);

        //            //Check there is a pixel in the same place as the one we're working on
        //            if ((otherTexturePixel.X >= 0) && (otherTexturePixel.X < otherTextureWidth) && (otherTexturePixel.Y >= 0) && (otherTexturePixel.Y < otherTextureHeight))
        //            {
        //                // Check the alpha (visibility) of the pixels, if its above 0 they both have colour and we have collided!
        //                if (this.ColourData[width, height].A > 0 && ObjectToCheck.ColourData[(int)otherTexturePixel.X, (int)otherTexturePixel.Y].A > 0)
        //                {
        //                    return true;
        //                }
        //            }
        //        }
        //    }
        //    return false;
        //    #endregion
        //}
        //Per Pixel Collision Detection Method
        //public bool HasCollidedWith(SpriteObject ObjectToCheck)
        //{


        //    // Find the bounds of the rectangle intersection
        //    int top = Math.Max(this.Bounds.Top, ObjectToCheck.Bounds.Top);
        //    int bottom = Math.Min(this.Bounds.Bottom, ObjectToCheck.Bounds.Bottom);
        //    int left = Math.Max(this.Bounds.Left, ObjectToCheck.Bounds.Left);
        //    int right = Math.Min(this.Bounds.Right, ObjectToCheck.Bounds.Right);

        //    for (int i = top; i < bottom; i++)
        //    {
        //        for (int j = left; j < right; j++)
        //        {
        //            //Get the colour of the current position of the intersect
        //            Color thisSpritesCurrentPositionColour = this.ColourData[(j - this.Bounds.Left) + (i - this.Bounds.Top) * this.Bounds.Width];
        //            Color objectToChecksCurrentPositionColour = ObjectToCheck.ColourData[(j - ObjectToCheck.Bounds.Left) + (i - ObjectToCheck.Bounds.Top) * ObjectToCheck.Width];

        //            //If both of them aren't alpha (see through) then they have hit!
        //            if (thisSpritesCurrentPositionColour.A != 0 && objectToChecksCurrentPositionColour.A != 0)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    //If we've got his far they haven't collided
        //    return false;
        //}

        //Default draw method, using no special effect and white colour
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(this.Texture, this.Bounds, Color.White);
        }

        //Overloaded draw method which allows different colours to be used
        public virtual void Draw(SpriteBatch sb, Color colour)
        {
            sb.Draw(this.Texture, this.Bounds, colour);
        }

        //Overloaded draw method which allows rotation
        public virtual void Draw(SpriteBatch sb, float rotation)
        {
            sb.Draw(this.Texture, this.Bounds, null, Color.White, rotation, new Vector2(Width / 2, Height / 2), SpriteEffects.None, 0);
        }

        //Overloaded draw method which allows rotation and colour
        public virtual void Draw(SpriteBatch sb, Color colour, float rotation, Vector2 origin)
        {
            sb.Draw(this.Texture, this.Bounds, this.Bounds, colour, rotation, origin, new SpriteEffects(), 9);
        }
    }
}
